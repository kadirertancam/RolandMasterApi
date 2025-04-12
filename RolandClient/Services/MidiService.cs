using System;
using System.Collections.Generic;
using System.Threading;
using Sanford.Multimedia.Midi;
using RolandClient.Models;
using Microsoft.Extensions.Logging;

namespace RolandClient.Services
{
    /// <summary>
    /// MIDI cihazlarını kontrol etmek için servis.
    /// Hem SysEx hem de CC/Program Change mesajları gönderilebilir.
    /// </summary>
    public class MidiService : IDisposable
    {
        private OutputDevice? _midiOut;
        private bool _isInitialized = false;
        private readonly ILogger<MidiService> _logger;
        private readonly ClientSettings _clientSettings;

        public MidiService(ILogger<MidiService> logger, ClientSettings clientSettings)
        {
            _logger = logger;
            _clientSettings = clientSettings;

            if (_clientSettings.AutoInitializeMidi)
            {
                // Varsayılan cihaz ID'si burada projede ayarlanmış değere göre
                Initialize(_clientSettings.MidiDeviceId);
            }
        }

        /// <summary>
        /// MIDI çıkış cihazını başlatır
        /// </summary>
        public void Initialize(int deviceId = 1)
        {
            try
            {
                if (OutputDevice.DeviceCount == 0)
                {
                    _logger.LogWarning("Hiç MIDI çıkış cihazı bulunamadı");
                    throw new InvalidOperationException("Hiç MIDI çıkış cihazı bulunamadı");
                }

                _logger.LogInformation("Mevcut MIDI çıkış cihazları:");
                for (int i = 0; i < OutputDevice.DeviceCount; i++)
                {
                    _logger.LogInformation($"  {i}: MIDI Çıkış Cihazı {i}");
                }

                if (deviceId < 0 || deviceId >= OutputDevice.DeviceCount)
                {
                    _logger.LogWarning($"Geçersiz MIDI cihaz ID'si: {deviceId}. Geçerli aralık: 0-{OutputDevice.DeviceCount - 1}");
                    throw new ArgumentException($"Geçersiz MIDI cihaz ID'si: {deviceId}. Geçerli aralık: 0-{OutputDevice.DeviceCount - 1}");
                }

                _midiOut?.Dispose();
                _midiOut = new OutputDevice(deviceId);
                _isInitialized = true;

                _logger.LogInformation($"MIDI cihazı başlatıldı: MIDI Çıkış Cihazı {deviceId}");

                // Test komutunu gönder (SysEx örneği)
                SendTestCommand();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MIDI cihazı başlatılırken hata oluştu");
                throw;
            }
        }

        public bool IsInitialized() => _isInitialized && _midiOut != null;

        public List<(int Id, string Name)> GetAvailableMidiDevices()
        {
            var devices = new List<(int, string)>();
            for (int i = 0; i < OutputDevice.DeviceCount; i++)
            {
                devices.Add((i, $"MIDI Çıkış Cihazı {i}"));
            }
            return devices;
        }

        // Örnek SysEx test komutu
        public void SendTestCommand()
        {
            try
            {
                _logger.LogInformation("Roland VT-4 cihazı için test komutu gönderiliyor...");

                // Roland Universal Identity Request: F0 7E 7F 06 01 F7
                byte[] identityRequest = new byte[] { 0xF0, 0x7E, 0x7F, 0x06, 0x01, 0xF7 };
                _logger.LogInformation("Identity Request gönderiliyor: {Bytes}", BitConverter.ToString(identityRequest));
                
                var sysExMessage = new SysExMessage(identityRequest);
                _midiOut!.Send(sysExMessage);

                _logger.LogInformation("Test komutları gönderildi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test komutu gönderilirken hata oluştu");
            }
        }

        // SysEx mesajı gönderir
        public void SendSysExMessage(byte[] sysExMessage)
        {
            EnsureInitialized();
            try
            {
                var sysEx = new SysExMessage(sysExMessage);
                _midiOut!.Send(sysEx);
                _logger.LogInformation($"SysEx mesajı gönderildi: {BitConverter.ToString(sysExMessage)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SysEx mesajı gönderilirken hata oluştu: {Message}", ex.Message);
                throw;
            }
        }

