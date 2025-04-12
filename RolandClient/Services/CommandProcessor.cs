using RolandClient.Models;

namespace RolandClient.Services
{
    /// <summary>
    /// Redis'ten gelen komutları işleyerek MIDI cihazına ileten servis
    /// </summary>
    public class CommandProcessor
    {
        private readonly ILogger<CommandProcessor> _logger;
        private readonly MidiService _midiService;
        private readonly DirectMidiSender _directMidi;
        private readonly RedisService _redisService;
        private readonly ClientSettings _clientSettings;
        
        // Cihazın güncel ayarları (son komutla güncellenir)
        private RolandClientSettings _currentSettings;

        public CommandProcessor(
            ILogger<CommandProcessor> logger,
            MidiService midiService,
            DirectMidiSender directMidi,
            RedisService redisService,
            ClientSettings clientSettings)
        {
            _logger = logger;
            _midiService = midiService;
            _directMidi = directMidi;
            _redisService = redisService;
            _clientSettings = clientSettings;
            
            // Varsayılan ayarları oluştur
            _currentSettings = new RolandClientSettings
            {
                DeviceId = _clientSettings.DeviceId,
                DeviceName = _clientSettings.DeviceName,
                MacAddress = _clientSettings.MacAddress,
                MidiDeviceId = _clientSettings.MidiDeviceId
            };
        }

        /// <summary>
        /// Güncel ayarları döndürür
        /// </summary>
        public RolandClientSettings GetCurrentSettings()
        {
            return _currentSettings;
        }

