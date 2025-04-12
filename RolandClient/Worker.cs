using RolandClient.Models;
using RolandClient.Services;

namespace RolandClient
{
    /// <summary>
    /// Ana worker servisi - arka planda çalışarak API ile iletişime geçer
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ClientSettings _clientSettings;
        private readonly MidiService _midiService;
        private readonly RedisService _redisService;
        private readonly CommandProcessor _commandProcessor;

        public Worker(
            ILogger<Worker> logger, 
            ClientSettings clientSettings,
            MidiService midiService,
            RedisService redisService,
            CommandProcessor commandProcessor)
        {
            _logger = logger;
            _clientSettings = clientSettings;
            _midiService = midiService;
            _redisService = redisService;
            _commandProcessor = commandProcessor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Roland VT-4 Client servisi başlatılıyor...");

                // MIDI cihazını başlat
                if (_midiService.IsInitialized())
                {
                    _logger.LogInformation("MIDI cihazı zaten başlatılmış");
                }
                else
                {
                    try
                    {
                        _midiService.Initialize(_clientSettings.MidiDeviceId);
                        _logger.LogInformation("MIDI cihazı başlatıldı: {MidiDeviceId}", _clientSettings.MidiDeviceId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "MIDI cihazı başlatılırken hata oluştu: {MidiDeviceId}", _clientSettings.MidiDeviceId);
                    }
                }

                // MAC adresini DeviceId olarak kullan (format düzenlenmiş halde)
                string formattedMacAddress = _clientSettings.MacAddress.Replace(":", "").Replace("-", "").ToUpper();
                
                _logger.LogInformation("Using formatted MAC address for DeviceId: {MacAddress}", formattedMacAddress);
                _logger.LogInformation("Original MAC address was: {OriginalMac}", _clientSettings.MacAddress);
                
                var settings = new RolandClientSettings
                {
                    DeviceId = formattedMacAddress, // Format düzenlenmiş MAC adresini deviceId olarak kullan
                    DeviceName = _clientSettings.DeviceName,
                    MacAddress = formattedMacAddress, // Hem DeviceId hem MacAddress için aynı formatı kullan
                    MidiDeviceId = _clientSettings.MidiDeviceId,
                    IsActive = true,
                    ActiveEffect = "none"
                };
                
                await _redisService.RegisterDeviceAsync(settings);
                _logger.LogInformation("Cihaz bilgileri Redis'e kaydedildi: DeviceId={DeviceId}, MAC={MacAddress}", settings.DeviceId, settings.MacAddress);

                // Komutu kanalını dinlemeye başla
                _redisService.SubscribeToCommands(async command => 
                {
                    await _commandProcessor.ProcessCommandAsync(command);
                });
                
                _logger.LogInformation("Komut kanalı dinleniyor. Çalışmaya hazır.");
                
                // Redis'teki ayarları periyodik olarak kontrol etme işlemi
                _ = Task.Run(async () =>
                {
                    _logger.LogInformation("Redis'teki ayarlar periyodik olarak kontrol edilecek");
                    
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            // Her 5 saniyede bir Redis'teki ayarları kontrol et
                            await Task.Delay(5000, stoppingToken);
                            
                            string formattedMacAddress = _clientSettings.MacAddress.Replace(":", "").Replace("-", "").ToUpper();
                            
                            // Redis üzerinden güncel ayarları al
                            var redisSettings = await _redisService.GetDeviceSettingsAsync(formattedMacAddress);
                            
                            if (redisSettings != null)
                            {
                                _logger.LogInformation("Redis'ten ayarlar alındı: DeviceId={DeviceId}, ActiveEffect={ActiveEffect}",
                                    redisSettings.DeviceId, redisSettings.ActiveEffect);
                                    
                                // Ayarlarda değişiklik varsa uygula
                                if (redisSettings.ActiveEffect != "none" && 
                                    (!string.Equals(redisSettings.ActiveEffect, _commandProcessor.GetCurrentSettings().ActiveEffect, StringComparison.OrdinalIgnoreCase)))
                                {
                                    _logger.LogInformation("Redis'te değişiklik tespit edildi. Aktif efekt değişiyor: {OldEffect} -> {NewEffect}",
                                        _commandProcessor.GetCurrentSettings().ActiveEffect, redisSettings.ActiveEffect);
                                        
                                    // Efekt değişimi için komut oluştur ve işle
                                    var effectCommand = new RolandEffectCommand
                                    {
                                        CommandId = Guid.NewGuid().ToString(),
                                        TargetDeviceId = formattedMacAddress,
                                        EffectType = redisSettings.ActiveEffect,
                                        Parameters = BuildParametersFromSettings(redisSettings)
                                    };
                                    
                                    await _commandProcessor.ProcessCommandAsync(effectCommand);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Redis'teki ayarlar kontrol edilirken hata oluştu");
                        }
                    }
                }, stoppingToken);
                
                // Uygulama kapatılana kadar bekle
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(5000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Worker servisi çalışırken beklenmeyen bir hata oluştu");
            }
            finally
            {
                _logger.LogInformation("Roland VT-4 Client servisi kapatılıyor...");
                _midiService.Dispose();
            }
        }
        