        #region CC (Control Change) Mesajlarıyla Efekt Çağrıları

        /// <summary>
        /// Robot efektini tetikleyen CC mesajlarını gönderir (B0 31 7F ve B0 31 00)
        /// </summary>
        public void SendRobotControl()
        {
            EnsureInitialized();
            // Kanal 1 için (0 tabanlı), kontrol numarası 0x31 ve değerleri gönderiyoruz.
            var messageOn = new ChannelMessage(ChannelCommand.Controller, 0, 0x31, 0x7F);
            _midiOut!.Send(messageOn);
            
            _logger.LogInformation("Robot kontrol mesajları gönderildi");
        }

        /// <summary>
        /// Megaphone efektini tetikleyen CC mesajlarını gönderir (B0 32 7F / B0 32 00)
        /// </summary>
        public void SendMegaphoneControl()
        {
            EnsureInitialized();
            var messageOn = new ChannelMessage(ChannelCommand.Controller, 0, 0x32, 0x7F);
            _midiOut!.Send(messageOn);
            
            _logger.LogInformation("Megaphone kontrol mesajları gönderildi");
        }

        /// <summary>
        /// Vocoder efektini tetikleyen CC mesajlarını gönderir (B0 34 7F / B0 34 00)
        /// </summary>
        public void SendVocoderControl()
        {
            EnsureInitialized();
            var messageOn = new ChannelMessage(ChannelCommand.Controller, 0, 0x34, 0x7F);
            _midiOut!.Send(messageOn);
            
            _logger.LogInformation("Vocoder kontrol mesajları gönderildi");
        }

        /// <summary>
        /// Harmony efektini tetikleyen CC mesajlarını gönderir (B0 35 7F / B0 35 00)
        /// </summary>
        public void SendHarmonyControl()
        {
            EnsureInitialized();
            var messageOn = new ChannelMessage(ChannelCommand.Controller, 0, 0x35, 0x7F);
            _midiOut!.Send(messageOn);
            _logger.LogInformation("Harmony kontrol mesajları gönderildi");
        }

        /// <summary>
        /// Key slider için kontrol (örneğin B0 30 [value]) gönderir
        /// </summary>
        public void SendKeyControl(byte value)
        {
            EnsureInitialized();
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, value);
            _midiOut!.Send(message);
            _logger.LogInformation($"Key kontrol mesajı gönderildi: 0x30 {value:X2}");
        }

