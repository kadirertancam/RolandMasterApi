using Microsoft.AspNetCore.Mvc;
using RolandMasterApi.Models;
using RolandMasterApi.Services;

namespace RolandMasterApi.Controllers
{
    /// <summary>
    /// Roland VT-4 cihazlarına komut göndermek için API
    /// </summary>
    [ApiController]
    [Route("api/commands")]
    public class CommandsController : ControllerBase
    {
        private readonly RedisService _redisService;
        private readonly ILogger<CommandsController> _logger;

        public CommandsController(RedisService redisService, ILogger<CommandsController> logger)
        {
            _redisService = redisService;
            _logger = logger;
        }

        /// <summary>
        /// Bir Roland cihazına komut gönderir
        /// </summary>
        /// <param name="command">Gönderilecek komut bilgileri</param>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/commands
        ///     {
        ///        "targetDeviceId": "device-001",
        ///        "effectType": "robot",
        ///        "parameters": {
        ///          "octave": 2,
        ///          "feedbackSwitch": 1,
        ///          "feedbackResonance": 120,
        ///          "feedbackLevel": 160
        ///        }
        ///     }
        /// 
        /// </remarks>
        /// <returns>Komut işleme sonucu</returns>
        /// <response code="202">Komut başarıyla işleme alındı</response>
        /// <response code="400">Geçersiz komut formatı</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RolandEffectCommand>> SendCommand([FromBody] RolandEffectCommand command)
        {
            if (!command.IsValid())
            {
                return BadRequest(new { message = "Geçersiz komut formatı" });
            }
            
            try
            {
                var success = await _redisService.SendCommandAsync(command);
                
                if (!success)
                {
                    return StatusCode(500, new { message = "Komut gönderilemedi" });
                }
                
                return Accepted(new { commandId = command.CommandId, message = "Komut işleme alındı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Komut gönderilirken hata oluştu: {CommandId}", command.CommandId);
                return StatusCode(500, new { message = "Komut gönderilirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Tüm Roland cihazlarına aynı komutu gönderir (broadcast)
        /// </summary>
        /// <param name="command">Gönderilecek komut bilgileri</param>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/commands/broadcast
        ///     {
        ///        "effectType": "reverb",
        ///        "parameters": {
        ///          "type": 0,
        ///          "param1": 100,
        ///          "param2": 150,
        ///          "param3": 200,
        ///          "param4": 180
        ///        }
        ///     }
        /// 
        /// </remarks>
        /// <returns>Broadcast komut işleme sonucu</returns>
        /// <response code="202">Broadcast komut başarıyla işleme alındı</response>
        /// <response code="400">Geçersiz komut formatı</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpPost("broadcast")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RolandEffectCommand>> BroadcastCommand([FromBody] RolandEffectCommand command)
        {
            // Broadcast komutu olarak işaretle
            command.IsBroadcast = true;
            command.TargetDeviceId = "broadcast";
            
            if (string.IsNullOrEmpty(command.EffectType) || command.EffectType == "none")
            {
                return BadRequest(new { message = "Geçersiz efekt tipi" });
            }
            
            try
            {
                var success = await _redisService.SendCommandAsync(command);
                
                if (!success)
                {
                    return StatusCode(500, new { message = "Broadcast komut gönderilemedi" });
                }
                
                return Accepted(new { commandId = command.CommandId, message = "Broadcast komut işleme alındı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Broadcast komut gönderilirken hata oluştu: {CommandId}", command.CommandId);
                return StatusCode(500, new { message = "Broadcast komut gönderilirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Bir komutun durumunu sorgular
        /// </summary>
        /// <param name="commandId">Sorgulanacak komutun ID'si</param>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     GET /api/commands/c8a6c3f0-5e2d-4f5a-94b7-b8b2e53c9b6a
        ///     
        /// Örnek yanıt:
        /// 
        ///     {
        ///       "commandId": "c8a6c3f0-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "targetDeviceId": "00155D4FA27F",
        ///       "effectType": "robot",
        ///       "parameters": {
        ///         "octave": 2,
        ///         "feedbackSwitch": 1,
        ///         "feedbackResonance": 120,
        ///         "feedbackLevel": 160
        ///       },
        ///       "createdAt": "2025-04-10T10:15:30.123Z",
        ///       "status": "completed",
        ///       "errorMessage": null,
        ///       "isBroadcast": false
        ///     }
        /// </remarks>
        /// <returns>Komut durum ve detayları</returns>
        /// <response code="200">Komut durumu başarıyla getirildi</response>
        /// <response code="404">Belirtilen ID'li komut bulunamadı</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpGet("{commandId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RolandEffectCommand>> GetCommandStatus(string commandId)
        {
            try
            {
                var command = await _redisService.GetCommandStatusAsync(commandId);
                
                if (command == null)
                {
                    return NotFound(new { message = $"{commandId} ID'li komut bulunamadı" });
                }
                
                return Ok(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Komut durumu alınırken hata oluştu: {CommandId}", commandId);
                return StatusCode(500, new { message = "Komut durumu alınırken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Bir Roland cihazına Robot efekti uygular
        /// </summary>
        /// <param name="deviceId">Hedef cihazın ID'si</param>
        /// <param name="parameters">Robot efekt parametreleri</param>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/commands/robot/device-001
        ///     {
        ///       "octave": 2,
        ///       "feedbackSwitch": 1,
        ///       "feedbackResonance": 120,
        ///       "feedbackLevel": 160
        ///     }
        /// 
        /// </remarks>
        /// <returns>Robot efekt komutu işleme sonucu</returns>
        /// <response code="202">Robot efekt komutu başarıyla işleme alındı</response>
        /// <response code="400">Geçersiz komut formatı</response>
        /// <response code="500">Sunucu hatası</response>
        [HttpPost("robot/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SendRobotCommand(string deviceId, [FromBody] System.Text.Json.JsonElement parameters)
        {
            try
            {
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "robot",
                    Parameters = new Dictionary<string, object>()
                };
                
                // JsonElement parametrelerini komuta ekle
                foreach (var property in parameters.EnumerateObject())
                {
                    switch (property.Value.ValueKind)
                    {
                        case System.Text.Json.JsonValueKind.Number:
                            if (property.Value.TryGetInt32(out int intValue))
                            {
                                command.Parameters.Add(property.Name, intValue);
                            }
                            else if (property.Value.TryGetDouble(out double doubleValue))
                            {
                                command.Parameters.Add(property.Name, doubleValue);
                            }
                            break;
                        case System.Text.Json.JsonValueKind.String:
                            command.Parameters.Add(property.Name, property.Value.GetString());
                            break;
                        default:
                            command.Parameters.Add(property.Name, property.Value.GetRawText());
                            break;
                    }
                }
                
                var success = await _redisService.SendCommandAsync(command);
                
                if (!success)
                {
                    return StatusCode(500, new { message = "Robot efekt komutu gönderilemedi" });
                }
                
                return Accepted(new { commandId = command.CommandId, message = "Robot efekt komutu işleme alındı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Robot efekt komutu gönderilirken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Robot efekt komutu gönderilirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Bir Roland cihazına Harmony efekti uygular
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/commands/harmony/00155D4FA27F
        ///     {
        ///       "h1Level": 200,
        ///       "h2Level": 150,
        ///       "h3Level": 100,
        ///       "h1Key": 0,
        ///       "h2Key": 4,
        ///       "h3Key": 7,
        ///       "h1Gender": 128,
        ///       "h2Gender": 128,
        ///       "h3Gender": 128
        ///     }
        ///     
        /// Örnek yanıt:
        /// 
        ///     {
        ///       "commandId": "d7e8f9a0-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Harmony efekt komutu işleme alındı"
        ///     }
        /// </remarks>
        [HttpPost("harmony/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SendHarmonyCommand(string deviceId, [FromBody] System.Text.Json.JsonElement parameters)
        {
            try
            {
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "harmony",
                    Parameters = new Dictionary<string, object>()
                };
                
                // JsonElement parametrelerini komuta ekle
                foreach (var property in parameters.EnumerateObject())
                {
                    switch (property.Value.ValueKind)
                    {
                        case System.Text.Json.JsonValueKind.Number:
                            if (property.Value.TryGetInt32(out int intValue))
                            {
                                command.Parameters.Add(property.Name, intValue);
                            }
                            else if (property.Value.TryGetDouble(out double doubleValue))
                            {
                                command.Parameters.Add(property.Name, doubleValue);
                            }
                            break;
                        case System.Text.Json.JsonValueKind.String:
                            command.Parameters.Add(property.Name, property.Value.GetString());
                            break;
                        default:
                            command.Parameters.Add(property.Name, property.Value.GetRawText());
                            break;
                    }
                }
                
                var success = await _redisService.SendCommandAsync(command);
                
                if (!success)
                {
                    return StatusCode(500, new { message = "Harmony efekt komutu gönderilemedi" });
                }
                
                return Accepted(new { commandId = command.CommandId, message = "Harmony efekt komutu işleme alındı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Harmony efekt komutu gönderilirken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Harmony efekt komutu gönderilirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Bir Roland cihazına Megaphone efekti uygular
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/commands/megaphone/00155D4FA27F
        ///     {
        ///       "type": 0,
        ///       "param1": 100,
        ///       "param2": 150,
        ///       "param3": 200,
        ///       "param4": 180
        ///     }
        ///     
        /// Örnek yanıt:
        /// 
        ///     {
        ///       "commandId": "a1b2c3d4-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Megaphone efekt komutu işleme alındı"
        ///     }
        /// </remarks>
        [HttpPost("megaphone/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SendMegaphoneCommand(string deviceId, [FromBody] System.Text.Json.JsonElement parameters)
        {
            try
            {
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "megaphone",
                    Parameters = new Dictionary<string, object>()
                };
                
                // JsonElement parametrelerini komuta ekle
                foreach (var property in parameters.EnumerateObject())
                {
                    switch (property.Value.ValueKind)
                    {
                        case System.Text.Json.JsonValueKind.Number:
                            if (property.Value.TryGetInt32(out int intValue))
                            {
                                command.Parameters.Add(property.Name, intValue);
                            }
                            else if (property.Value.TryGetDouble(out double doubleValue))
                            {
                                command.Parameters.Add(property.Name, doubleValue);
                            }
                            break;
                        case System.Text.Json.JsonValueKind.String:
                            command.Parameters.Add(property.Name, property.Value.GetString());
                            break;
                        default:
                            command.Parameters.Add(property.Name, property.Value.GetRawText());
                            break;
                    }
                }
                
                var success = await _redisService.SendCommandAsync(command);
                
                if (!success)
                {
                    return StatusCode(500, new { message = "Megaphone efekt komutu gönderilemedi" });
                }
                
                return Accepted(new { commandId = command.CommandId, message = "Megaphone efekt komutu işleme alındı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Megaphone efekt komutu gönderilirken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Megaphone efekt komutu gönderilirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Bir Roland cihazına Reverb efekti uygular
        /// </summary>
        [HttpPost("reverb/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SendReverbCommand(string deviceId, [FromBody] System.Text.Json.JsonElement parameters)
        {
            try
            {
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "reverb",
                    Parameters = new Dictionary<string, object>()
                };
                
                // JsonElement parametrelerini komuta ekle
                foreach (var property in parameters.EnumerateObject())
                {
                    switch (property.Value.ValueKind)
                    {
                        case System.Text.Json.JsonValueKind.Number:
                            if (property.Value.TryGetInt32(out int intValue))
                            {
                                command.Parameters.Add(property.Name, intValue);
                            }
                            else if (property.Value.TryGetDouble(out double doubleValue))
                            {
                                command.Parameters.Add(property.Name, doubleValue);
                            }
                            break;
                        case System.Text.Json.JsonValueKind.String:
                            command.Parameters.Add(property.Name, property.Value.GetString());
                            break;
                        default:
                            command.Parameters.Add(property.Name, property.Value.GetRawText());
                            break;
                    }
                }
                
                var success = await _redisService.SendCommandAsync(command);
                
                if (!success)
                {
                    return StatusCode(500, new { message = "Reverb efekt komutu gönderilemedi" });
                }
                
                return Accepted(new { commandId = command.CommandId, message = "Reverb efekt komutu işleme alındı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reverb efekt komutu gönderilirken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Reverb efekt komutu gönderilirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Bir Roland cihazına Vocoder efekti uygular
        /// </summary>
        [HttpPost("vocoder/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SendVocoderCommand(string deviceId, [FromBody] dynamic parameters)
        {
            try
            {
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "vocoder",
                    Parameters = new Dictionary<string, object>()
                };
                
                // Dinamik parametreleri komuta ekle
                foreach (var prop in parameters.GetType().GetProperties())
                {
                    command.Parameters.Add(prop.Name, prop.GetValue(parameters));
                }
                
                var success = await _redisService.SendCommandAsync(command);
                
                if (!success)
                {
                    return StatusCode(500, new { message = "Vocoder efekt komutu gönderilemedi" });
                }
                
                return Accepted(new { commandId = command.CommandId, message = "Vocoder efekt komutu işleme alındı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Vocoder efekt komutu gönderilirken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Vocoder efekt komutu gönderilirken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Bir Roland cihazına Equalizer efekti uygular
        /// </summary>
        [HttpPost("equalizer/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SendEqualizerCommand(string deviceId, [FromBody] System.Text.Json.JsonElement parameters)
        {
            try
            {
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "equalizer",
                    Parameters = new Dictionary<string, object>()
                };
                
                // JsonElement parametrelerini komuta ekle
                foreach (var property in parameters.EnumerateObject())
                {
                    switch (property.Value.ValueKind)
                    {
                        case System.Text.Json.JsonValueKind.Number:
                            if (property.Value.TryGetInt32(out int intValue))
                            {
                                command.Parameters.Add(property.Name, intValue);
                            }
                            else if (property.Value.TryGetDouble(out double doubleValue))
                            {
                                command.Parameters.Add(property.Name, doubleValue);
                            }
                            break;
                        case System.Text.Json.JsonValueKind.String:
                            command.Parameters.Add(property.Name, property.Value.GetString());
                            break;
                        default:
                            command.Parameters.Add(property.Name, property.Value.GetRawText());
                            break;
                    }
                }
                
                var success = await _redisService.SendCommandAsync(command);
                
                if (!success)
                {
                    return StatusCode(500, new { message = "Equalizer efekt komutu gönderilemedi" });
                }
                
                return Accepted(new { commandId = command.CommandId, message = "Equalizer efekt komutu işleme alındı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Equalizer efekt komutu gönderilirken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Equalizer efekt komutu gönderilirken bir hata oluştu" });
            }
        }
    }
}