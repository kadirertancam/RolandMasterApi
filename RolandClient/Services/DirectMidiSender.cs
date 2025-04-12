using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace RolandClient.Services
{
    /// <summary>
    /// MIDI mesajlarını doğrudan Windows MIDI API'si kullanarak gönderen sınıf (Bome SendSX benzeri)
    /// </summary>
    public class DirectMidiSender : IDisposable
    {
        private readonly ILogger<DirectMidiSender> _logger;
        private IntPtr _midiOutHandle = IntPtr.Zero;
        private bool _isInitialized = false;

        // Windows MIDI API sabitleri
        private const int MMSYSERR_NOERROR = 0;
        private const int CALLBACK_NULL = 0;

        // Windows MIDI API fonksiyonları
        [DllImport("winmm.dll")]
        private static extern int midiOutGetNumDevs();

        [DllImport("winmm.dll")]
        private static extern int midiOutGetDevCaps(int deviceID, ref MIDIOUTCAPS caps, int sizeOfMidiOutCaps);

        [DllImport("winmm.dll")]
        private static extern int midiOutOpen(ref IntPtr handle, int deviceID, IntPtr callback, IntPtr instance, int flags);

        [DllImport("winmm.dll")]
        private static extern int midiOutClose(IntPtr handle);

        [DllImport("winmm.dll")]
        private static extern int midiOutShortMsg(IntPtr handle, uint message);

        [DllImport("winmm.dll")]
        private static extern int midiOutLongMsg(IntPtr handle, ref MIDIHDR header, int sizeOfMidiHeader);

        [DllImport("winmm.dll")]
        private static extern int midiOutPrepareHeader(IntPtr handle, ref MIDIHDR header, int sizeOfMidiHeader);

        [DllImport("winmm.dll")]
        private static extern int midiOutUnprepareHeader(IntPtr handle, ref MIDIHDR header, int sizeOfMidiHeader);

        [StructLayout(LayoutKind.Sequential)]
        private struct MIDIOUTCAPS
        {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public ushort wTechnology;
            public ushort wVoices;
            public ushort wNotes;
            public ushort wChannelMask;
            public uint dwSupport;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MIDIHDR
        {
            public IntPtr lpData;
            public uint dwBufferLength;
            public uint dwBytesRecorded;
            public IntPtr dwUser;
            public uint dwFlags;
            public IntPtr lpNext;
            public IntPtr reserved;
            public uint dwOffset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public uint[] dwReserved;
        }

        public DirectMidiSender(ILogger<DirectMidiSender> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Kullanılabilir MIDI çıkış cihazlarını listeler
        /// </summary>
        public List<(int Id, string Name)> GetAvailableMidiDevices()
        {
            var devices = new List<(int, string)>();
            int numDevs = midiOutGetNumDevs();

            for (int i = 0; i < numDevs; i++)
            {
                MIDIOUTCAPS caps = new MIDIOUTCAPS();
                int size = Marshal.SizeOf(typeof(MIDIOUTCAPS));
                int result = midiOutGetDevCaps(i, ref caps, size);
                if (result == MMSYSERR_NOERROR)
                {
                    devices.Add((i, caps.szPname));
                }
            }
            return devices;
        }

        /// <summary>
        /// MIDI cihazını başlatır
        /// </summary>
        public bool Initialize(int deviceId = 0)
        {
            try
            {
                if (_isInitialized)
                {
                    _logger.LogInformation("MIDI cihazı zaten başlatılmış");
                    return true;
                }

                if (deviceId < 0)
                {
                    _logger.LogWarning("Geçersiz cihaz ID'si: {DeviceId}", deviceId);
                    deviceId = 0;
                }

                int result = midiOutOpen(ref _midiOutHandle, deviceId, IntPtr.Zero, IntPtr.Zero, CALLBACK_NULL);
                if (result == MMSYSERR_NOERROR)
                {
                    _isInitialized = true;
                    _logger.LogInformation("MIDI cihazı başarıyla başlatıldı: DeviceId={DeviceId}", deviceId);
                    return true;
                }
                else
                {
                    _logger.LogError("MIDI cihazı başlatılamadı. Hata kodu: {ErrorCode}", result);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MIDI cihazı başlatılırken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// MIDI mesajını doğrudan gönderir (Bome SendSX'te gördüğünüz gibi)
        /// </summary>
        public bool SendRawMidiMessage(byte[] message)
        {
            if (!_isInitialized || _midiOutHandle == IntPtr.Zero)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            try
            {
                // İlk byte status byte
                byte status = message[0];

                if (message.Length <= 3 && status != 0xF0)
                {
                    // Bu bir kısa MIDI mesajı (örneğin Control Change, Program Change, Note On/Off)
                    uint midiMessage = 0;
                    midiMessage |= (uint)message[0];

                    if (message.Length > 1)
                        midiMessage |= ((uint)message[1] << 8);

                    if (message.Length > 2)
                        midiMessage |= ((uint)message[2] << 16);

                    int result = midiOutShortMsg(_midiOutHandle, midiMessage);
                    
                    if (result == MMSYSERR_NOERROR)
                    {
                        _logger.LogInformation("MIDI mesajı gönderildi: {Message}", BitConverter.ToString(message));
                        return true;
                    }
                    else
                    {
                        _logger.LogError("MIDI mesajı gönderilemedi. Hata kodu: {ErrorCode}", result);
                        return false;
                    }
                }
                else
                {
                    // Bu bir uzun MIDI mesajı (örneğin SysEx)
                    IntPtr buffer = Marshal.AllocHGlobal(message.Length);
                    Marshal.Copy(message, 0, buffer, message.Length);

                    MIDIHDR header = new MIDIHDR();
                    header.lpData = buffer;
                    header.dwBufferLength = (uint)message.Length;
                    header.dwBytesRecorded = (uint)message.Length;
                    header.dwFlags = 0;

                    int size = Marshal.SizeOf(typeof(MIDIHDR));
                    int result = midiOutPrepareHeader(_midiOutHandle, ref header, size);
                    if (result != MMSYSERR_NOERROR)
                    {
                        Marshal.FreeHGlobal(buffer);
                        _logger.LogError("MIDI başlığı hazırlanamadı. Hata kodu: {ErrorCode}", result);
                        return false;
                    }

                    result = midiOutLongMsg(_midiOutHandle, ref header, size);
                    if (result != MMSYSERR_NOERROR)
                    {
                        midiOutUnprepareHeader(_midiOutHandle, ref header, size);
                        Marshal.FreeHGlobal(buffer);
                        _logger.LogError("SysEx mesajı gönderilemedi. Hata kodu: {ErrorCode}", result);
                        return false;
                    }

                    midiOutUnprepareHeader(_midiOutHandle, ref header, size);
                    Marshal.FreeHGlobal(buffer);

                    _logger.LogInformation("SysEx mesajı gönderildi: {Message}", BitConverter.ToString(message));
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MIDI mesajı gönderilirken hata oluştu");
                return false;
            }
        }

        #region Roland VT-4 Efekt Komutları

        /// <summary>
        /// Robot efektini tetikler (B0 31 7F / B0 31 00)
        /// </summary>
        public bool SendRobotEffect()
        {
            if (!_isInitialized)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            try
            {
                // B0 31 7F - Robot efekt aktivasyon komutu
                byte[] robotOn = new byte[] { 0xB0, 0x31, 0x7F };
                SendRawMidiMessage(robotOn);
                
                // Küçük bir gecikme
                System.Threading.Thread.Sleep(10);
                
                // B0 31 00 - Robot efekt sonlandırma komutu
                byte[] robotOff = new byte[] { 0xB0, 0x31, 0x00 };
                SendRawMidiMessage(robotOff);
                
                _logger.LogInformation("Robot efekt komutu gönderildi");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Robot efekti gönderilirken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// Harmony efektini tetikler (B0 35 7F / B0 35 00)
        /// </summary>
        public bool SendHarmonyEffect()
        {
            if (!_isInitialized)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            try
            {
                // B0 35 7F - Harmony efekt aktivasyon komutu
                byte[] harmonyOn = new byte[] { 0xB0, 0x35, 0x7F };
                SendRawMidiMessage(harmonyOn);
                
                // Küçük bir gecikme
                System.Threading.Thread.Sleep(10);
                
                // B0 35 00 - Harmony efekt sonlandırma komutu
                byte[] harmonyOff = new byte[] { 0xB0, 0x35, 0x00 };
                SendRawMidiMessage(harmonyOff);
                
                _logger.LogInformation("Harmony efekt komutu gönderildi");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Harmony efekti gönderilirken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// Megaphone efektini tetikler (B0 32 7F / B0 32 00)
        /// </summary>
        public bool SendMegaphoneEffect()
        {
            if (!_isInitialized)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            try
            {
                // B0 32 7F - Megaphone efekt aktivasyon komutu
                byte[] megaphoneOn = new byte[] { 0xB0, 0x32, 0x7F };
                SendRawMidiMessage(megaphoneOn);
                
                // Küçük bir gecikme
                System.Threading.Thread.Sleep(10);
                
                // B0 32 00 - Megaphone efekt sonlandırma komutu
                byte[] megaphoneOff = new byte[] { 0xB0, 0x32, 0x00 };
                SendRawMidiMessage(megaphoneOff);
                
                _logger.LogInformation("Megaphone efekt komutu gönderildi");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Megaphone efekti gönderilirken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// Vocoder efektini tetikler (B0 34 7F / B0 34 00)
        /// </summary>
        public bool SendVocoderEffect()
        {
            if (!_isInitialized)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            try
            {
                // B0 34 7F - Vocoder efekt aktivasyon komutu
                byte[] vocoderOn = new byte[] { 0xB0, 0x34, 0x7F };
                SendRawMidiMessage(vocoderOn);
                
                // Küçük bir gecikme
                System.Threading.Thread.Sleep(10);
                
                // B0 34 00 - Vocoder efekt sonlandırma komutu
                byte[] vocoderOff = new byte[] { 0xB0, 0x34, 0x00 };
                SendRawMidiMessage(vocoderOff);
                
                _logger.LogInformation("Vocoder efekt komutu gönderildi");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Vocoder efekti gönderilirken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// Key değerini ayarlar (B0 30 XX)
        /// </summary>
        public bool SendKeyControl(byte value)
        {
            if (!_isInitialized)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            try
            {
                // B0 30 XX - Key/Octave kontrolü
                byte[] keyControl = new byte[] { 0xB0, 0x30, value };
                SendRawMidiMessage(keyControl);
                
                _logger.LogInformation("Key kontrol komutu gönderildi: Value={Value:X2}", value);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Key değeri gönderilirken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// Formant değerini ayarlar (B0 36 XX)
        /// </summary>
        public bool SendFormantControl(byte value)
        {
            if (!_isInitialized)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            try
            {
                // B0 36 XX - Formant kontrolü
                byte[] formantControl = new byte[] { 0xB0, 0x36, value };
                SendRawMidiMessage(formantControl);
                
                _logger.LogInformation("Formant kontrol komutu gönderildi: Value={Value:X2}", value);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Formant değeri gönderilirken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// Balance değerini ayarlar (B0 38 XX)
        /// </summary>
        public bool SendBalanceControl(byte value)
        {
            if (!_isInitialized)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            try
            {
                // B0 38 XX - Balance kontrolü
                byte[] balanceControl = new byte[] { 0xB0, 0x38, value };
                SendRawMidiMessage(balanceControl);
                
                _logger.LogInformation("Balance kontrol komutu gönderildi: Value={Value:X2}", value);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Balance değeri gönderilirken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// Volume değerini ayarlar (B0 2E XX)
        /// </summary>
        public bool SendVolumeControl(byte value)
        {
            if (!_isInitialized)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            try
            {
                // B0 2E XX - Volume kontrolü
                byte[] volumeControl = new byte[] { 0xB0, 0x2E, value };
                SendRawMidiMessage(volumeControl);
                
                _logger.LogInformation("Volume kontrol komutu gönderildi: Value={Value:X2}", value);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Volume değeri gönderilirken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// Reverb değerini ayarlar (B0 39 XX)
        /// </summary>
        public bool SendReverbControl(byte value)
        {
            if (!_isInitialized)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            try
            {
                // B0 39 XX - Reverb kontrolü
                byte[] reverbControl = new byte[] { 0xB0, 0x39, value };
                SendRawMidiMessage(reverbControl);
                
                _logger.LogInformation("Reverb kontrol komutu gönderildi: Value={Value:X2}", value);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reverb değeri gönderilirken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// Program değişim komutu gönderir (C0 XX)
        /// </summary>
        public bool SendProgramChange(byte program)
        {
            if (!_isInitialized)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            try
            {
                // C0 XX - Program Change
                byte[] programChange = new byte[] { 0xC0, program };
                SendRawMidiMessage(programChange);
                
                _logger.LogInformation("Program Change komutu gönderildi: Program={Program:X2}", program);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Program Change komutu gönderilirken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// Pitch değişim komutu gönderir (E0 LL MM)
        /// </summary>
        public bool SendPitchChange(byte lsb, byte msb)
        {
            if (!_isInitialized)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            try
            {
                // E0 LL MM - Pitch Wheel Change
                byte[] pitchChange = new byte[] { 0xE0, lsb, msb };
                SendRawMidiMessage(pitchChange);
                
                _logger.LogInformation("Pitch Change komutu gönderildi: LSB={LSB:X2}, MSB={MSB:X2}", lsb, msb);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pitch Change komutu gönderilirken hata oluştu");
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Cihazın durumunu kontrol eder
        /// </summary>
        public bool IsInitialized()
        {
            return _isInitialized && _midiOutHandle != IntPtr.Zero;
        }

        /// <summary>
        /// Kaynakları temizler
        /// </summary>
        public void Dispose()
        {
            if (_isInitialized && _midiOutHandle != IntPtr.Zero)
            {
                midiOutClose(_midiOutHandle);
                _midiOutHandle = IntPtr.Zero;
                _isInitialized = false;
            }
        }
    }
}