        /// <summary>
        /// Formant slider için kontrol (B0 36 [value])
        /// </summary>
        public void SendFormantControl(byte value)
        {
            EnsureInitialized();
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x36, value);
            _midiOut!.Send(message);
            _logger.LogInformation($"Formant kontrol mesajı gönderildi: 0x36 {value:X2}");
        }

        /// <summary>
        /// Balance slider için kontrol (B0 38 [value])
        /// </summary>
        public void SendBalanceControl(byte value)
        {
            EnsureInitialized();
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x38, value);
            _midiOut!.Send(message);
            _logger.LogInformation($"Balance kontrol mesajı gönderildi: 0x38 {value:X2}");
        }

        /// <summary>
        /// Volume slider için kontrol (B0 2E [value])
        /// </summary>
        public void SendVolumeControl(byte value)
        {
            EnsureInitialized();
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x2E, value);
            _midiOut!.Send(message);
            _logger.LogInformation($"Volume kontrol mesajı gönderildi: 0x2E {value:X2}");
        }

        /// <summary>
        /// Mic Sens slider için kontrol (B0 2F [value])
        /// </summary>
        public void SendMicSensControl(byte value)
        {
            EnsureInitialized();
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x2F, value);
            _midiOut!.Send(message);
            _logger.LogInformation($"Mic Sens kontrol mesajı gönderildi: 0x2F {value:X2}");
        }

        /// <summary>
        /// Pitch slider için, E0 mesajı gönderir (Pitch Wheel Change).
        /// Sanford API'si 0-16383 (14-bit) değeri iki 7-bitlik değere (LSB ve MSB) böler.
        /// </summary>
        public void SendPitchWheelChange(int value)
        {
            if (value < 0 || value > 16383)
                throw new ArgumentOutOfRangeException(nameof(value), "Pitch değeri 0-16383 aralığında olmalıdır");
            EnsureInitialized();
            
            // Değeri LSB ve MSB'ye dönüştür
            int lsb = value & 0x7F;         // Alt 7 bit (0-127)
            int msb = (value >> 7) & 0x7F;  // Üst 7 bit (0-127)
            
            var message = new ChannelMessage(ChannelCommand.PitchWheel, 0, lsb, msb);
            _midiOut!.Send(message);
            _logger.LogInformation($"Pitch Wheel mesajı gönderildi: {value}");
        }
            
        /// <summary>
        /// Program Change (Efekt/Scene recall) mesajı gönderir: C0 01, C0 02, C0 03, C0 04
        /// </summary>
        /// <param name="program">Program numarası (0-127)</param>
        public void SendEffectProgramChange(byte program)
        {
            // MIDI program değişim komutları 0-127 arasında olur; 
            // Genellikle 1,2,3,4 gibi değerler kullanılır (dikkat: çoğu cihazda program değişim değeri 0 bazlıdır)
            EnsureInitialized();
            var message = new ChannelMessage(ChannelCommand.ProgramChange, 0, program);
            _midiOut!.Send(message);
            _logger.LogInformation($"Program Change mesajı gönderildi: Program {program}");
        }

        #endregion

        #region Raw MIDI Komutları

        /// <summary>
        /// Robotik ses efekti için doğrudan MIDI mesajı gönderir.
        /// Bu metot, Bome SendSX ile benzer mesajı gönderir.
        /// </summary>
        public void SendRobotEffectRaw()
        {
            EnsureInitialized();
            _logger.LogInformation("Robot efekti için raw MIDI mesajı gönderiliyor");
            
            // B0 31 7F / Robot etkisini aktifleştir
            var robotOn = new ChannelMessage(ChannelCommand.Controller, 0, 0x31, 0x7F);
            _midiOut!.Send(robotOn);
            
            _logger.LogInformation("Robot efekti için raw MIDI mesajları gönderildi");
        }
        
        /// <summary>
        /// Harmony ses efekti için doğrudan MIDI mesajı gönderir.
        /// </summary>
        public void SendHarmonyEffectRaw()
        {
            EnsureInitialized();
            _logger.LogInformation("Harmony efekti için raw MIDI mesajı gönderiliyor");
            
            // B0 35 7F / Harmony etkisini aktifleştir
            var harmonyOn = new ChannelMessage(ChannelCommand.Controller, 0, 0x35, 0x7F);
            _midiOut!.Send(harmonyOn);
            
            _logger.LogInformation("Harmony efekti için raw MIDI mesajları gönderildi");
        }

        /// <summary>
        /// Megaphone ses efekti için doğrudan MIDI mesajı gönderir.
        /// </summary>
        public void SendMegaphoneEffectRaw()
        {
            EnsureInitialized();
            _logger.LogInformation("Megaphone efekti için raw MIDI mesajı gönderiliyor");
            
            // B0 32 7F / Megaphone etkisini aktifleştir
            var megaphoneOn = new ChannelMessage(ChannelCommand.Controller, 0, 0x32, 0x7F);
            _midiOut!.Send(megaphoneOn);
            
            _logger.LogInformation("Megaphone efekti için raw MIDI mesajları gönderildi");
        }

        /// <summary>
        /// Vocoder ses efekti için doğrudan MIDI mesajı gönderir.
        /// </summary>
        public void SendVocoderEffectRaw()
        {
            EnsureInitialized();
            _logger.LogInformation("Vocoder efekti için raw MIDI mesajı gönderiliyor");
            
            // B0 34 7F / Vocoder etkisini aktifleştir
            var vocoderOn = new ChannelMessage(ChannelCommand.Controller, 0, 0x34, 0x7F);
            _midiOut!.Send(vocoderOn);
            
            _logger.LogInformation("Vocoder efekti için raw MIDI mesajları gönderildi");
        }

        /// <summary>
        /// Program değişimi için doğrudan MIDI mesajı gönderir.
        /// </summary>
        public void SendProgramChangeRaw(byte program)
        {
            EnsureInitialized();
            _logger.LogInformation($"Program Change için raw MIDI mesajı gönderiliyor: Program {program}");
            
            var message = new ChannelMessage(ChannelCommand.ProgramChange, 0, program);
            _midiOut!.Send(message);
            
            _logger.LogInformation($"Program Change için raw MIDI mesajı gönderildi: C0 {program:X2}");
        }

        /// <summary>
        /// Pitch değişimi için doğrudan MIDI mesajı gönderir.
        /// </summary>
        public void SendPitchWheelRaw(byte msb, byte lsb)
        {
            EnsureInitialized();
            _logger.LogInformation($"Pitch Wheel için raw MIDI mesajı gönderiliyor: MSB={msb:X2}, LSB={lsb:X2}");
            
            // E0 XX YY / Pitch değişimi (Sanford'da PitchWheel data1=LSB, data2=MSB olur)
            var message = new ChannelMessage(ChannelCommand.PitchWheel, 0, lsb, msb);
            _midiOut!.Send(message);
            
            _logger.LogInformation($"Pitch Wheel için raw MIDI mesajı gönderildi: E0 {lsb:X2} {msb:X2}");
        }

        #endregion

        #region Efekt Metodları

        /// <summary>
        /// Robot efektini kapatır (B0 31 00)
        /// </summary>
        public void SendRobotEffectOff()
        {
            EnsureInitialized();
            _logger.LogInformation("Robot efektini kapatma komutu gönderiliyor");
            
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x31, 0x00);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Robot efekti kapatıldı");
        }

        /// <summary>
        /// Harmony efektini kapatır (B0 35 00)
        /// </summary>
        public void SendHarmonyEffectOff()
        {
            EnsureInitialized();
            _logger.LogInformation("Harmony efektini kapatma komutu gönderiliyor");
            
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x35, 0x00);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Harmony efekti kapatıldı");
        }

        /// <summary>
        /// Megaphone efektini kapatır (B0 32 00)
        /// </summary>
        public void SendMegaphoneEffectOff()
        {
            EnsureInitialized();
            _logger.LogInformation("Megaphone efektini kapatma komutu gönderiliyor");
            
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x32, 0x00);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Megaphone efekti kapatıldı");
        }

        /// <summary>
        /// Vocoder efektini kapatır (B0 34 00)
        /// </summary>
        public void SendVocoderEffectOff()
        {
            EnsureInitialized();
            _logger.LogInformation("Vocoder efektini kapatma komutu gönderiliyor");
            
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x34, 0x00);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Vocoder efekti kapatıldı");
        }

        /// <summary>
        /// Efekt1 seçimini uygular (Program Change 1)
        /// </summary>
        public void SendEffect1()
        {
            EnsureInitialized();
            _logger.LogInformation("Efekt1 komutu gönderiliyor");
            
            var message = new ChannelMessage(ChannelCommand.ProgramChange, 0, 0); // Program değişimi 0 (1. program)
            _midiOut!.Send(message);
            
            _logger.LogInformation("Efekt1 komutu gönderildi");
        }

        /// <summary>
        /// Efekt2 seçimini uygular (Program Change 2)
        /// </summary>
        public void SendEffect2()
        {
            EnsureInitialized();
            _logger.LogInformation("Efekt2 komutu gönderiliyor");
            
            var message = new ChannelMessage(ChannelCommand.ProgramChange, 0, 1); // Program değişimi 1 (2. program)
            _midiOut!.Send(message);
            
            _logger.LogInformation("Efekt2 komutu gönderildi");
        }

        /// <summary>
        /// Efekt3 seçimini uygular (Program Change 3)
        /// </summary>
        public void SendEffect3()
        {
            EnsureInitialized();
            _logger.LogInformation("Efekt3 komutu gönderiliyor");
            
            var message = new ChannelMessage(ChannelCommand.ProgramChange, 0, 2); // Program değişimi 2 (3. program)
            _midiOut!.Send(message);
            
            _logger.LogInformation("Efekt3 komutu gönderildi");
        }

        /// <summary>
        /// Efekt4 seçimini uygular (Program Change 4)
        /// </summary>
        public void SendEffect4()
        {
            EnsureInitialized();
            _logger.LogInformation("Efekt4 komutu gönderiliyor");
            
            var message = new ChannelMessage(ChannelCommand.ProgramChange, 0, 3); // Program değişimi 3 (4. program)
            _midiOut!.Send(message);
            
            _logger.LogInformation("Efekt4 komutu gönderildi");
        }
        /// <summary>
        /// Robot efektini gönderir
        /// </summary>
        public void SendRobotEffect(int octave, int feedbackSwitch, int feedbackResonance, int feedbackLevel)
        {
            EnsureInitialized();
            _logger.LogInformation($"Robot efekti gönderiliyor: Octave={octave}, FeedbackSwitch={feedbackSwitch}, FeedbackResonance={feedbackResonance}, FeedbackLevel={feedbackLevel}");
            
            // Raw MIDI komutlarını kullanarak robot efektini aktifleştir
            SendRobotEffectRaw();
            
            // Pitch ve formant değerlerine göre ayarlama yap
            // Key değeri - Octave kontrolü için B0 30 [value]
            var keyControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, (byte)octave);
            _midiOut!.Send(keyControl);
            
            // Formant değeri - B0 36 [value]
            var formantControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x36, (byte)feedbackResonance);
            _midiOut!.Send(formantControl);
            
            _logger.LogInformation("Robot efekti başarıyla gönderildi");
        }

        /// <summary>
        /// Harmony efektini gönderir
        /// </summary>
        public void SendHarmonyEffect(int h1Level, int h2Level, int h3Level, int h1Key, int h2Key, int h3Key, int h1Gender, int h2Gender, int h3Gender)
        {
            EnsureInitialized();
            _logger.LogInformation($"Harmony efekti gönderiliyor");
            
            // Raw MIDI komutlarını kullanarak harmony efektini aktifleştir
            SendHarmonyEffectRaw();
            
            // Harmony için key değerlerini ayarla - B0 30 [value]
            var keyControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, (byte)h1Key);
            _midiOut!.Send(keyControl);
            
            // Formant değerlerini ayarla - B0 36 [value]
            var formantControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x36, (byte)h1Gender);
            _midiOut!.Send(formantControl);
            
            // Balance değerlerini ayarla - B0 38 [value]
            var balanceControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x38, (byte)h1Level);
            _midiOut!.Send(balanceControl);
            
            _logger.LogInformation("Harmony efekti başarıyla gönderildi");
        }

        /// <summary>
        /// Megaphone efektini gönderir
        /// </summary>
        public void SendMegaphoneEffect(int type, int param1, int param2, int param3, int param4)
        {
            EnsureInitialized();
            _logger.LogInformation($"Megaphone efekti gönderiliyor: Type={type}");
            
            // Raw MIDI komutlarını kullanarak megaphone efektini aktifleştir
            SendMegaphoneEffectRaw();
            
            // Megaphone parametrelerini ayarla
            if (type > 0) {
                // Volume - B0 2E [value]
                var volumeControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x2E, (byte)param1);
                _midiOut!.Send(volumeControl);
                
                // Formant - B0 36 [value]
                var formantControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x36, (byte)param2);
                _midiOut!.Send(formantControl);
            }
            
            _logger.LogInformation("Megaphone efekti başarıyla gönderildi");
        }

        /// <summary>
        /// Reverb efektini gönderir
        /// </summary>
        public void SendReverbEffect(int type, int param1, int param2, int param3, int param4)
        {
            EnsureInitialized();
            _logger.LogInformation($"Reverb efekti gönderiliyor: Type={type}");
            
            // Reverb değerini ayarla - B0 39 [value]
            var reverbControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x39, (byte)param1);
            _midiOut!.Send(reverbControl);
            
            // Diğer parametreler varsa, bunları da ayarla
            if (type > 0) {
                // Örneğin balance değeri
                var balanceControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x38, (byte)param2);
                _midiOut!.Send(balanceControl);
            }
            
            _logger.LogInformation("Reverb efekti başarıyla gönderildi");
        }

        /// <summary>
        /// Vocoder efektini gönderir
        /// </summary>
        public void SendVocoderEffect(int type, int param1, int param2, int param3, int param4)
        {
            EnsureInitialized();
            _logger.LogInformation($"Vocoder efekti gönderiliyor: Type={type}");
            
            // Raw MIDI komutlarını kullanarak vocoder efektini aktifleştir
            SendVocoderEffectRaw();
            
            // Vocoder parametrelerini ayarla
            if (type > 0) {
                // Formant - B0 36 [value]
                var formantControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x36, (byte)param1);
                _midiOut!.Send(formantControl);
                
                // Balance - B0 38 [value]
                var balanceControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x38, (byte)param2);
                _midiOut!.Send(balanceControl);
            }
            
            _logger.LogInformation("Vocoder efekti başarıyla gönderildi");
        }

        /// <summary>
        /// Key A notası (MIDI notu 69 - A4)
        /// </summary>
        public void SendKeyA()
        {
            EnsureInitialized();
            _logger.LogInformation("Key A komutu gönderiliyor");
            
            // A notası için 69 (MIDI değeri) gönder - Key değeri olarak 69 kayıt et
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, 69);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Key A komutu gönderildi");
        }

        /// <summary>
        /// Key A# notası (MIDI notu 70 - A#4)
        /// </summary>
        public void SendKeyASharp()
        {
            EnsureInitialized();
            _logger.LogInformation("Key A# komutu gönderiliyor");
            
            // A# notası için 70 (MIDI değeri) gönder
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, 70);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Key A# komutu gönderildi");
        }

        /// <summary>
        /// Key B notası (MIDI notu 71 - B4)
        /// </summary>
        public void SendKeyB()
        {
            EnsureInitialized();
            _logger.LogInformation("Key B komutu gönderiliyor");
            
            // B notası için 71 (MIDI değeri) gönder
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, 71);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Key B komutu gönderildi");
        }

        /// <summary>
        /// Key C notası (MIDI notu 60 - C4)
        /// </summary>
        public void SendKeyC()
        {
            EnsureInitialized();
            _logger.LogInformation("Key C komutu gönderiliyor");
            
            // C notası için 60 (MIDI değeri) gönder
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, 60);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Key C komutu gönderildi");
        }

        /// <summary>
        /// Key C# notası (MIDI notu 61 - C#4)
        /// </summary>
        public void SendKeyCSharp()
        {
            EnsureInitialized();
            _logger.LogInformation("Key C# komutu gönderiliyor");
            
            // C# notası için 61 (MIDI değeri) gönder
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, 61);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Key C# komutu gönderildi");
        }

        /// <summary>
        /// Key D notası (MIDI notu 62 - D4)
        /// </summary>
        public void SendKeyD()
        {
            EnsureInitialized();
            _logger.LogInformation("Key D komutu gönderiliyor");
            
            // D notası için 62 (MIDI değeri) gönder
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, 62);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Key D komutu gönderildi");
        }

        /// <summary>
        /// Key D# notası (MIDI notu 63 - D#4)
        /// </summary>
        public void SendKeyDSharp()
        {
            EnsureInitialized();
            _logger.LogInformation("Key D# komutu gönderiliyor");
            
            // D# notası için 63 (MIDI değeri) gönder
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, 63);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Key D# komutu gönderildi");
        }

        /// <summary>
        /// Key E notası (MIDI notu 64 - E4)
        /// </summary>
        public void SendKeyE()
        {
            EnsureInitialized();
            _logger.LogInformation("Key E komutu gönderiliyor");
            
            // E notası için 64 (MIDI değeri) gönder
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, 64);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Key E komutu gönderildi");
        }

        /// <summary>
        /// Key F notası (MIDI notu 65 - F4)
        /// </summary>
        public void SendKeyF()
        {
            EnsureInitialized();
            _logger.LogInformation("Key F komutu gönderiliyor");
            
            // F notası için 65 (MIDI değeri) gönder
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, 65);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Key F komutu gönderildi");
        }

        /// <summary>
        /// Key F# notası (MIDI notu 66 - F#4)
        /// </summary>
        public void SendKeyFSharp()
        {
            EnsureInitialized();
            _logger.LogInformation("Key F# komutu gönderiliyor");
            
            // F# notası için 66 (MIDI değeri) gönder
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, 66);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Key F# komutu gönderildi");
        }

        /// <summary>
        /// Key G notası (MIDI notu 67 - G4)
        /// </summary>
        public void SendKeyG()
        {
            EnsureInitialized();
            _logger.LogInformation("Key G komutu gönderiliyor");
            
            // G notası için 67 (MIDI değeri) gönder
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, 67);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Key G komutu gönderildi");
        }

        /// <summary>
        /// Key G# notası (MIDI notu 68 - G#4)
        /// </summary>
        public void SendKeyGSharp()
        {
            EnsureInitialized();
            _logger.LogInformation("Key G# komutu gönderiliyor");
            
            // G# notası için 68 (MIDI değeri) gönder
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x30, 68);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Key G# komutu gönderildi");
        }

        /// <summary>
        /// Pitch değerini ayarlar (0-127 arasında değer)
        /// </summary>
        public void SendPitchValue(byte value)
        {
            EnsureInitialized();
            _logger.LogInformation($"Pitch değeri ayarlanıyor: {value}");
            
            // 14-bit pitch wheel değerini hesapla
            int pitchValue = ((int)value * 128); // 0-127 değerini 0-16256 aralığına çevir
            
            // Değeri LSB ve MSB'ye dönüştür
            byte lsb = (byte)(pitchValue & 0x7F);
            byte msb = (byte)((pitchValue >> 7) & 0x7F);
            
            var message = new ChannelMessage(ChannelCommand.PitchWheel, 0, lsb, msb);
            _midiOut!.Send(message);
            
            _logger.LogInformation($"Pitch değeri başarıyla ayarlandı: {value}");
        }

        /// <summary>
        /// Kesin değer ile mic sens ayarlar (0-127 arasında değer) (B0 2F XX)
        /// </summary>
        public void SendMicSensValue(byte value)
        {
            EnsureInitialized();
            _logger.LogInformation($"Mic Sens değeri ayarlanıyor: {value}");
            
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x2F, value);
            _midiOut!.Send(message);
            
            _logger.LogInformation($"Mic Sens değeri başarıyla ayarlandı: {value}");
        }

        /// <summary>
        /// Kesin değer ile volume ayarlar (0-127 arasında değer) (B0 2E XX)
        /// </summary>
        public void SendVolumeValue(byte value)
        {
            EnsureInitialized();
            _logger.LogInformation($"Volume değeri ayarlanıyor: {value}");
            
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x2E, value);
            _midiOut!.Send(message);
            
            _logger.LogInformation($"Volume değeri başarıyla ayarlandı: {value}");
        }

        /// <summary>
        /// Kesin değer ile reverb ayarlar (0-127 arasında değer) (B0 39 XX)
        /// </summary>
        public void SendReverbValue(byte value)
        {
            EnsureInitialized();
            _logger.LogInformation($"Reverb değeri ayarlanıyor: {value}");
            
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x39, value);
            _midiOut!.Send(message);
            
            _logger.LogInformation($"Reverb değeri başarıyla ayarlandı: {value}");
        }

        /// <summary>
        /// Kesin değer ile balance ayarlar (0-127 arasında değer) (B0 38 XX)
        /// </summary>
        public void SendBalanceValue(byte value)
        {
            EnsureInitialized();
            _logger.LogInformation($"Balance değeri ayarlanıyor: {value}");
            
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x38, value);
            _midiOut!.Send(message);
            
            _logger.LogInformation($"Balance değeri başarıyla ayarlandı: {value}");
        }

        /// <summary>
        /// Kesin değer ile formant ayarlar (0-127 arasında değer) (B0 36 XX)
        /// </summary>
        public void SendFormantValue(byte value)
        {
            EnsureInitialized();
            _logger.LogInformation($"Formant değeri ayarlanıyor: {value}");
            
            var message = new ChannelMessage(ChannelCommand.Controller, 0, 0x36, value);
            _midiOut!.Send(message);
            
            _logger.LogInformation($"Formant değeri başarıyla ayarlandı: {value}");
        }
        /// <summary>
        /// Equalizer efektini gönderir
        /// </summary>
        public void SendEqualizerEffect(int eqSwitch, int lowShelfFreq, int lowShelfGain, int lowMidFreq, int lowMidQ, int lowMidGain, int highMidFreq, int highMidQ, int highMidGain, int highShelfFreq, int highShelfGain)
        {
            EnsureInitialized();
            _logger.LogInformation($"Equalizer efekti gönderiliyor: EqSwitch={eqSwitch}");
            
            // Equalizer için özel bir kontrol metodu yok gibi görünüyor,
            // Bu nedenle efekt seçimi yapabiliriz (C0 [01-04])
            if (eqSwitch > 0) {
                // Efekt seçimi - C0 0x
                byte program = (byte)(eqSwitch > 4 ? 4 : eqSwitch);
                SendProgramChangeRaw(program);
                
                // EQ ayarları için formant, balance ve reverb değerlerini kullanabiliriz
                var formantControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x36, (byte)lowShelfGain);
                _midiOut!.Send(formantControl);
                
                var balanceControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x38, (byte)lowMidGain);
                _midiOut!.Send(balanceControl);
                
                var reverbControl = new ChannelMessage(ChannelCommand.Controller, 0, 0x39, (byte)highMidGain);
                _midiOut!.Send(reverbControl);
            }
            
            _logger.LogInformation("Equalizer efekti başarıyla gönderildi");
        }
        
        /// <summary>
        /// Pitch değerini ayarla (Pitch Wheel Change - E0)
        /// </summary>
        public void SendPitchEffect(int value)
        {
            EnsureInitialized();
            _logger.LogInformation($"Pitch değeri ayarlanıyor: {value}");
            
            // Pitch değerini ayarla - E0 xx yy (14-bit pitch wheel message)
            int pitchValue = value & 0x3FFF; // 14-bit değere sınırla (0-16383)
            
            // 14-bit değeri MSB ve LSB olarak ayrıştır
            byte lsb = (byte)(pitchValue & 0x7F);
            byte msb = (byte)((pitchValue >> 7) & 0x7F);
            
            var message = new ChannelMessage(ChannelCommand.PitchWheel, 0, lsb, msb);
            _midiOut!.Send(message);
            
            _logger.LogInformation("Pitch değeri başarıyla ayarlandı");
        }

        #endregion

        #region Yardımcı Metodlar

        /// <summary>
        /// Roland checksum hesaplama: 
        /// Belirtilen baytların toplamı üzerinden, alt 7 bit çıkartılarak hesaplanır.
        /// </summary>
        private byte CalculateRolandChecksum(byte[] data, int start, int count)
        {
            int sum = 0;
            for (int i = start; i < start + count; i++)
            {
                sum += data[i];
            }
            return (byte)((128 - (sum % 128)) % 128);
        }

        /// <summary>
        /// MIDI cihazının başlatıldığından emin olur.
        /// </summary>
        private void EnsureInitialized()
        {
            if (!_isInitialized || _midiOut == null)
            {
                _logger.LogWarning("MIDI cihazı başlatılmadı");
                throw new InvalidOperationException("MIDI cihazı başlatılmadı. Önce Initialize() metodunu çağırın.");
            }
        }

        #endregion

        /// <summary>
        /// Kaynakları temizler
        /// </summary>
        public void Dispose()
        {
            _midiOut?.Dispose();
            _midiOut = null;
            _isInitialized = false;
            GC.SuppressFinalize(this);
        }
    }
}