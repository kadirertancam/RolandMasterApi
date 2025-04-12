    #region Effect Off Endpoints

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

    /// <summary>
    /// Bütün efektleri kapatır
    /// </summary>
    [HttpPost("{deviceId}/all-off")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> TurnOffAllEffects(string deviceId)
    {
        try
        {
            // Önce cihazın var olduğunu kontrol edelim
            var device = await _redisService.GetClientSettingsAsync(deviceId);
            if (device == null)
            {
                return NotFound(new { message = $"{deviceId} ID'li cihaz bulunamadı" });
            }
            
            // Tüm efektleri kapatma komutu (none efekti)
            var command = new RolandEffectCommand
            {
                TargetDeviceId = deviceId,
                EffectType = "none",
                Parameters = new Dictionary<string, object>()
            };
            
            // Komutu gönder
            bool success = await _redisService.SendCommandAsync(command);
            
            if (success)
            {
                return Accepted(new { 
                    commandId = command.CommandId,
                    message = "Tüm efektleri kapatma komutu gönderildi",
                    statusCheckUrl = $"/api/commands/{command.CommandId}"
                });
            }
            else
            {
                return StatusCode(500, new { message = "Efektleri kapatma komutu gönderilirken bir hata oluştu" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tüm efektleri kapatırken hata oluştu: {DeviceId}", deviceId);
            return StatusCode(500, new { message = "Tüm efektleri kapatırken bir hata oluştu" });
        }
    }

    #endregion