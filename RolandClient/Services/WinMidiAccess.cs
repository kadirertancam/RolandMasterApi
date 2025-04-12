using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;

namespace RolandClient.Services
{
    /// <summary>
    /// Windows MIDI API'sine doğrudan erişim sağlayan sınıf
    /// </summary>
    public class WinMidiAccess : IDisposable
    {
        private readonly ILogger<WinMidiAccess> _logger;
        private IntPtr midiOutHandle = IntPtr.Zero;
        private bool isInitialized = false;

        // Windows API için sabitler
        private const int MMSYSERR_NOERROR = 0;
        private const int CALLBACK_NULL = 0;

        // Windows MIDI fonksiyonlarının dışa aktarılması
        [DllImport("winmm.dll")]
        private static extern int midiOutGetNumDevs();

        [DllImport("winmm.dll")]
        private static extern int midiOutGetDevCaps(int deviceID, ref MIDIOUTCAPS caps, int sizeOfMidiOutCaps);

        [DllImport("winmm.dll")]
        private static extern int midiOutOpen(ref IntPtr handle, int deviceID, IntPtr callback, IntPtr instance, int flags);

        [DllImport("winmm.dll")]
        private static extern int midiOutClose(IntPtr handle);

        [DllImport("winmm.dll")]
        private static extern int midiOutShortMsg(IntPtr handle, int message);

        [DllImport("winmm.dll")]
        private static extern int midiOutLongMsg(IntPtr handle, ref MIDIHDR header, int sizeOfMidiHeader);

        [DllImport("winmm.dll")]
        private static extern int midiOutPrepareHeader(IntPtr handle, ref MIDIHDR header, int sizeOfMidiHeader);

        [DllImport("winmm.dll")]
        private static extern int midiOutUnprepareHeader(IntPtr handle, ref MIDIHDR header, int sizeOfMidiHeader);

        // Windows MIDI yapıları
        [StructLayout(LayoutKind.Sequential)]
        private struct MIDIOUTCAPS
        {
            public short wMid;
            public short wPid;
            public int vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public short wTechnology;
            public short wVoices;
            public short wNotes;
            public short wChannelMask;
            public int dwSupport;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MIDIHDR
        {
            public IntPtr lpData;
            public int dwBufferLength;
            public int dwBytesRecorded;
            public IntPtr dwUser;
            public int dwFlags;
            public IntPtr lpNext;
            public IntPtr reserved;
            public int dwOffset;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public IntPtr[] dwReserved;
        }

