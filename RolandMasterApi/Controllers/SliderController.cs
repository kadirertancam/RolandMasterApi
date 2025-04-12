using Microsoft.AspNetCore.Mvc;
using RolandMasterApi.Models;
using RolandMasterApi.Services;

namespace RolandMasterApi.Controllers
{
    [ApiController]
    [Route("api/sliders")]
    public class SliderController : ControllerBase
    {
        private readonly RedisService _redisService;
        private readonly ILogger<SliderController> _logger;

        public SliderController(RedisService redisService, ILogger<SliderController> logger)
        {
            _redisService = redisService;
            _logger = logger;
        }

        #region Key Control

        /// <summary>
        /// Bir cihazın Key (nota) ayarını değiştirir
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/sliders/key/00155D4FA27F
        ///     {
        ///       "value": 0
        ///     }
        ///     
        /// Value değerleri:
        /// - 0: C
        /// - 1: C#
        /// - 2: D
        /// - 3: D#
        /// - 4: E
        /// - 5: F
        /// - 6: F#
        /// - 7: G
        /// - 8: G#
        /// - 9: A
        /// - 10: A#
        /// - 11: B
        /// 
        /// MIDI komutu: B0 30 [value]
        /// 
        /// Örnek yanıt:
        /// 
        ///     {
        ///       "commandId": "a1b2c3d4-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Key ayar komutu gönderildi",
        ///       "statusCheckUrl": "/api/commands/a1b2c3d4-5e2d-4f5a-94b7-b8b2e53c9b6a"
        ///     }
        /// </remarks>
        [HttpPost("key/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyKeyControl(string deviceId, [FromBody] KeyParams parameters)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Key ayarı için komut oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "key",
                    Parameters = new Dictionary<string, object>
                    {
                        { "value", parameters.Value }
                    }
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Key ayar komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Key ayar komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Key ayarı uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Key ayarı uygulanırken bir hata oluştu" });
            }
        }

        #endregion
        
        #region Mic Sens Control

        /// <summary>
        /// Bir cihazın Mikrofon Hassasiyet ayarını değiştirir
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/sliders/micsens/00155D4FA27F
        ///     {
        ///       "value": 64
        ///     }
        ///     
        /// Value değeri 0-127 arasında olmalıdır
        /// 
        /// MIDI komutu: B0 2F [value]
        /// 
        /// Örnek yanıt:
        /// 
        ///     {
        ///       "commandId": "b2c3d4e5-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Mikrofon Hassasiyet ayar komutu gönderildi",
        ///       "statusCheckUrl": "/api/commands/b2c3d4e5-5e2d-4f5a-94b7-b8b2e53c9b6a"
        ///     }
        /// </remarks>
        [HttpPost("micsens/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyMicSensControl(string deviceId, [FromBody] MicSensParams parameters)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Mic Sens ayarı için komut oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "micsens",
                    Parameters = new Dictionary<string, object>
                    {
                        { "value", parameters.Value }
                    }
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Mikrofon Hassasiyet ayar komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Mikrofon Hassasiyet ayar komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mikrofon Hassasiyet ayarı uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Mikrofon Hassasiyet ayarı uygulanırken bir hata oluştu" });
            }
        }

        #endregion
        
        #region Volume Control

        /// <summary>
        /// Bir cihazın Ses Seviyesi ayarını değiştirir
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/sliders/volume/00155D4FA27F
        ///     {
        ///       "value": 100
        ///     }
        ///     
        /// Value değeri 0-127 arasında olmalıdır
        /// 
        /// MIDI komutu: B0 2E [value]
        /// 
        /// Örnek yanıt:
        /// 
        ///     {
        ///       "commandId": "c3d4e5f6-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Ses Seviyesi ayar komutu gönderildi",
        ///       "statusCheckUrl": "/api/commands/c3d4e5f6-5e2d-4f5a-94b7-b8b2e53c9b6a"
        ///     }
        /// </remarks>
        [HttpPost("volume/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyVolumeControl(string deviceId, [FromBody] VolumeParams parameters)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Volume ayarı için komut oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "volume",
                    Parameters = new Dictionary<string, object>
                    {
                        { "value", parameters.Value }
                    }
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Ses Seviyesi ayar komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Ses Seviyesi ayar komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ses Seviyesi ayarı uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Ses Seviyesi ayarı uygulanırken bir hata oluştu" });
            }
        }

        #endregion
        
        #region Reverb Control

        /// <summary>
        /// Bir cihazın Reverb (yankı) seviyesini değiştirir
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/sliders/reverb/00155D4FA27F
        ///     {
        ///       "value": 64
        ///     }
        ///     
        /// Value değeri 0-127 arasında olmalıdır
        /// 
        /// MIDI komutu: B0 39 [value]
        /// 
        /// Örnek yanıt:
        /// 
        ///     {
        ///       "commandId": "d4e5f6g7-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Reverb seviyesi ayar komutu gönderildi",
        ///       "statusCheckUrl": "/api/commands/d4e5f6g7-5e2d-4f5a-94b7-b8b2e53c9b6a"
        ///     }
        /// </remarks>
        [HttpPost("reverb/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyReverbValueControl(string deviceId, [FromBody] ReverbValueParams parameters)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Reverb seviyesi ayarı için komut oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "reverbvalue",
                    Parameters = new Dictionary<string, object>
                    {
                        { "value", parameters.Value }
                    }
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Reverb seviyesi ayar komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Reverb seviyesi ayar komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reverb seviyesi ayarı uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Reverb seviyesi ayarı uygulanırken bir hata oluştu" });
            }
        }

        #endregion
        
