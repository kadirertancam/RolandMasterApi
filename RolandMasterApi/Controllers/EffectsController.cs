using Microsoft.AspNetCore.Mvc;
using RolandMasterApi.Models;
using RolandMasterApi.Services;

namespace RolandMasterApi.Controllers
{
    [ApiController]
    [Route("api/effects")]
    public class EffectsController : ControllerBase
    {
        private readonly RedisService _redisService;
        private readonly ILogger<EffectsController> _logger;

        public EffectsController(RedisService redisService, ILogger<EffectsController> logger)
        {
            _redisService = redisService;
            _logger = logger;
        }

        #region Robot Effect

        /// <summary>
        /// Bir cihaza Robot efektini uygular
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/effects/robot/00155D4FA27F
        ///     {
        ///       "octave": 2,
        ///       "feedbackSwitch": 1,
        ///       "feedbackResonance": 120,
        ///       "feedbackLevel": 160
        ///     }
        ///     
        /// Örnek yanıt:
        /// 
        ///     {
        ///       "commandId": "c8a6c3f0-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Robot efekti komutu gönderildi",
        ///       "statusCheckUrl": "/api/commands/c8a6c3f0-5e2d-4f5a-94b7-b8b2e53c9b6a"
        ///     }
        /// </remarks>
        [HttpPost("robot/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyRobotEffect(string deviceId, [FromBody] RobotParams parameters)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }

                // Robot efekti için komut oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "robot",
                    Parameters = new Dictionary<string, object>
                    {
                        { "octave", parameters.Octave },
                        { "feedbackSwitch", parameters.FeedbackSwitch },
                        { "feedbackResonance", parameters.FeedbackResonance },
                        { "feedbackLevel", parameters.FeedbackLevel }
                    }
                };

                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);

                if (success)
                {
                    return Accepted(new
                    {
                        commandId = command.CommandId,
                        message = "Robot efekti komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekt komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Robot efekti uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Robot efekti uygulanırken bir hata oluştu" });
            }
        }

        #endregion

        #region Harmony Effect

        /// <summary>
        /// Bir cihaza Harmony efektini uygular
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/effects/harmony/00155D4FA27F
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
        ///       "commandId": "d8a6c3f0-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Harmony efekti komutu gönderildi",
        ///       "statusCheckUrl": "/api/commands/d8a6c3f0-5e2d-4f5a-94b7-b8b2e53c9b6a"
        ///     }
        /// </remarks>
        [HttpPost("harmony/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyHarmonyEffect(string deviceId, [FromBody] HarmonyParams parameters)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }

                // Harmony efekti için komut oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "harmony",
                    Parameters = new Dictionary<string, object>
                    {
                        { "h1Level", parameters.H1Level },
                        { "h2Level", parameters.H2Level },
                        { "h3Level", parameters.H3Level },
                        { "h1Key", parameters.H1Key },
                        { "h2Key", parameters.H2Key },
                        { "h3Key", parameters.H3Key },
                        { "h1Gender", parameters.H1Gender },
                        { "h2Gender", parameters.H2Gender },
                        { "h3Gender", parameters.H3Gender }
                    }
                };

                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);