        public WinMidiAccess(ILogger<WinMidiAccess> logger)
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
        /// MIDI cihazını açar
        /// </summary>
        public bool Initialize(int deviceId = 0)
        {
            try
            {
                if (isInitialized)
                {
                    _logger.LogWarning("MIDI çıkışı zaten başlatılmış");
                    return true;
                }

                int result = midiOutOpen(ref midiOutHandle, deviceId, IntPtr.Zero, IntPtr.Zero, CALLBACK_NULL);
                if (result == MMSYSERR_NOERROR)
                {
                    isInitialized = true;
                    _logger.LogInformation("MIDI çıkışı başarıyla başlatıldı");
                    return true;
                }
                else
                {
                    _logger.LogError("MIDI çıkışı başlatılamadı. Hata kodu: {ErrorCode}", result);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MIDI çıkışı başlatılırken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// Kısa MIDI mesajı gönderir (Control Change, Note On/Off, Program Change, vb.)
        /// </summary>
        public bool SendShortMessage(byte status, byte data1, byte data2)
        {
            if (!isInitialized || midiOutHandle == IntPtr.Zero)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            try
            {
                // Kısa MIDI mesajı oluşturma: status | (data1 << 8) | (data2 << 16)
                int message = status | (data1 << 8) | (data2 << 16);
                int result = midiOutShortMsg(midiOutHandle, message);

                if (result == MMSYSERR_NOERROR)
                {
                    _logger.LogInformation("MIDI mesajı gönderildi: {Status:X2} {Data1:X2} {Data2:X2}", status, data1, data2);
                    return true;
                }
                else
                {
                    _logger.LogError("MIDI mesajı gönderilemedi. Hata kodu: {ErrorCode}", result);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MIDI mesajı gönderilirken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// SysEx (System Exclusive) MIDI mesajı gönderir
        /// </summary>
        public bool SendSysExMessage(byte[] data)
        {
            if (!isInitialized || midiOutHandle == IntPtr.Zero)
            {
                _logger.LogError("MIDI cihazı başlatılmadı");
                return false;
            }

            if (data == null || data.Length == 0)
            {
                _logger.LogError("Geçersiz SysEx verisi");
                return false;
            }

            try
            {
                // Veriyi yönetilen hafızada ayır
                IntPtr buffer = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, buffer, data.Length);

                // MIDI başlığını oluştur
                MIDIHDR header = new MIDIHDR
                {
                    lpData = buffer,
                    dwBufferLength = data.Length,
                    dwBytesRecorded = data.Length,
                    dwFlags = 0
                };

                // Başlığı hazırla
                int size = Marshal.SizeOf(typeof(MIDIHDR));
                int result = midiOutPrepareHeader(midiOutHandle, ref header, size);
                if (result != MMSYSERR_NOERROR)
                {
                    Marshal.FreeHGlobal(buffer);
                    _logger.LogError("MIDI başlığı hazırlanamadı. Hata kodu: {ErrorCode}", result);
                    return false;
                }

                // SysEx mesajını gönder
                result = midiOutLongMsg(midiOutHandle, ref header, size);
                if (result != MMSYSERR_NOERROR)
                {
                    midiOutUnprepareHeader(midiOutHandle, ref header, size);
                    Marshal.FreeHGlobal(buffer);
                    _logger.LogError("SysEx mesajı gönderilemedi. Hata kodu: {ErrorCode}", result);
                    return false;
                }

                // Başlığı temizle
                result = midiOutUnprepareHeader(midiOutHandle, ref header, size);
                if (result != MMSYSERR_NOERROR)
                {
                    _logger.LogWarning("MIDI başlığı temizlenemedi. Hata kodu: {ErrorCode}", result);
                }

                // Kaynakları serbest bırak
                Marshal.FreeHGlobal(buffer);

                _logger.LogInformation("SysEx mesajı başarıyla gönderildi. Uzunluk: {Length}", data.Length);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SysEx mesajı gönderilirken hata oluştu");
                return false;
            }
        }

        /// <summary>
        /// Control Change (CC) mesajı gönderir - B0 nn vv
        /// </summary>
        public bool SendControlChange(byte channel, byte controller, byte value)
        {
            if (channel > 15)
            {
                _logger.LogError("Geçersiz MIDI kanalı: {Channel}. Kanal 0-15 arasında olmalıdır", channel);
                return false;
            }

            // Control Change status byte: 0xB0 + channel
            byte status = (byte)(0xB0 | channel);
            return SendShortMessage(status, controller, value);
        }

        /// <summary>
        /// Program Change mesajı gönderir - C0 pp
        /// </summary>
        public bool SendProgramChange(byte channel, byte program)
        {
            if (channel > 15)
            {
                _logger.LogError("Geçersiz MIDI kanalı: {Channel}. Kanal 0-15 arasında olmalıdır", channel);
                return false;
            }

            // Program Change status byte: 0xC0 + channel
            byte status = (byte)(0xC0 | channel);
            return SendShortMessage(status, program, 0);
        }

        /// <summary>
        /// Pitch Wheel mesajı gönderir - E0 ll mm
        /// </summary>
        public bool SendPitchWheel(byte channel, int value)
        {
            if (channel > 15)
            {
                _logger.LogError("Geçersiz MIDI kanalı: {Channel}. Kanal 0-15 arasında olmalıdır", channel);
                return false;
            }

            if (value < 0 || value > 16383)
            {
                _logger.LogError("Geçersiz pitch wheel değeri: {Value}. Değer 0-16383 arasında olmalıdır", value);
                return false;
            }

            // Pitch Wheel status byte: 0xE0 + channel
            byte status = (byte)(0xE0 | channel);
            byte lsb = (byte)(value & 0x7F);         // Düşük 7 bit
            byte msb = (byte)((value >> 7) & 0x7F);  // Yüksek 7 bit

            return SendShortMessage(status, lsb, msb);
        }

        /// <summary>
        /// Robot efekti için MIDI komutlarını gönderir
        /// </summary>
        public bool SendRobotEffect()
        {
            bool success = true;

            // B0 31 7F / Robot etkisini aktifleştir
            success &= SendControlChange(0, 0x31, 0x7F);
            
            // Küçük bir gecikme
            System.Threading.Thread.Sleep(10);
            
            // B0 31 00 / Robot komutu tamamla
            success &= SendControlChange(0, 0x31, 0x00);

            return success;
        }

        /// <summary>
        /// Harmony efekti için MIDI komutlarını gönderir
        /// </summary>
        public bool SendHarmonyEffect()
        {
            bool success = true;

            // B0 35 7F / Harmony etkisini aktifleştir
            success &= SendControlChange(0, 0x35, 0x7F);
            
            // Küçük bir gecikme
            System.Threading.Thread.Sleep(10);
            
            // B0 35 00 / Harmony komutu tamamla
            success &= SendControlChange(0, 0x35, 0x00);

            return success;
        }

        /// <summary>
        /// Megaphone efekti için MIDI komutlarını gönderir
        /// </summary>
        public bool SendMegaphoneEffect()
        {
            bool success = true;

            // B0 32 7F / Megaphone etkisini aktifleştir
            success &= SendControlChange(0, 0x32, 0x7F);
            
            // Küçük bir gecikme
            System.Threading.Thread.Sleep(10);
            
            // B0 32 00 / Megaphone komutu tamamla
            success &= SendControlChange(0, 0x32, 0x00);

            return success;
        }

        /// <summary>
        /// Vocoder efekti için MIDI komutlarını gönderir
        /// </summary>
        public bool SendVocoderEffect()
        {
            bool success = true;

            // B0 34 7F / Vocoder etkisini aktifleştir
            success &= SendControlChange(0, 0x34, 0x7F);
            
            // Küçük bir gecikme
            System.Threading.Thread.Sleep(10);
            
            // B0 34 00 / Vocoder komutu tamamla
            success &= SendControlChange(0, 0x34, 0x00);

            return success;
        }

        /// <summary>
        /// Key (B0 30 XX) değerini ayarlar
        /// </summary>
        public bool SendKeyControl(byte value)
        {
            return SendControlChange(0, 0x30, value);
        }

        /// <summary>
        /// Formant (B0 36 XX) değerini ayarlar
        /// </summary>
        public bool SendFormantControl(byte value)
        {
            return SendControlChange(0, 0x36, value);
        }

        /// <summary>
        /// Balance (B0 38 XX) değerini ayarlar
        /// </summary>
        public bool SendBalanceControl(byte value)
        {
            return SendControlChange(0, 0x38, value);
        }

        /// <summary>
        /// Volume (B0 2E XX) değerini ayarlar
        /// </summary>
        public bool SendVolumeControl(byte value)
        {
            return SendControlChange(0, 0x2E, value);
        }

        /// <summary>
        /// Reverb (B0 39 XX) değerini ayarlar
        /// </summary>
        public bool SendReverbControl(byte value)
        {
            return SendControlChange(0, 0x39, value);
        }

        /// <summary>
        /// Kaynakları temizler
        /// </summary>
        public void Dispose()
        {
            if (isInitialized && midiOutHandle != IntPtr.Zero)
            {
                midiOutClose(midiOutHandle);
                midiOutHandle = IntPtr.Zero;
                isInitialized = false;
            }
        }

        /// <summary>
        /// Cihazın başlatılıp başlatılmadığını kontrol eder
        /// </summary>
        public bool IsInitialized()
        {
            return isInitialized && midiOutHandle != IntPtr.Zero;
        }
    }
}
