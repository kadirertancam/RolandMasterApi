using System;
using System.Windows.Forms;
using NAudio.Midi;

namespace RolandTest
{
    public partial class Form1 : Form
    {
        private MidiOut midiOut;

        public Form1()
        {
            InitializeComponent();
            // İlk MIDI çıkış cihazını açıyoruz (gerekiyorsa cihaz seçimi de ekleyebilirsiniz)
            try
            {
                midiOut = new MidiOut(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("MIDI cihazı açılamadı: " + ex.Message);
                Environment.Exit(1);
            }
        }

        // ------------------- Robot Efekti -------------------
        // Robot efektine ait parametrelerin kontrol edildiği örnek event handler
        private void RobotParametersChanged(object sender, EventArgs e)
        {
            // UI üzerindeki robot kontrollerinden değerleri alalım:
            // Örneğin; comboBoxRobotOctave (0-3 arası), checkBoxRobotFeedbackSwitch (0: kapalı, 1: açık),
            // trackBarRobotFeedbackResonance (0-255), trackBarRobotFeedbackLevel (0-255)
            byte octave = (byte)comboBoxRobotOctave.SelectedIndex;
            byte feedbackSwitch = checkBoxRobotFeedbackSwitch.Checked ? (byte)1 : (byte)0;
            byte feedbackResonance = (byte)trackBarRobotFeedbackResonance.Value;
            byte feedbackLevel = (byte)trackBarRobotFeedbackLevel.Value;

            byte[] sysEx = ConstructRobotSysExMessage(octave, feedbackSwitch, feedbackResonance, feedbackLevel);
            SendSysExMessage(sysEx);
        }

        // Robot efektine ait SysEx mesajı oluşturma (adres, komut ID’leri örnek değerlerdir)
        private byte[] ConstructRobotSysExMessage(byte octave, byte feedbackSwitch, byte feedbackResonance, byte feedbackLevel)
        {
            // Örnek şablon:
            // F0 41 10 00 00 00 51 12 [Address: 00 00 00 30] [Data Length: 04] [octave, fbSwitch, fbResonance, fbLevel] [Checksum] F7
            byte[] message = new byte[]
            {
                0xF0,
                0x41,       // Roland üretici ID
                0x10,       // Cihaz ID
                0x00, 0x00, 0x00, // Model ID (örnek)
                0x51,
                0x12,       // Robot efekt komut ID (örnek)
                0x00, 0x00, 0x00, 0x30, // Adres (örnek, PDF’den uyarlayın)
                0x04,       // Veri uzunluğu (4 byte)
                octave,
                feedbackSwitch,
                feedbackResonance,
                feedbackLevel,
                0x00,       // Checksum (placeholder)
                0xF7
            };
            // Checksum hesaplaması: adres (4 byte) + veri uzunluğu + veri (4 byte) => toplam 9 byte
            message[message.Length - 2] = CalculateRolandChecksum(message, 8, 9);
            return message;
        }

        // ------------------- Harmony Efekti -------------------
        // Harmony için üç kanala ait parametreler; bu örnekte tüm değerler toplu olarak gönderiliyor.
        private void btnApplyHarmony_Click(object sender, EventArgs e)
        {
            byte h1Level = (byte)trackBarHarmony1Level.Value;
            byte h2Level = (byte)trackBarHarmony2Level.Value;
            byte h3Level = (byte)trackBarHarmony3Level.Value;
            byte h1Key = (byte)comboBoxHarmony1Key.SelectedIndex; // 0-10 arası
            byte h2Key = (byte)comboBoxHarmony2Key.SelectedIndex;
            byte h3Key = (byte)comboBoxHarmony3Key.SelectedIndex;
            byte h1Gender = (byte)trackBarHarmony1Gender.Value;
            byte h2Gender = (byte)trackBarHarmony2Gender.Value;
            byte h3Gender = (byte)trackBarHarmony3Gender.Value;

            byte[] sysEx = ConstructHarmonySysExMessage(h1Level, h2Level, h3Level, h1Key, h2Key, h3Key, h1Gender, h2Gender, h3Gender);
            SendSysExMessage(sysEx);
        }

        private byte[] ConstructHarmonySysExMessage(byte h1Level, byte h2Level, byte h3Level,
                                                      byte h1Key, byte h2Key, byte h3Key,
                                                      byte h1Gender, byte h2Gender, byte h3Gender)
        {
            // Örnek şablon:
            // F0 41 10 00 00 00 51 13 [Address: 31 00 00 00] [Data Length: 09] 
            // [h1Level, h2Level, h3Level, h1Key, h2Key, h3Key, h1Gender, h2Gender, h3Gender] [Checksum] F7
            byte[] message = new byte[]
            {
                0xF0,
                0x41,
                0x10,
                0x00, 0x00, 0x00,
                0x51,
                0x13,       // Harmony efekt komut ID (örnek)
                0x31, 0x00, 0x00, 0x00, // Adres (örnek)
                0x09,       // Veri uzunluğu: 9 byte
                h1Level,
                h2Level,
                h3Level,
                h1Key,
                h2Key,
                h3Key,
                h1Gender,
                h2Gender,
                h3Gender,
                0x00,       // Checksum placeholder
                0xF7
            };
            message[message.Length - 2] = CalculateRolandChecksum(message, 8, 14); // 4 adres + 1 uzunluk + 9 veri = 14 byte
            return message;
        }

        // ------------------- Megaphone Efekti -------------------
        private void MegaphoneParametersChanged(object sender, EventArgs e)
        {
            byte type = (byte)comboBoxMegaphoneType.SelectedIndex; // 0-3 arası
            byte param1 = (byte)trackBarMegaphoneParam1.Value;
            byte param2 = (byte)trackBarMegaphoneParam2.Value;
            byte param3 = (byte)trackBarMegaphoneParam3.Value;
            byte param4 = (byte)trackBarMegaphoneParam4.Value;

            byte[] sysEx = ConstructMegaphoneSysExMessage(type, param1, param2, param3, param4);
            SendSysExMessage(sysEx);
        }

        private byte[] ConstructMegaphoneSysExMessage(byte type, byte param1, byte param2, byte param3, byte param4)
        {
            // Örnek şablon:
            // F0 41 10 00 00 00 51 14 [Address: 41 00 00 00] [Data Length: 05]
            // [type, param1, param2, param3, param4] [Checksum] F7
            byte[] message = new byte[]
            {
                0xF0,
                0x41,
                0x10,
                0x00, 0x00, 0x00,
                0x51,
                0x14,       // Megaphone efekt komut ID (örnek)
                0x41, 0x00, 0x00, 0x00, // Adres (örnek)
                0x05,       // Veri uzunluğu: 5 byte
                type,
                param1,
                param2,
                param3,
                param4,
                0x00,       // Checksum placeholder
                0xF7
            };
            message[message.Length - 2] = CalculateRolandChecksum(message, 8, 10);
            return message;
        }

        // ------------------- Reverb Efekti -------------------
        private void ReverbParametersChanged(object sender, EventArgs e)
        {
            byte type = (byte)comboBoxReverbType.SelectedIndex; // Örneğin 0-5 arası
            byte param1 = (byte)trackBarReverbParam1.Value;
            byte param2 = (byte)trackBarReverbParam2.Value;
            byte param3 = (byte)trackBarReverbParam3.Value;
            byte param4 = (byte)trackBarReverbParam4.Value;

            byte[] sysEx = ConstructReverbSysExMessage(type, param1, param2, param3, param4);
            SendSysExMessage(sysEx);
        }

        private byte[] ConstructReverbSysExMessage(byte type, byte param1, byte param2, byte param3, byte param4)
        {
            // Örnek şablon:
            // F0 41 10 00 00 00 51 15 [Address: 51 00 00 00] [Data Length: 05]
            // [type, param1, param2, param3, param4] [Checksum] F7
            byte[] message = new byte[]
            {
                0xF0,
                0x41,
                0x10,
                0x00, 0x00, 0x00,
                0x51,
                0x15,       // Reverb efekt komut ID (örnek)
                0x51, 0x00, 0x00, 0x00, // Adres (örnek)
                0x05,       // Veri uzunluğu: 5 byte
                type,
                param1,
                param2,
                param3,
                param4,
                0x00,       // Checksum placeholder
                0xF7
            };
            message[message.Length - 2] = CalculateRolandChecksum(message, 8, 10);
            return message;
        }

        // ------------------- Vocoder Efekti -------------------
        private void VocoderParametersChanged(object sender, EventArgs e)
        {
            byte type = (byte)comboBoxVocoderType.SelectedIndex; // 0-4 arası
            byte param1 = (byte)trackBarVocoderParam1.Value;
            byte param2 = (byte)trackBarVocoderParam2.Value;
            byte param3 = (byte)trackBarVocoderParam3.Value;
            byte param4 = (byte)trackBarVocoderParam4.Value;

            byte[] sysEx = ConstructVocoderSysExMessage(type, param1, param2, param3, param4);
            SendSysExMessage(sysEx);
        }

        private byte[] ConstructVocoderSysExMessage(byte type, byte param1, byte param2, byte param3, byte param4)
        {
            // Örnek şablon:
            // F0 41 10 00 00 00 51 16 [Address: 61 00 00 00] [Data Length: 05]
            // [type, param1, param2, param3, param4] [Checksum] F7
            byte[] message = new byte[]
            {
                0xF0,
                0x41,
                0x10,
                0x00, 0x00, 0x00,
                0x51,
                0x16,       // Vocoder efekt komut ID (örnek)
                0x61, 0x00, 0x00, 0x00, // Adres (örnek)
                0x05,       // Veri uzunluğu: 5 byte
                type,
                param1,
                param2,
                param3,
                param4,
                0x00,       // Checksum placeholder
                0xF7
            };
            message[message.Length - 2] = CalculateRolandChecksum(message, 8, 10);
            return message;
        }

        // ------------------- Equalizer Efekti -------------------
        private void btnApplyEqualizer_Click(object sender, EventArgs e)
        {
            byte eqSwitch = checkBoxEqualizer.Checked ? (byte)1 : (byte)0;
            byte lowShelfFreq = (byte)trackBarLowShelfFreq.Value;
            byte lowShelfGain = (byte)trackBarLowShelfGain.Value;
            byte lowMidFreq = (byte)trackBarLowMidFreq.Value;
            byte lowMidQ = (byte)trackBarLowMidQ.Value;
            byte lowMidGain = (byte)trackBarLowMidGain.Value;
            byte highMidFreq = (byte)trackBarHighMidFreq.Value;
            byte highMidQ = (byte)trackBarHighMidQ.Value;
            byte highMidGain = (byte)trackBarHighMidGain.Value;
            byte highShelfFreq = (byte)trackBarHighShelfFreq.Value;
            byte highShelfGain = (byte)trackBarHighShelfGain.Value;

            byte[] sysEx = ConstructEqualizerSysExMessage(eqSwitch, lowShelfFreq, lowShelfGain,
                                                            lowMidFreq, lowMidQ, lowMidGain,
                                                            highMidFreq, highMidQ, highMidGain,
                                                            highShelfFreq, highShelfGain);
            SendSysExMessage(sysEx);
        }

        private byte[] ConstructEqualizerSysExMessage(byte eqSwitch, byte lowShelfFreq, byte lowShelfGain,
                                                       byte lowMidFreq, byte lowMidQ, byte lowMidGain,
                                                       byte highMidFreq, byte highMidQ, byte highMidGain,
                                                       byte highShelfFreq, byte highShelfGain)
        {
            // Örnek şablon:
            // F0 41 10 00 00 00 51 17 [Address: 63 00 00 00] [Data Length: 0B]
            // [eqSwitch, lowShelfFreq, lowShelfGain, lowMidFreq, lowMidQ, lowMidGain,
            //  highMidFreq, highMidQ, highMidGain, highShelfFreq, highShelfGain] [Checksum] F7
            byte[] message = new byte[]
            {
                0xF0,
                0x41,
                0x10,
                0x00, 0x00, 0x00,
                0x51,
                0x17,       // Equalizer efekt komut ID (örnek)
                0x63, 0x00, 0x00, 0x00, // Adres (örnek)
                0x0B,       // Veri uzunluğu: 11 byte
                eqSwitch,
                lowShelfFreq,
                lowShelfGain,
                lowMidFreq,
                lowMidQ,
                lowMidGain,
                highMidFreq,
                highMidQ,
                highMidGain,
                highShelfFreq,
                highShelfGain,
                0x00,       // Checksum placeholder
                0xF7
            };
            message[message.Length - 2] = CalculateRolandChecksum(message, 8, 4 + 1 + 11);
            return message;
        }

        // ------------------- Yardımcı Metodlar -------------------
        // Roland checksum hesaplama: adres + veri uzunluğu + veri byte’ları toplamı üzerinden
        private byte CalculateRolandChecksum(byte[] data, int start, int count)
        {
            int sum = 0;
            for (int i = start; i < start + count; i++)
            {
                sum += data[i];
            }
            return (byte)((128 - (sum % 128)) % 128);
        }

        // Oluşturulan SysEx mesajını MIDI çıkış cihazına gönderir.
        private void SendSysExMessage(byte[] sysExMessage)
        {
            try
            {
                // NAudio'nın SendBuffer metodunu kullanarak mesajı gönderiyoruz.
                midiOut.SendBuffer(sysExMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mesaj gönderilemedi: " + ex.Message);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            midiOut?.Dispose();
            base.OnFormClosing(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
