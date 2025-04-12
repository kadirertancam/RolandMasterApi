using Microsoft.AspNetCore.Mvc;
using RolandMasterApi.Models;
using RolandMasterApi.Services;

namespace RolandMasterApi.Controllers
{
    [ApiController]
    [Route("api/effect-toggles")]
    public class EffectToggleController : ControllerBase
    {
        private readonly RedisService _redisService;
        private readonly ILogger<EffectToggleController> _logger;

        public EffectToggleController(RedisService redisService, ILogger<EffectToggleController> logger)
        {
            _redisService = redisService;
            _logger = logger;
        }

        #region Robot Effect Toggle

        /// <summary>
        /// Robot efektini açar
        /// </summary>
        /// <remarks>
        /// MIDI komutu: B0 31 7F
        /// </remarks>
        [HttpPost("robot/{deviceId}/on")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TurnOnRobotEffect(string deviceId)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Robot efektini açma komutu oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "roboton",
                    Parameters = new Dictionary<string, object>()
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Robot efektini açma komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekti açma komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Robot efektini açarken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Robot efektini açarken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Robot efektini kapatır
        /// </summary>
        /// <remarks>
        /// MIDI komutu: B0 31 00
        /// </remarks>
        [HttpPost("robot/{deviceId}/off")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TurnOffRobotEffect(string deviceId)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Robot efektini kapatma komutu oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "robotoff",
                    Parameters = new Dictionary<string, object>()
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Robot efektini kapatma komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekti kapatma komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Robot efektini kapatırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Robot efektini kapatırken bir hata oluştu" });
            }
        }

        #endregion

        #region Megaphone Effect Toggle

        /// <summary>
        /// Megaphone efektini açar
        /// </summary>
        /// <remarks>
        /// MIDI komutu: B0 32 7F
        /// </remarks>
        [HttpPost("megaphone/{deviceId}/on")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TurnOnMegaphoneEffect(string deviceId)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Megaphone efektini açma komutu oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "megaphoneon",
                    Parameters = new Dictionary<string, object>()
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Megaphone efektini açma komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekti açma komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Megaphone efektini açarken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Megaphone efektini açarken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Megaphone efektini kapatır
        /// </summary>
        /// <remarks>
        /// MIDI komutu: B0 32 00
        /// </remarks>
        [HttpPost("megaphone/{deviceId}/off")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TurnOffMegaphoneEffect(string deviceId)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Megaphone efektini kapatma komutu oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "megaphoneoff",
                    Parameters = new Dictionary<string, object>()
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Megaphone efektini kapatma komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekti kapatma komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Megaphone efektini kapatırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Megaphone efektini kapatırken bir hata oluştu" });
            }
        }

        #endregion

        #region Vocoder Effect Toggle

        /// <summary>
        /// Vocoder efektini açar
        /// </summary>
        /// <remarks>
        /// MIDI komutu: B0 34 7F
        /// </remarks>
        [HttpPost("vocoder/{deviceId}/on")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TurnOnVocoderEffect(string deviceId)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Vocoder efektini açma komutu oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "vocoderon",
                    Parameters = new Dictionary<string, object>()
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Vocoder efektini açma komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekti açma komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Vocoder efektini açarken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Vocoder efektini açarken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Vocoder efektini kapatır
        /// </summary>
        /// <remarks>
        /// MIDI komutu: B0 34 00
        /// </remarks>
        [HttpPost("vocoder/{deviceId}/off")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TurnOffVocoderEffect(string deviceId)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Vocoder efektini kapatma komutu oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "vocoderoff",
                    Parameters = new Dictionary<string, object>()
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Vocoder efektini kapatma komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekti kapatma komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Vocoder efektini kapatırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Vocoder efektini kapatırken bir hata oluştu" });
            }
        }

        #endregion

        #region Harmony Effect Toggle

        /// <summary>
        /// Harmony efektini açar
        /// </summary>
        /// <remarks>
        /// MIDI komutu: B0 35 7F
        /// </remarks>
        [HttpPost("harmony/{deviceId}/on")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TurnOnHarmonyEffect(string deviceId)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Harmony efektini açma komutu oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "harmonyon",
                    Parameters = new Dictionary<string, object>()
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Harmony efektini açma komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekti açma komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Harmony efektini açarken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Harmony efektini açarken bir hata oluştu" });
            }
        }

        /// <summary>
        /// Harmony efektini kapatır
        /// </summary>
        /// <remarks>
        /// MIDI komutu: B0 35 00
        /// </remarks>
        [HttpPost("harmony/{deviceId}/off")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TurnOffHarmonyEffect(string deviceId)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Harmony efektini kapatma komutu oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "harmonyoff",
                    Parameters = new Dictionary<string, object>()
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Harmony efektini kapatma komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekti kapatma komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Harmony efektini kapatırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Harmony efektini kapatırken bir hata oluştu" });
            }
        }

        #endregion
    }
}