        /// <summary>
        /// Komutu işler ve MIDI cihazına gönderir
        /// </summary>
        public async Task ProcessCommandAsync(RolandEffectCommand command)
        {
            try
            {
                _logger.LogInformation("Komut işleniyor: {CommandId}, Efekt: {EffectType}, Parametreler: {Parameters}", 
                    command.CommandId, command.EffectType, 
                    System.Text.Json.JsonSerializer.Serialize(command.Parameters));
                
                // Komut durumunu "executing" olarak işaretle
                command.Status = "executing";
                
                // MIDI cihazının başlatıldığından emin ol
                if (!_midiService.IsInitialized())
                {
                    _logger.LogInformation("MIDI cihazı başlatılıyor: {MidiDeviceId}", _clientSettings.MidiDeviceId);
                    _midiService.Initialize(_clientSettings.MidiDeviceId);
                }
                else
                {
                    _logger.LogInformation("MIDI cihazı zaten başlatılmış");
                }
                
                // Windows MIDI API cihazını da başlat
                if (!_directMidi.IsInitialized())
                {
                    _logger.LogInformation("Direct MIDI Sender ile cihaz başlatılıyor: {MidiDeviceId}", _clientSettings.MidiDeviceId);
                    _directMidi.Initialize(_clientSettings.MidiDeviceId);
                }
                
                // Komutu efekt tipine göre işle
                _logger.LogInformation("Efekt uygulanıyor: {EffectType}", command.EffectType);
                bool success = await ApplyEffectAsync(command);
                _logger.LogInformation("Efekt uygulama sonucu: {Success}", success);
                
                // Formatted MAC address for consistent DeviceId usage
                string formattedMacAddress = _clientSettings.MacAddress.Replace(":", "").Replace("-", "").ToUpper();
                
                // Komut yanıtını oluştur
                var response = new RolandCommandResponse
                {
                    CommandId = command.CommandId,
                    DeviceId = formattedMacAddress, // Format düzenlenmiş MAC adresini deviceId olarak kullan
                    Status = success ? "completed" : "failed",
                    ErrorMessage = success ? null : "Komut uygulanırken hata oluştu"
                };
                
                // Yanıtı gönder
                await _redisService.SendCommandResponseAsync(response);
                
                // Güncel ayarları Redis'e kaydet
                await _redisService.UpdateSettingsAsync(_currentSettings);
                
                _logger.LogInformation("Komut başarıyla işlendi: {CommandId}", command.CommandId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Komut işlenirken hata oluştu: {CommandId}", command.CommandId);
                
                // Formatted MAC address for consistent DeviceId usage
                string formattedMacAddress = _clientSettings.MacAddress.Replace(":", "").Replace("-", "").ToUpper();
                
                // Hata yanıtı gönder
                var errorResponse = new RolandCommandResponse
                {
                    CommandId = command.CommandId,
                    DeviceId = formattedMacAddress, // Format düzenlenmiş MAC adresini deviceId olarak kullan
                    Status = "failed",
                    ErrorMessage = $"Hata: {ex.Message}"
                };
                
                await _redisService.SendCommandResponseAsync(errorResponse);
            }
        }

        /// <summary>
        /// Komutu MIDI cihazına uygular
        /// </summary>
        private async Task<bool> ApplyEffectAsync(RolandEffectCommand command)
        {
            try
            {
                _logger.LogInformation("Efekt uygulama başlıyor: {EffectType}, CommandID: {CommandId}", 
                    command.EffectType, command.CommandId);

                switch (command.EffectType.ToLower())
                {
                    case "robot":
                        _logger.LogInformation("Robot efekti uygulanıyor (Raw MIDI)");
                        ApplyRobotEffect(command);
                        break;
                        
                    case "harmony":
                        _logger.LogInformation("Harmony efekti uygulanıyor (Direct MIDI)");
                        ApplyHarmonyEffect(command);
                        break;
                        
                    case "megaphone":
                        _logger.LogInformation("Megaphone efekti uygulanıyor (Raw MIDI)");
                        ApplyMegaphoneEffect(command);
                        break;
                        
                    case "reverb":
                        _logger.LogInformation("Reverb efekti uygulanıyor");
                        ApplyReverbEffect(command);
                        break;
                        
                    case "vocoder":
                        _logger.LogInformation("Vocoder efekti uygulanıyor (Raw MIDI)");
                        ApplyVocoderEffect(command);
                        break;
                        
                    case "equalizer":
                        _logger.LogInformation("Equalizer efekti uygulanıyor");
                        ApplyEqualizerEffect(command);
                        break;
                        
                    case "none":
                        // Efekt kapatma (ayarları koru ama hiçbir efekt uygulanmasın)
                        _logger.LogInformation("Efekt kapatılıyor (none)");
                        _currentSettings.ActiveEffect = "none";
                        break;
                        
                    case "robotoff":
                        _logger.LogInformation("Robot efekti kapatılıyor");
                        _midiService.SendRobotEffectOff();
                        _currentSettings.ActiveEffect = "none";
                        break;
                        
                    case "harmonyoff":
                        _logger.LogInformation("Harmony efekti kapatılıyor");
                        _midiService.SendHarmonyEffectOff();
                        _currentSettings.ActiveEffect = "none";
                        break;
                        
                    case "megaphoneoff":
                        _logger.LogInformation("Megaphone efekti kapatılıyor");
                        _midiService.SendMegaphoneEffectOff();
                        _currentSettings.ActiveEffect = "none";
                        break;
                        
                    case "vocoderoff":
                        _logger.LogInformation("Vocoder efekti kapatılıyor");
                        _midiService.SendVocoderEffectOff();
                        _currentSettings.ActiveEffect = "none";
                        break;
                        
                    case "effect1":
                        _logger.LogInformation("Efekt 1 uygulanıyor");
                        _midiService.SendEffect1();
                        _currentSettings.ActiveEffect = "effect1";
                        break;
                        
                    case "effect2":
                        _logger.LogInformation("Efekt 2 uygulanıyor");
                        _midiService.SendEffect2();
                        _currentSettings.ActiveEffect = "effect2";
                        break;
                        
                    case "effect3":
                        _logger.LogInformation("Efekt 3 uygulanıyor");
                        _midiService.SendEffect3();
                        _currentSettings.ActiveEffect = "effect3";
                        break;
                        
                    case "effect4":
                        _logger.LogInformation("Efekt 4 uygulanıyor");
                        _midiService.SendEffect4();
                        _currentSettings.ActiveEffect = "effect4";
                        break;
                        
                    default:
                        _logger.LogWarning("Bilinmeyen efekt tipi: {EffectType}", command.EffectType);
                        return false;
                }
                
                _logger.LogInformation("Efekt başarıyla uygulandı: {EffectType}, ActiveEffect: {ActiveEffect}", 
                    command.EffectType, _currentSettings.ActiveEffect);
                
                // Başarılı ise
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Efekt uygulanırken hata oluştu: {EffectType}, Hata: {Message}", 
                    command.EffectType, ex.Message);
                return false;
            }
        }
        
