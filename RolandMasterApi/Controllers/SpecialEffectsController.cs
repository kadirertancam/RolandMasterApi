using Microsoft.AspNetCore.Mvc;
using RolandMasterApi.Models;
using RolandMasterApi.Services;

namespace RolandMasterApi.Controllers
{
    [ApiController]
    [Route("api/special-effects")]
    public class SpecialEffectsController : ControllerBase
    {
        private readonly RedisService _redisService;
        private readonly ILogger<SpecialEffectsController> _logger;

        public SpecialEffectsController(RedisService redisService, ILogger<SpecialEffectsController> logger)
        {
            _redisService = redisService;
            _logger = logger;
        }

        #region Effect 1

        /// <summary>
        /// Efekt 1'i çalıştırır
        /// </summary>
        /// <remarks>
        /// MIDI komutu: C0 01
        /// </remarks>
        [HttpPost("effect1/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TriggerEffect1(string deviceId)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Effect 1 komutu oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "effect1",
                    Parameters = new Dictionary<string, object>()
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Effect 1 komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Effect 1 komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Effect 1 tetiklenirken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Effect 1 tetiklenirken bir hata oluştu" });
            }
        }

        #endregion

        #region Effect 2

        /// <summary>
        /// Efekt 2'yi çalıştırır
        /// </summary>
        /// <remarks>
        /// MIDI komutu: C0 02
        /// </remarks>
        [HttpPost("effect2/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TriggerEffect2(string deviceId)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Effect 2 komutu oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "effect2",
                    Parameters = new Dictionary<string, object>()
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Effect 2 komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Effect 2 komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Effect 2 tetiklenirken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Effect 2 tetiklenirken bir hata oluştu" });
            }
        }

        #endregion

        #region Effect 3

        /// <summary>
        /// Efekt 3'ü çalıştırır
        /// </summary>
        /// <remarks>
        /// MIDI komutu: C0 03
        /// </remarks>
        [HttpPost("effect3/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TriggerEffect3(string deviceId)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Effect 3 komutu oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "effect3",
                    Parameters = new Dictionary<string, object>()
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Effect 3 komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Effect 3 komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Effect 3 tetiklenirken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Effect 3 tetiklenirken bir hata oluştu" });
            }
        }

        #endregion

        #region Effect 4

        /// <summary>
        /// Efekt 4'ü çalıştırır
        /// </summary>
        /// <remarks>
        /// MIDI komutu: C0 04
        /// </remarks>
        [HttpPost("effect4/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> TriggerEffect4(string deviceId)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }
                
                // Effect 4 komutu oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "effect4",
                    Parameters = new Dictionary<string, object>()
                };
                
                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);
                
                if (success)
                {
                    return Accepted(new { 
                        commandId = command.CommandId,
                        message = "Effect 4 komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Effect 4 komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Effect 4 tetiklenirken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Effect 4 tetiklenirken bir hata oluştu" });
            }
        }

        #endregion
    }
}