        /// <summary>
        /// RolandClientSettings'ten efekt parametrelerini içeren Dictionary oluşturur
        /// </summary>
        private Dictionary<string, object> BuildParametersFromSettings(RolandClientSettings settings)
        {
            var parameters = new Dictionary<string, object>();
            
            // Aktif efekte göre parametreleri ekle
            switch (settings.ActiveEffect.ToLower())
            {
                case "robot":
                    parameters.Add("octave", settings.RobotOctave);
                    parameters.Add("feedbackSwitch", settings.RobotFeedbackSwitch);
                    parameters.Add("feedbackResonance", settings.RobotFeedbackResonance);
                    parameters.Add("feedbackLevel", settings.RobotFeedbackLevel);
                    break;
                    
                case "harmony":
                    parameters.Add("h1Level", settings.HarmonyH1Level);
                    parameters.Add("h2Level", settings.HarmonyH2Level);
                    parameters.Add("h3Level", settings.HarmonyH3Level);
                    parameters.Add("h1Key", settings.HarmonyH1Key);
                    parameters.Add("h2Key", settings.HarmonyH2Key);
                    parameters.Add("h3Key", settings.HarmonyH3Key);
                    parameters.Add("h1Gender", settings.HarmonyH1Gender);
                    parameters.Add("h2Gender", settings.HarmonyH2Gender);
                    parameters.Add("h3Gender", settings.HarmonyH3Gender);
                    break;
                    
                case "megaphone":
                    parameters.Add("type", settings.MegaphoneType);
                    parameters.Add("param1", settings.MegaphoneParam1);
                    parameters.Add("param2", settings.MegaphoneParam2);
                    parameters.Add("param3", settings.MegaphoneParam3);
                    parameters.Add("param4", settings.MegaphoneParam4);
                    break;
                    
                case "reverb":
                    parameters.Add("type", settings.ReverbType);
                    parameters.Add("param1", settings.ReverbParam1);
                    parameters.Add("param2", settings.ReverbParam2);
                    parameters.Add("param3", settings.ReverbParam3);
                    parameters.Add("param4", settings.ReverbParam4);
                    break;
                    
                case "vocoder":
                    parameters.Add("type", settings.VocoderType);
                    parameters.Add("param1", settings.VocoderParam1);
                    parameters.Add("param2", settings.VocoderParam2);
                    parameters.Add("param3", settings.VocoderParam3);
                    parameters.Add("param4", settings.VocoderParam4);
                    break;
                    
                case "equalizer":
                    parameters.Add("eqSwitch", settings.EqualizerSwitch);
                    parameters.Add("lowShelfFreq", settings.EqualizerLowShelfFreq);
                    parameters.Add("lowShelfGain", settings.EqualizerLowShelfGain);
                    parameters.Add("lowMidFreq", settings.EqualizerLowMidFreq);
                    parameters.Add("lowMidQ", settings.EqualizerLowMidQ);
                    parameters.Add("lowMidGain", settings.EqualizerLowMidGain);
                    parameters.Add("highMidFreq", settings.EqualizerHighMidFreq);
                    parameters.Add("highMidQ", settings.EqualizerHighMidQ);
                    parameters.Add("highMidGain", settings.EqualizerHighMidGain);
                    parameters.Add("highShelfFreq", settings.EqualizerHighShelfFreq);
                    parameters.Add("highShelfGain", settings.EqualizerHighShelfGain);
                    break;
            }
            
            return parameters;
        }
    }
}