        #region Efekt Uygulama Metodları

        /// <summary>
        /// Robot efektini uygular
        /// </summary>
        private void ApplyRobotEffect(RolandEffectCommand command)
        {
            _logger.LogInformation("Robot efekt parametreleri alınıyor");
            
            // Önceki değerleri logla
            _logger.LogInformation("Mevcut Robot ayarları: Octave={Octave}, FeedbackSwitch={FeedbackSwitch}, FeedbackResonance={FeedbackResonance}, FeedbackLevel={FeedbackLevel}", 
                _currentSettings.RobotOctave, _currentSettings.RobotFeedbackSwitch, 
                _currentSettings.RobotFeedbackResonance, _currentSettings.RobotFeedbackLevel);
                
            var octave = command.GetParameterValue("octave", _currentSettings.RobotOctave);
            var feedbackSwitch = command.GetParameterValue("feedbackSwitch", _currentSettings.RobotFeedbackSwitch);
            var feedbackResonance = command.GetParameterValue("feedbackResonance", _currentSettings.RobotFeedbackResonance);
            var feedbackLevel = command.GetParameterValue("feedbackLevel", _currentSettings.RobotFeedbackLevel);
            
            _logger.LogInformation("Komuttan alınan Robot parametreleri: Octave={Octave}, FeedbackSwitch={FeedbackSwitch}, FeedbackResonance={FeedbackResonance}, FeedbackLevel={FeedbackLevel}", 
                octave, feedbackSwitch, feedbackResonance, feedbackLevel);
            
            // Ayarları güncelle
            _currentSettings.ActiveEffect = "robot";
            _currentSettings.RobotOctave = octave;
            _currentSettings.RobotFeedbackSwitch = feedbackSwitch;
            _currentSettings.RobotFeedbackResonance = feedbackResonance;
            _currentSettings.RobotFeedbackLevel = feedbackLevel;
            
            _logger.LogInformation("Ayarlar güncellendi, MIDI komutu gönderiliyor");
            
            // Üç farklı yöntem deneyeceğiz: Windows API, Raw MIDI, NAudio sınıfları
            bool success = false;
            
            // 1. Windows MIDI API
            try
            {
                _logger.LogInformation("Direct MIDI ile Robot efekti gönderiliyor");
                
                // Robot efektini tetikle
                bool result = _directMidi.SendRobotEffect();
                
                if (result)
                {
                    // Key (Octave) değerini ayarla
                    _directMidi.SendKeyControl((byte)octave);
                    
                    // Formant değerini ayarla
                    _directMidi.SendFormantControl((byte)feedbackResonance);
                    
                    _logger.LogInformation("Direct MIDI ile Robot efekti başarıyla gönderildi");
                    success = true;
                }
                else
                {
                    _logger.LogWarning("Direct MIDI ile Robot efekti gönderilemedi, diğer yöntemler deneniyor");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Direct MIDI ile gönderirken hata oluştu: {Message}", ex.Message);
            }
            
            // 2. Raw MIDI yöntemi
            if (!success)
            {
                try
                {
                    _logger.LogInformation("Raw MIDI yöntemiyle Robot efekti gönderiliyor");
                    _midiService.SendRobotEffectRaw();
                    
                    // Ayarlar için parametreleri gönderelim
                    _midiService.SendKeyControl((byte)octave);
                    _midiService.SendFormantControl((byte)feedbackResonance);
                    
                    _logger.LogInformation("Raw MIDI ile Robot efekti başarıyla uygulandı");
                    success = true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Raw MIDI ile gönderirken hata oluştu: {Message}. Normal metot deneniyor.", ex.Message);
                }
            }
            
            // 3. Sanford sınıfları (eski NAudio sınıfları)
            if (!success)
            {
                try
                {
                    _logger.LogInformation("Sanford sınıflarıyla Robot efekti gönderiliyor");
                    _midiService.SendRobotEffect(octave, feedbackSwitch, feedbackResonance, feedbackLevel);
                    _logger.LogInformation("Sanford sınıflarıyla Robot efekti başarıyla gönderildi");
                    success = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Sanford sınıflarıyla gönderirken hata oluştu: {Message}", ex.Message);
                }
            }
            
            if (!success)
            {
                _logger.LogError("Hiçbir MIDI yöntemi başarılı olmadı!");
                throw new InvalidOperationException("MIDI komutları gönderilemedi");
            }
        }

        /// <summary>
        /// Harmony efektini uygular
        /// </summary>
        private void ApplyHarmonyEffect(RolandEffectCommand command)
        {
            _logger.LogInformation("Harmony efekt parametreleri alınıyor");
            
            // Önceki değerleri logla
            _logger.LogInformation("Mevcut Harmony ayarları");
            
            var h1Level = command.GetParameterValue("h1Level", _currentSettings.HarmonyH1Level);
            var h2Level = command.GetParameterValue("h2Level", _currentSettings.HarmonyH2Level);
            var h3Level = command.GetParameterValue("h3Level", _currentSettings.HarmonyH3Level);
            var h1Key = command.GetParameterValue("h1Key", _currentSettings.HarmonyH1Key);
            var h2Key = command.GetParameterValue("h2Key", _currentSettings.HarmonyH2Key);
            var h3Key = command.GetParameterValue("h3Key", _currentSettings.HarmonyH3Key);
            var h1Gender = command.GetParameterValue("h1Gender", _currentSettings.HarmonyH1Gender);
            var h2Gender = command.GetParameterValue("h2Gender", _currentSettings.HarmonyH2Gender);
            var h3Gender = command.GetParameterValue("h3Gender", _currentSettings.HarmonyH3Gender);
            
            _logger.LogInformation("Komuttan alınan Harmony parametreleri");
            
            // Ayarları güncelle
            _currentSettings.ActiveEffect = "harmony";
            _currentSettings.HarmonyH1Level = h1Level;
            _currentSettings.HarmonyH2Level = h2Level;
            _currentSettings.HarmonyH3Level = h3Level;
            _currentSettings.HarmonyH1Key = h1Key;
            _currentSettings.HarmonyH2Key = h2Key;
            _currentSettings.HarmonyH3Key = h3Key;
            _currentSettings.HarmonyH1Gender = h1Gender;
            _currentSettings.HarmonyH2Gender = h2Gender;
            _currentSettings.HarmonyH3Gender = h3Gender;
            
            _logger.LogInformation("Ayarlar güncellendi, MIDI komutu gönderiliyor");
            
            // Önce raw MIDI yöntemini deneyelim
            try
            {
                _midiService.SendHarmonyEffectRaw();
                
                // Ayarlar için parametreleri gönderelim
                _midiService.SendKeyControl((byte)h1Key);
                _midiService.SendFormantControl((byte)h1Gender);
                _midiService.SendBalanceControl((byte)h1Level);
                
                _logger.LogInformation("Harmony efekti Raw MIDI ile başarıyla uygulandı");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Raw MIDI ile gönderirken hata oluştu: {Message}. Normal metot deneniyor.", ex.Message);
                
                // Normal MIDI komutlarını gönder
                _midiService.SendHarmonyEffect(
                    h1Level, h2Level, h3Level,
                    h1Key, h2Key, h3Key,
                    h1Gender, h2Gender, h3Gender);
            }
                
            _logger.LogInformation("Harmony efekti başarıyla uygulandı");
        }

        /// <summary>
        /// Megaphone efektini uygular
        /// </summary>
        private void ApplyMegaphoneEffect(RolandEffectCommand command)
        {
            _logger.LogInformation("Megaphone efekt parametreleri alınıyor");
            
            var type = command.GetParameterValue("type", _currentSettings.MegaphoneType);
            var param1 = command.GetParameterValue("param1", _currentSettings.MegaphoneParam1);
            var param2 = command.GetParameterValue("param2", _currentSettings.MegaphoneParam2);
            var param3 = command.GetParameterValue("param3", _currentSettings.MegaphoneParam3);
            var param4 = command.GetParameterValue("param4", _currentSettings.MegaphoneParam4);
            
            _logger.LogInformation("Komuttan alınan Megaphone parametreleri: Type={Type}", type);
            
            // Ayarları güncelle
            _currentSettings.ActiveEffect = "megaphone";
            _currentSettings.MegaphoneType = type;
            _currentSettings.MegaphoneParam1 = param1;
            _currentSettings.MegaphoneParam2 = param2;
            _currentSettings.MegaphoneParam3 = param3;
            _currentSettings.MegaphoneParam4 = param4;
            
            _logger.LogInformation("Ayarlar güncellendi, MIDI komutu gönderiliyor");
            
            // Önce raw MIDI yöntemini deneyelim
            try
            {
                _midiService.SendMegaphoneEffectRaw();
                
                // Volume ve formant ayarlarını da gönder
                if (type > 0)
                {
                    _midiService.SendVolumeControl((byte)param1);
                    _midiService.SendFormantControl((byte)param2);
                }
                
                _logger.LogInformation("Megaphone efekti Raw MIDI ile başarıyla uygulandı");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Raw MIDI ile gönderirken hata oluştu: {Message}. Normal metot deneniyor.", ex.Message);
                
                // Normal MIDI komutunu gönder
                _midiService.SendMegaphoneEffect(type, param1, param2, param3, param4);
            }
            
            _logger.LogInformation("Megaphone efekti başarıyla uygulandı");
        }

        /// <summary>
        /// Reverb efektini uygular
        /// </summary>
        private void ApplyReverbEffect(RolandEffectCommand command)
        {
            _logger.LogInformation("Reverb efekt parametreleri alınıyor");
            
            var type = command.GetParameterValue("type", _currentSettings.ReverbType);
            var param1 = command.GetParameterValue("param1", _currentSettings.ReverbParam1);
            var param2 = command.GetParameterValue("param2", _currentSettings.ReverbParam2);
            var param3 = command.GetParameterValue("param3", _currentSettings.ReverbParam3);
            var param4 = command.GetParameterValue("param4", _currentSettings.ReverbParam4);
            
            _logger.LogInformation("Komuttan alınan Reverb parametreleri: Type={Type}", type);
            
            // Ayarları güncelle
            _currentSettings.ActiveEffect = "reverb";
            _currentSettings.ReverbType = type;
            _currentSettings.ReverbParam1 = param1;
            _currentSettings.ReverbParam2 = param2;
            _currentSettings.ReverbParam3 = param3;
            _currentSettings.ReverbParam4 = param4;
            
            _logger.LogInformation("Ayarlar güncellendi, MIDI komutu gönderiliyor");
            
            // MIDI komutunu gönder
            _midiService.SendReverbEffect(type, param1, param2, param3, param4);
            
            _logger.LogInformation("Reverb efekti başarıyla uygulandı");
        }

        /// <summary>
        /// Vocoder efektini uygular
        /// </summary>
        private void ApplyVocoderEffect(RolandEffectCommand command)
        {
            _logger.LogInformation("Vocoder efekt parametreleri alınıyor");
            
            var type = command.GetParameterValue("type", _currentSettings.VocoderType);
            var param1 = command.GetParameterValue("param1", _currentSettings.VocoderParam1);
            var param2 = command.GetParameterValue("param2", _currentSettings.VocoderParam2);
            var param3 = command.GetParameterValue("param3", _currentSettings.VocoderParam3);
            var param4 = command.GetParameterValue("param4", _currentSettings.VocoderParam4);
            
            _logger.LogInformation("Komuttan alınan Vocoder parametreleri: Type={Type}", type);
            
            // Ayarları güncelle
            _currentSettings.ActiveEffect = "vocoder";
            _currentSettings.VocoderType = type;
            _currentSettings.VocoderParam1 = param1;
            _currentSettings.VocoderParam2 = param2;
            _currentSettings.VocoderParam3 = param3;
            _currentSettings.VocoderParam4 = param4;
            
            _logger.LogInformation("Ayarlar güncellendi, MIDI komutu gönderiliyor");
            
            // Önce raw MIDI yöntemini deneyelim
            try
            {
                _midiService.SendVocoderEffectRaw();
                
                // Vocoder parametrelerini ayarla
                if (type > 0)
                {
                    _midiService.SendFormantControl((byte)param1);
                    _midiService.SendBalanceControl((byte)param2);
                }
                
                _logger.LogInformation("Vocoder efekti Raw MIDI ile başarıyla uygulandı");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Raw MIDI ile gönderirken hata oluştu: {Message}. Normal metot deneniyor.", ex.Message);
                
                // Normal MIDI komutunu gönder
                _midiService.SendVocoderEffect(type, param1, param2, param3, param4);
            }
            
            _logger.LogInformation("Vocoder efekti başarıyla uygulandı");
        }

        /// <summary>
        /// Equalizer efektini uygular
        /// </summary>
        private void ApplyEqualizerEffect(RolandEffectCommand command)
        {
            _logger.LogInformation("Equalizer efekt parametreleri alınıyor");
            
            var eqSwitch = command.GetParameterValue("eqSwitch", _currentSettings.EqualizerSwitch);
            var lowShelfFreq = command.GetParameterValue("lowShelfFreq", _currentSettings.EqualizerLowShelfFreq);
            var lowShelfGain = command.GetParameterValue("lowShelfGain", _currentSettings.EqualizerLowShelfGain);
            var lowMidFreq = command.GetParameterValue("lowMidFreq", _currentSettings.EqualizerLowMidFreq);
            var lowMidQ = command.GetParameterValue("lowMidQ", _currentSettings.EqualizerLowMidQ);
            var lowMidGain = command.GetParameterValue("lowMidGain", _currentSettings.EqualizerLowMidGain);
            var highMidFreq = command.GetParameterValue("highMidFreq", _currentSettings.EqualizerHighMidFreq);
            var highMidQ = command.GetParameterValue("highMidQ", _currentSettings.EqualizerHighMidQ);
            var highMidGain = command.GetParameterValue("highMidGain", _currentSettings.EqualizerHighMidGain);
            var highShelfFreq = command.GetParameterValue("highShelfFreq", _currentSettings.EqualizerHighShelfFreq);
            var highShelfGain = command.GetParameterValue("highShelfGain", _currentSettings.EqualizerHighShelfGain);
            
            _logger.LogInformation("Komuttan alınan Equalizer parametreleri: EqSwitch={EqSwitch}", eqSwitch);
            
            // Ayarları güncelle
            _currentSettings.ActiveEffect = "equalizer";
            _currentSettings.EqualizerSwitch = eqSwitch;
            _currentSettings.EqualizerLowShelfFreq = lowShelfFreq;
            _currentSettings.EqualizerLowShelfGain = lowShelfGain;
            _currentSettings.EqualizerLowMidFreq = lowMidFreq;
            _currentSettings.EqualizerLowMidQ = lowMidQ;
            _currentSettings.EqualizerLowMidGain = lowMidGain;
            _currentSettings.EqualizerHighMidFreq = highMidFreq;
            _currentSettings.EqualizerHighMidQ = highMidQ;
            _currentSettings.EqualizerHighMidGain = highMidGain;
            _currentSettings.EqualizerHighShelfFreq = highShelfFreq;
            _currentSettings.EqualizerHighShelfGain = highShelfGain;
            
            _logger.LogInformation("Ayarlar güncellendi, MIDI komutu gönderiliyor");
            
            // MIDI komutunu gönder
            _midiService.SendEqualizerEffect(
                eqSwitch, lowShelfFreq, lowShelfGain,
                lowMidFreq, lowMidQ, lowMidGain,
                highMidFreq, highMidQ, highMidGain,
                highShelfFreq, highShelfGain);
                
            _logger.LogInformation("Equalizer efekti başarıyla uygulandı");
        }

        #endregion
        
        #region Slider Kontrol Metodları
        
        /// <summary>
        /// Key (nota) değerini ayarlar
        /// </summary>
        private void ApplyKeyControl(RolandEffectCommand command)
        {
            _logger.LogInformation("Key değeri ayarlanıyor");
            
            var value = command.GetParameterValue("value", _currentSettings.KeyValue);
            _logger.LogInformation($"Komuttan alınan Key değeri: {value}");
            
            // Ayarları güncelle
            _currentSettings.KeyValue = value;
            
            // MIDI komutunu gönder
            _midiService.SendKeyControl((byte)value);
            
            _logger.LogInformation($"Key değeri başarıyla ayarlandı: {value}");
        }
        
        /// <summary>
        /// Mikrofon hassasiyet değerini ayarlar
        /// </summary>
        private void ApplyMicSensControl(RolandEffectCommand command)
        {
            _logger.LogInformation("Mikrofon hassasiyet değeri ayarlanıyor");
            
            var value = command.GetParameterValue("value", _currentSettings.MicSensValue);
            _logger.LogInformation($"Komuttan alınan Mic Sens değeri: {value}");
            
            // Ayarları güncelle
            _currentSettings.MicSensValue = value;
            
            // MIDI komutunu gönder
            _midiService.SendMicSensValue((byte)value);
            
            _logger.LogInformation($"Mikrofon hassasiyet değeri başarıyla ayarlandı: {value}");
        }
        
        /// <summary>
        /// Ses seviyesi değerini ayarlar
        /// </summary>
        private void ApplyVolumeControl(RolandEffectCommand command)
        {
            _logger.LogInformation("Ses seviyesi değeri ayarlanıyor");
            
            var value = command.GetParameterValue("value", _currentSettings.VolumeValue);
            _logger.LogInformation($"Komuttan alınan Volume değeri: {value}");
            
            // Ayarları güncelle
            _currentSettings.VolumeValue = value;
            
            // MIDI komutunu gönder
            _midiService.SendVolumeValue((byte)value);
            
            _logger.LogInformation($"Ses seviyesi değeri başarıyla ayarlandı: {value}");
        }
        
        /// <summary>
        /// Reverb seviyesi değerini ayarlar
        /// </summary>
        private void ApplyReverbValueControl(RolandEffectCommand command)
        {
            _logger.LogInformation("Reverb seviyesi değeri ayarlanıyor");
            
            var value = command.GetParameterValue("value", _currentSettings.ReverbValue);
            _logger.LogInformation($"Komuttan alınan Reverb değeri: {value}");
            
            // Ayarları güncelle
            _currentSettings.ReverbValue = value;
            
            // MIDI komutunu gönder
            _midiService.SendReverbValue((byte)value);
            
            _logger.LogInformation($"Reverb seviyesi değeri başarıyla ayarlandı: {value}");
        }
        
        /// <summary>
        /// Balance seviyesi değerini ayarlar
        /// </summary>
        private void ApplyBalanceControl(RolandEffectCommand command)
        {
            _logger.LogInformation("Balance seviyesi değeri ayarlanıyor");
            
            var value = command.GetParameterValue("value", _currentSettings.BalanceValue);
            _logger.LogInformation($"Komuttan alınan Balance değeri: {value}");
            
            // Ayarları güncelle
            _currentSettings.BalanceValue = value;
            
            // MIDI komutunu gönder
            _midiService.SendBalanceValue((byte)value);
            
            _logger.LogInformation($"Balance seviyesi değeri başarıyla ayarlandı: {value}");
        }
        
        /// <summary>
        /// Formant seviyesi değerini ayarlar
        /// </summary>
        private void ApplyFormantControl(RolandEffectCommand command)
        {
            _logger.LogInformation("Formant seviyesi değeri ayarlanıyor");
            
            var value = command.GetParameterValue("value", _currentSettings.FormantValue);
            _logger.LogInformation($"Komuttan alınan Formant değeri: {value}");
            
            // Ayarları güncelle
            _currentSettings.FormantValue = value;
            
            // MIDI komutunu gönder
            _midiService.SendFormantValue((byte)value);
            
            _logger.LogInformation($"Formant seviyesi değeri başarıyla ayarlandı: {value}");
        }
        
        /// <summary>
        /// Pitch seviyesi değerini ayarlar
        /// </summary>
        private void ApplyPitchControl(RolandEffectCommand command)
        {
            _logger.LogInformation("Pitch seviyesi değeri ayarlanıyor");
            
            var value = command.GetParameterValue("value", _currentSettings.PitchValue);
            _logger.LogInformation($"Komuttan alınan Pitch değeri: {value}");
            
            // Ayarları güncelle
            _currentSettings.PitchValue = value;
            
            // MIDI komutunu gönder - Pitch değeri 0-16383 arasında olmalıdır
            _midiService.SendPitchEffect(value);
            
            _logger.LogInformation($"Pitch seviyesi değeri başarıyla ayarlandı: {value}");
        }
        
        #endregion
    }
}