        #region Balance Control

        /// <summary>
        /// Bir cihazın Balance (denge) seviyesini değiştirir
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/sliders/balance/00155D4FA27F
        ///     {
        ///       "value": 64
        ///     }
        ///     
        /// Value değeri 0-127 arasında olmalıdır
        /// 
        /// MIDI komutu: B0 38 [value]
        /// 
        /// Örnek yanıt:
        /// 
        ///     {
        ///       "commandId": "e5f6g7h8-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Balance seviyesi ayar komutu gönderildi",
        ///       "statusCheckUrl": "/api/commands/e5f6g7h8-5e2d-4f5a-94b7-b8b2e53c9b6a"
        ///     }
        /// </remarks>
        [HttpPost("balance/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyBalanceControl(string deviceId, [FromBody] BalanceParams parameters)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Balance seviyesi ayarı için komut oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "balance",
                    Parameters = new Dictionary<string, object>
                    {
                        { "value", parameters.Value }
                    }
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Balance seviyesi ayar komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Balance seviyesi ayar komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Balance seviyesi ayarı uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Balance seviyesi ayarı uygulanırken bir hata oluştu" });
            }
        }

        #endregion
        
        #region Formant Control

        /// <summary>
        /// Bir cihazın Formant seviyesini değiştirir
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/sliders/formant/00155D4FA27F
        ///     {
        ///       "value": 64
        ///     }
        ///     
        /// Value değeri 0-127 arasında olmalıdır
        /// 
        /// MIDI komutu: B0 36 [value]
        /// 
        /// Örnek yanıt:
        /// 
        ///     {
        ///       "commandId": "f6g7h8i9-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Formant seviyesi ayar komutu gönderildi",
        ///       "statusCheckUrl": "/api/commands/f6g7h8i9-5e2d-4f5a-94b7-b8b2e53c9b6a"
        ///     }
        /// </remarks>
        [HttpPost("formant/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyFormantControl(string deviceId, [FromBody] FormantParams parameters)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Formant seviyesi ayarı için komut oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "formant",
                    Parameters = new Dictionary<string, object>
                    {
                        { "value", parameters.Value }
                    }
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Formant seviyesi ayar komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Formant seviyesi ayar komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Formant seviyesi ayarı uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Formant seviyesi ayarı uygulanırken bir hata oluştu" });
            }
        }

        #endregion
        
        #region Pitch Control

        /// <summary>
        /// Bir cihazın Pitch (ses yüksekliği) seviyesini değiştirir
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/sliders/pitch/00155D4FA27F
        ///     {
        ///       "value": 8192
        ///     }
        ///     
        /// Value değeri 0-16383 arasında olmalıdır (8192 merkez değerdir)
        /// 
        /// MIDI komutu: E0 [lsb] [msb] - Pitch değeri 14-bit formatta gönderilir
        /// 
        /// Örnek yanıt:
        /// 
        ///     {
        ///       "commandId": "g7h8i9j0-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Pitch seviyesi ayar komutu gönderildi",
        ///       "statusCheckUrl": "/api/commands/g7h8i9j0-5e2d-4f5a-94b7-b8b2e53c9b6a"
        ///     }
        /// </remarks>
        [HttpPost("pitch/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyPitchControl(string deviceId, [FromBody] PitchParams parameters)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Pitch seviyesi ayarı için komut oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "pitch",
                    Parameters = new Dictionary<string, object>
                    {
                        { "value", parameters.Value }
                    }
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Pitch seviyesi ayar komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Pitch seviyesi ayar komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pitch seviyesi ayarı uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Pitch seviyesi ayarı uygulanırken bir hata oluştu" });
            }
        }

        #endregion
    }

    #region Parameter Classes 

    /// <summary>
    /// Key (nota) parametreleri
    /// </summary>
    public class KeyParams
    {
        /// <summary>
        /// 0-11 arasında değer (C=0, C#=1, D=2, D#=3, E=4, F=5, F#=6, G=7, G#=8, A=9, A#=10, B=11)
        /// </summary>
        public byte Value { get; set; } = 0;
    }
    
    /// <summary>
    /// Mikrofon hassasiyet parametreleri
    /// </summary>
    public class MicSensParams
    {
        /// <summary>
        /// 0-127 arasında değer
        /// </summary>
        public byte Value { get; set; } = 64;
    }
    
    /// <summary>
    /// Ses seviyesi parametreleri
    /// </summary>
    public class VolumeParams
    {
        /// <summary>
        /// 0-127 arasında değer
        /// </summary>
        public byte Value { get; set; } = 100;
    }
    
    /// <summary>
    /// Reverb (yankı) seviyesi parametreleri
    /// </summary>
    public class ReverbValueParams
    {
        /// <summary>
        /// 0-127 arasında değer
        /// </summary>
        public byte Value { get; set; } = 64;
    }
    
    /// <summary>
    /// Balance (denge) parametreleri
    /// </summary>
    public class BalanceParams
    {
        /// <summary>
        /// 0-127 arasında değer
        /// </summary>
        public byte Value { get; set; } = 64;
    }
    
    /// <summary>
    /// Formant parametreleri
    /// </summary>
    public class FormantParams
    {
        /// <summary>
        /// 0-127 arasında değer
        /// </summary>
        public byte Value { get; set; } = 64;
    }
    
    /// <summary>
    /// Pitch (ses yüksekliği) parametreleri
    /// </summary>
    public class PitchParams
    {
        /// <summary>
        /// 0-16383 arasında değer (merkez 8192)
        /// </summary>
        public int Value { get; set; } = 8192;
    }

    #endregion
}