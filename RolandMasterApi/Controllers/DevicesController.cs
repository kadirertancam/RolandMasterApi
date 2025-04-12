using Microsoft.AspNetCore.Mvc;
using RolandMasterApi.Models;
using RolandMasterApi.Services;

namespace RolandMasterApi.Controllers
{
    /// <summary>
    /// Roland VT-4 cihazlarını yönetmek için API
    /// </summary>
    [ApiController]
    [Route("api/devices")]
    public class DevicesController : ControllerBase
    {
        private readonly RedisService _redisService;
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(RedisService redisService, ILogger<DevicesController> logger)
        {
            _redisService = redisService;
            _logger = logger;
        }

        /// <summary>
        /// Tüm Roland cihazlarını listeler
        /// </summary>
        /// <remarks>
        /// Örnek yanıt:
        /// 
        ///     [
        ///       {
        ///         "deviceId": "00155D4FA27F",
        ///         "deviceName": "VT-4 Stüdyo 1",
        ///         "macAddress": "00155D4FA27F",
        ///         "midiDeviceId": 1,
        ///         "isActive": true,
        ///         "activeEffect": "robot",
        ///         "robotOctave": 2,
        ///         "robotFeedbackSwitch": 1,
        ///         "robotFeedbackResonance": 120,
        ///         "robotFeedbackLevel": 160
        ///       },
        ///       {
        ///         "deviceId": "E8ABCDEF1234",
        ///         "deviceName": "VT-4 Stüdyo 2",
        ///         "macAddress": "E8ABCDEF1234",
        ///         "midiDeviceId": 2,
        ///         "isActive": true,
        ///         "activeEffect": "harmony"
        ///       }
        ///     ]
        /// </remarks>
        /// <returns>Sistemde kayıtlı tüm Roland VT-4 cihazlarının listesi</returns>
        /// <response code="200">Cihaz listesi başarıyla getirildi</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<RolandClientSettings>>> GetAllDevices()
        {
            try
            {
                var devices = await _redisService.GetAllClientSettingsAsync();
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihazları listelerken hata oluştu");
                return StatusCode(500, new { message = "Cihazlar listelenirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Belirli bir Roland cihazının ayarlarını getirir
        /// </summary>
        /// <param name="deviceId">Cihazın benzersiz ID'si</param>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     GET /api/devices/00155D4FA27F
        ///     
        /// Örnek yanıt:
        /// 
        ///     {
        ///       "deviceId": "00155D4FA27F",
        ///       "deviceName": "VT-4 Stüdyo 1",
        ///       "macAddress": "00155D4FA27F",
        ///       "midiDeviceId": 1,
        ///       "isActive": true,
        ///       "activeEffect": "robot",
        ///       "robotOctave": 2,
        ///       "robotFeedbackSwitch": 1,
        ///       "robotFeedbackResonance": 120,
        ///       "robotFeedbackLevel": 160,
        ///       "harmonyH1Level": 200,
        ///       "harmonyH2Level": 150,
        ///       "harmonyH3Level": 100,
        ///       "harmonyH1Key": 0,
        ///       "harmonyH2Key": 4,
        ///       "harmonyH3Key": 7,
        ///       "harmonyH1Gender": 128,
        ///       "harmonyH2Gender": 128,
        ///       "harmonyH3Gender": 128,
        ///       "masterVolume": 100,
        ///       "isMuted": false
        ///     }
        /// </remarks>
        /// <returns>Cihazın tüm ayarlarını içeren detaylı bilgiler</returns>
        /// <response code="200">Cihaz bilgileri başarıyla getirildi</response>
        /// <response code="404">Belirtilen ID'li cihaz bulunamadı</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpGet("{deviceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RolandClientSettings>> GetDeviceById(string deviceId)
        {
            try
            {
                // DeviceId'yi standart formata getir (MAC adresi formatını düzenle)
                deviceId = deviceId.Replace(":", "").Replace("-", "").ToUpper();
                
                _logger.LogInformation("GetDeviceById: Searching for device with ID: {DeviceId}", deviceId);
                
                // Directly check Redis key first to troubleshoot
                var redisKeys = await _redisService.GetAllRedisKeysAsync();
                _logger.LogInformation("Available Redis keys: {Keys}", string.Join(", ", redisKeys));
                
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                
                if (device == null)
                {
                    // Try also checking for the key directly
                    var rawValue = await _redisService.GetRedisValueAsync($"{RedisConfiguration.DeviceKeyPrefix}{deviceId}");
                    if (rawValue != null)
                    {
                        _logger.LogInformation("Found device in Redis with direct key access");
                        try {
                            device = System.Text.Json.JsonSerializer.Deserialize<RolandClientSettings>(rawValue);
                            return Ok(device);
                        } catch (Exception jsonEx) {
                            _logger.LogError(jsonEx, "Failed to deserialize device data");
                        }
                    }
                    
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                return Ok(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz bilgisi alınırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Cihaz bilgisi alınırken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Yeni bir Roland cihazı kaydeder
        /// </summary>
        /// <param name="settings">Kaydedilecek cihaz bilgileri</param>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/devices
        ///     {
        ///        "deviceName": "VT-4 Studio 1",
        ///        "macAddress": "00:11:22:33:44:55",
        ///        "midiDeviceId": 1
        ///     }
        /// 
        /// </remarks>
        /// <returns>Yeni oluşturulan cihaz bilgileri</returns>
        /// <response code="201">Cihaz başarıyla oluşturuldu</response>
        /// <response code="400">Geçersiz istek verileri</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RolandClientSettings>> RegisterDevice([FromBody] RolandClientSettings settings)
        {
            if (string.IsNullOrEmpty(settings.DeviceId))
            {
                settings.DeviceId = Guid.NewGuid().ToString();
            }
            
            try
            {
                await _redisService.SetClientSettingsAsync(settings);
                
                return CreatedAtAction(nameof(GetDeviceById), new { deviceId = settings.DeviceId }, settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz kaydedilirken hata oluştu");
                return StatusCode(500, new { message = "Cihaz kaydedilirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Bir Roland cihazının ayarlarını günceller
        /// </summary>
        /// <param name="deviceId">Güncellenecek cihazın ID'si</param>
        /// <param name="settings">Güncellenecek cihaz bilgileri</param>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     PUT /api/devices/device-001
        ///     {
        ///        "deviceName": "VT-4 Studio 1 (Güncellendi)",
        ///        "isActive": true,
        ///        "midiDeviceId": 2
        ///     }
        /// 
        /// </remarks>
        /// <returns>Güncellenmiş cihaz bilgileri</returns>
        /// <response code="200">Cihaz başarıyla güncellendi</response>
        /// <response code="404">Belirtilen ID'li cihaz bulunamadı</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpPut("{deviceId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RolandClientSettings>> UpdateDevice(string deviceId, [FromBody] RolandClientSettings settings)
        {
            try
            {
                // DeviceId'yi standart formata getir (MAC adresi formatını düzenle)
                deviceId = deviceId.Replace(":", "").Replace("-", "").ToUpper();
                
                var existingDevice = await _redisService.GetClientSettingsAsync(deviceId);
                
                if (existingDevice == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Cihaz ID'sini koruyalım
                settings.DeviceId = deviceId;
                
                await _redisService.SetClientSettingsAsync(settings);
                
                return Ok(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz güncellenirken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Cihaz güncellenirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Bir Roland cihazını siler
        /// </summary>
        /// <param name="deviceId">Silinecek cihazın ID'si</param>
        /// <returns>Başarı durumunda içerik dönmez (No Content)</returns>
        /// <response code="204">Cihaz başarıyla silindi</response>
        /// <response code="404">Belirtilen ID'li cihaz bulunamadı</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpDelete("{deviceId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteDevice(string deviceId)
        {
            try
            {
                // DeviceId'yi standart formata getir (MAC adresi formatını düzenle)
                deviceId = deviceId.Replace(":", "").Replace("-", "").ToUpper();
                
                var existingDevice = await _redisService.GetClientSettingsAsync(deviceId);
                
                if (existingDevice == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                await _redisService.DeleteClientSettingsAsync(deviceId);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cihaz silinirken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Cihaz silinirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Redis anahtarlarını listeler (diagnostik amaçlı)
        /// </summary>
        /// <returns>Redis'teki tüm anahtarların listesi</returns>
        [HttpGet("diagnostics/redis-keys")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<string>>> GetRedisKeys()
        {
            try
            {
                var keys = await _redisService.GetAllRedisKeysAsync();
                return Ok(keys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis anahtarları listelenirken hata oluştu");
                return StatusCode(500, new { message = "Redis anahtarları listelenirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Redis'teki bir anahtarın değerini ham şekilde döner (diagnostik amaçlı)
        /// </summary>
        /// <param name="key">Redis anahtarı</param>
        /// <returns>Anahtarın değeri</returns>
        [HttpGet("diagnostics/redis-value")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> GetRedisValue([FromQuery] string key)
        {
            try
            {
                var value = await _redisService.GetRedisValueAsync(key);
                
                if (value == null)
                {
                    return NotFound(new { message = $"{key} anahtarı bulunamadı" });
                }
                
                return Ok(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis değeri alınırken hata oluştu: {Key}", key);
                return StatusCode(500, new { message = "Redis değeri alınırken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Redis'te bir anahtarı siler (diagnostik amaçlı)
        /// </summary>
        /// <param name="key">Silinecek anahtar</param>
        /// <returns>Başarı durumunda içerik dönmez (No Content)</returns>
        [HttpDelete("diagnostics/redis-key")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteRedisKey([FromQuery] string key)
        {
            try
            {
                var deleted = await _redisService.DeleteRedisKeyAsync(key);
                
                if (!deleted)
                {
                    return NotFound(new { message = $"{key} anahtarı bulunamadı" });
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Redis anahtarı silinirken hata oluştu: {Key}", key);
                return StatusCode(500, new { message = "Redis anahtarı silinirken bir hata oluştu" });
            }
        }
    }
}