                if (success)
                {
                    return Accepted(new
                    {
                        commandId = command.CommandId,
                        message = "Harmony efekti komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekt komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Harmony efekti uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Harmony efekti uygulanırken bir hata oluştu" });
            }
        }

        #endregion

        #region Megaphone Effect

        /// <summary>
        /// Bir cihaza Megaphone efektini uygular
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/effects/megaphone/00155D4FA27F
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
        ///       "message": "Megaphone efekti komutu gönderildi",
        ///       "statusCheckUrl": "/api/commands/a1b2c3d4-5e2d-4f5a-94b7-b8b2e53c9b6a"
        ///     }
        /// </remarks>
        [HttpPost("megaphone/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyMegaphoneEffect(string deviceId, [FromBody] MegaphoneParams parameters)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }

                // Megaphone efekti için komut oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "megaphone",
                    Parameters = new Dictionary<string, object>
                    {
                        { "type", parameters.Type },
                        { "param1", parameters.Param1 },
                        { "param2", parameters.Param2 },
                        { "param3", parameters.Param3 },
                        { "param4", parameters.Param4 }
                    }
                };

                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);

                if (success)
                {
                    return Accepted(new
                    {
                        commandId = command.CommandId,
                        message = "Megaphone efekti komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekt komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Megaphone efekti uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Megaphone efekti uygulanırken bir hata oluştu" });
            }
        }

        #endregion

        #region Reverb Effect

        /// <summary>
        /// Bir cihaza Reverb efektini uygular
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/effects/reverb/00155D4FA27F
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
        ///       "commandId": "e5f6g7h8-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Reverb efekti komutu gönderildi",
        ///       "statusCheckUrl": "/api/commands/e5f6g7h8-5e2d-4f5a-94b7-b8b2e53c9b6a"
        ///     }
        /// </remarks>
        [HttpPost("reverb/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyReverbEffect(string deviceId, [FromBody] ReverbParams parameters)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }

                // Reverb efekti için komut oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "reverb",
                    Parameters = new Dictionary<string, object>
                    {
                        { "type", parameters.Type },
                        { "param1", parameters.Param1 },
                        { "param2", parameters.Param2 },
                        { "param3", parameters.Param3 },
                        { "param4", parameters.Param4 }
                    }
                };

                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);

                if (success)
                {
                    return Accepted(new
                    {
                        commandId = command.CommandId,
                        message = "Reverb efekti komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekt komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reverb efekti uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Reverb efekti uygulanırken bir hata oluştu" });
            }
        }

        #endregion

        #region Vocoder Effect

        /// <summary>
        /// Bir cihaza Vocoder efektini uygular
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/effects/vocoder/00155D4FA27F
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
        ///       "commandId": "j9k1l2m3-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Vocoder efekti komutu gönderildi",
        ///       "statusCheckUrl": "/api/commands/j9k1l2m3-5e2d-4f5a-94b7-b8b2e53c9b6a"
        ///     }
        /// </remarks>
        [HttpPost("vocoder/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyVocoderEffect(string deviceId, [FromBody] VocoderParams parameters)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }

                // Vocoder efekti için komut oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "vocoder",
                    Parameters = new Dictionary<string, object>
                    {
                        { "type", parameters.Type },
                        { "param1", parameters.Param1 },
                        { "param2", parameters.Param2 },
                        { "param3", parameters.Param3 },
                        { "param4", parameters.Param4 }
                    }
                };

                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);

                if (success)
                {
                    return Accepted(new
                    {
                        commandId = command.CommandId,
                        message = "Vocoder efekti komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekt komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Vocoder efekti uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Vocoder efekti uygulanırken bir hata oluştu" });
            }
        }

        #endregion

        #region Equalizer Effect

        /// <summary>
        /// Bir cihaza Equalizer efektini uygular
        /// </summary>
        /// <remarks>
        /// Örnek istek:
        /// 
        ///     POST /api/effects/equalizer/00155D4FA27F
        ///     {
        ///       "eqSwitch": 1,
        ///       "lowShelfFreq": 40,
        ///       "lowShelfGain": 20,
        ///       "lowMidFreq": 60,
        ///       "lowMidQ": 70,
        ///       "lowMidGain": 15,
        ///       "highMidFreq": 80,
        ///       "highMidQ": 70,
        ///       "highMidGain": 10,
        ///       "highShelfFreq": 100,
        ///       "highShelfGain": 20
        ///     }
        ///     
        /// Örnek yanıt:
        /// 
        ///     {
        ///       "commandId": "n4p5q6r7-5e2d-4f5a-94b7-b8b2e53c9b6a",
        ///       "message": "Equalizer efekti komutu gönderildi",
        ///       "statusCheckUrl": "/api/commands/n4p5q6r7-5e2d-4f5a-94b7-b8b2e53c9b6a"
        ///     }
        /// </remarks>
        [HttpPost("equalizer/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyEqualizerEffect(string deviceId, [FromBody] EqualizerParams parameters)
        {
            try
            {
                // Önce cihazın var olduğunu kontrol edelim
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }

                // Equalizer efekti için komut oluştur
                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "equalizer",
                    Parameters = new Dictionary<string, object>
                    {
                        { "eqSwitch", parameters.EqSwitch },
                        { "lowShelfFreq", parameters.LowShelfFreq },
                        { "lowShelfGain", parameters.LowShelfGain },
                        { "lowMidFreq", parameters.LowMidFreq },
                        { "lowMidQ", parameters.LowMidQ },
                        { "lowMidGain", parameters.LowMidGain },
                        { "highMidFreq", parameters.HighMidFreq },
                        { "highMidQ", parameters.HighMidQ },
                        { "highMidGain", parameters.HighMidGain },
                        { "highShelfFreq", parameters.HighShelfFreq },
                        { "highShelfGain", parameters.HighShelfGain }
                    }
                };

                // Komutu gönder
                bool success = await _redisService.SendCommandAsync(command);

                if (success)
                {
                    return Accepted(new
                    {
                        commandId = command.CommandId,
                        message = "Equalizer efekti komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekt komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Equalizer efekti uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Equalizer efekti uygulanırken bir hata oluştu" });
            }
        }

        #endregion

        #region Yeni Slider Efektleri

        #region Key (Slider) Effect
        [HttpPost("key/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyKeyEffect(string deviceId, [FromBody] KeyParams parameters)
        {
            try
            {
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }

                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "key",
                    Parameters = new Dictionary<string, object>
                    {
                        { "key", parameters.Key }
                    }
                };

                bool success = await _redisService.SendCommandAsync(command);
                if (success)
                {
                    return Accepted(new
                    {
                        commandId = command.CommandId,
                        message = "Key efekti komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekt komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Key efekti uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Key efekti uygulanırken bir hata oluştu" });
            }
        }
        #endregion

        #region Mic Sens (Slider) Effect
        [HttpPost("micsens/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyMicSensitivityEffect(string deviceId, [FromBody] MicSensParams parameters)
        {
            try
            {
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }

                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "micsens",
                    Parameters = new Dictionary<string, object>
                    {
                        { "micSensitivity", parameters.MicSensitivity }
                    }
                };

                bool success = await _redisService.SendCommandAsync(command);
                if (success)
                {
                    return Accepted(new
                    {
                        commandId = command.CommandId,
                        message = "Mikrofon hassasiyeti komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekt komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mikrofon hassasiyeti efekti uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Mikrofon hassasiyeti efekti uygulanırken bir hata oluştu" });
            }
        }
        #endregion

        #region Volume (Slider) Effect
        [HttpPost("volume/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyVolumeEffect(string deviceId, [FromBody] VolumeParams parameters)
        {
            try
            {
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }

                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "volume",
                    Parameters = new Dictionary<string, object>
                    {
                        { "volume", parameters.Volume }
                    }
                };

                bool success = await _redisService.SendCommandAsync(command);
                if (success)
                {
                    return Accepted(new
                    {
                        commandId = command.CommandId,
                        message = "Volume efekti komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekt komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Volume efekti uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Volume efekti uygulanırken bir hata oluştu" });
            }
        }
        #endregion

        #region Balance (Slider) Effect
        [HttpPost("balance/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyBalanceEffect(string deviceId, [FromBody] BalanceParams parameters)
        {
            try
            {
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }

                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "balance",
                    Parameters = new Dictionary<string, object>
                    {
                        { "balance", parameters.Balance }
                    }
                };

                bool success = await _redisService.SendCommandAsync(command);
                if (success)
                {
                    return Accepted(new
                    {
                        commandId = command.CommandId,
                        message = "Balance efekti komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekt komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Balance efekti uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Balance efekti uygulanırken bir hata oluştu" });
            }
        }
        #endregion

        #region Formant (Slider) Effect
        [HttpPost("formant/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyFormantEffect(string deviceId, [FromBody] FormantParams parameters)
        {
            try
            {
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }

                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "formant",
                    Parameters = new Dictionary<string, object>
                    {
                        { "formant", parameters.Formant }
                    }
                };

                bool success = await _redisService.SendCommandAsync(command);
                if (success)
                {
                    return Accepted(new
                    {
                        commandId = command.CommandId,
                        message = "Formant efekti komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekt komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Formant efekti uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Formant efekti uygulanırken bir hata oluştu" });
            }
        }
        #endregion

        #region Pitch (Slider) Effect
        [HttpPost("pitch/{deviceId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ApplyPitchEffect(string deviceId, [FromBody] PitchParams parameters)
        {
            try
            {
                var device = await _redisService.GetClientSettingsAsync(deviceId);
                if (device == null)
                {
                    return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
                }

                var command = new RolandEffectCommand
                {
                    TargetDeviceId = deviceId,
                    EffectType = "pitch",
                    Parameters = new Dictionary<string, object>
                    {
                        { "pitch", parameters.Pitch }
                    }
                };

                bool success = await _redisService.SendCommandAsync(command);
                if (success)
                {
                    return Accepted(new
                    {
                        commandId = command.CommandId,
                        message = "Pitch efekti komutu gönderildi",
                        statusCheckUrl = $"/api/commands/{command.CommandId}"
                    });
                }
                else
                {
                    return StatusCode(500, new { message = "Efekt komutu gönderilirken bir hata oluştu" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pitch efekti uygulanırken hata oluştu: {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Pitch efekti uygulanırken bir hata oluştu" });
            }
        }
        #endregion

        #endregion
    }

    #region Effect Parameter Classes

    public class RobotParams
    {
        public byte Octave { get; set; } = 2;
        public byte FeedbackSwitch { get; set; } = 0;
        public byte FeedbackResonance { get; set; } = 120;
        public byte FeedbackLevel { get; set; } = 160;
    }

    public class HarmonyParams
    {
        public byte H1Level { get; set; } = 200;
        public byte H2Level { get; set; } = 150;
        public byte H3Level { get; set; } = 100;
        public byte H1Key { get; set; } = 0;
        public byte H2Key { get; set; } = 4;
        public byte H3Key { get; set; } = 7;
        public byte H1Gender { get; set; } = 128;
        public byte H2Gender { get; set; } = 128;
        public byte H3Gender { get; set; } = 128;
    }

    public class MegaphoneParams
    {
        public byte Type { get; set; } = 0;
        public byte Param1 { get; set; } = 100;
        public byte Param2 { get; set; } = 150;
        public byte Param3 { get; set; } = 200;
        public byte Param4 { get; set; } = 180;
    }

    public class ReverbParams
    {
        public byte Type { get; set; } = 0;
        public byte Param1 { get; set; } = 100;
        public byte Param2 { get; set; } = 150;
        public byte Param3 { get; set; } = 200;
        public byte Param4 { get; set; } = 180;
    }

    public class VocoderParams
    {
        public byte Type { get; set; } = 0;
        public byte Param1 { get; set; } = 100;
        public byte Param2 { get; set; } = 150;
        public byte Param3 { get; set; } = 200;
        public byte Param4 { get; set; } = 180;
    }

    public class EqualizerParams
    {
        public byte EqSwitch { get; set; } = 1;
        public byte LowShelfFreq { get; set; } = 40;
        public byte LowShelfGain { get; set; } = 20;
        public byte LowMidFreq { get; set; } = 60;
        public byte LowMidQ { get; set; } = 70;
        public byte LowMidGain { get; set; } = 15;
        public byte HighMidFreq { get; set; } = 80;
        public byte HighMidQ { get; set; } = 70;
        public byte HighMidGain { get; set; } = 10;
        public byte HighShelfFreq { get; set; } = 100;
        public byte HighShelfGain { get; set; } = 20;
    }

    // Yeni eklenen parametre sınıfları:
    public class KeyParams
    {
        public string Key { get; set; }
    }

    public class MicSensParams
    {
        public byte MicSensitivity { get; set; }
    }

    public class VolumeParams
    {
        public byte Volume { get; set; }
    }

    public class BalanceParams
    {
        public byte Balance { get; set; }
    }

    public class FormantParams
    {
        public byte Formant { get; set; }
    }

    public class PitchParams
    {
        public byte Pitch { get; set; }
    }

    #endregion
}
