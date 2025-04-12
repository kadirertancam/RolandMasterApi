namespace RolandClient.Models
{
    /// <summary>
    /// API'den alınan Roland VT-4 cihazı efekt ayarları
    /// </summary>
    public class RolandClientSettings
    {
        // Cihaz Tanımlama Bilgileri
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        
        // Genel Ayarlar
        public bool IsActive { get; set; } = true;
        public int MidiDeviceId { get; set; } = 1;
        
        // Aktif Efekt Bilgisi
        public string ActiveEffect { get; set; } = "none"; // none, robot, harmony, megaphone, reverb, vocoder, equalizer
        
        // Slider Kontrollerinin Ayarları
        public byte KeyValue { get; set; } = 0; // 0-11 (A=9, A#=10, B=11, C=0, C#=1, D=2, D#=3, E=4, F=5, F#=6, G=7, G#=8)
        public byte MicSensValue { get; set; } = 64; // 0-127
        public byte VolumeValue { get; set; } = 100; // 0-127
        public byte ReverbValue { get; set; } = 64; // 0-127
        public byte BalanceValue { get; set; } = 64; // 0-127
        public byte FormantValue { get; set; } = 64; // 0-127
        public int PitchValue { get; set; } = 8192; // 0-16383 (center at 8192)
        
        // Robot Efekt Parametreleri
        public byte RobotOctave { get; set; } = 2; // 0-3
        public byte RobotFeedbackSwitch { get; set; } = 0; // 0-1
        public byte RobotFeedbackResonance { get; set; } = 120; // 0-255
        public byte RobotFeedbackLevel { get; set; } = 160; // 0-255
        
        // Harmony Efekt Parametreleri
        public byte HarmonyH1Level { get; set; } = 200; // 0-255
        public byte HarmonyH2Level { get; set; } = 150; // 0-255
        public byte HarmonyH3Level { get; set; } = 100; // 0-255
        public byte HarmonyH1Key { get; set; } = 0; // 0-11
        public byte HarmonyH2Key { get; set; } = 4; // 0-11
        public byte HarmonyH3Key { get; set; } = 7; // 0-11
        public byte HarmonyH1Gender { get; set; } = 128; // 0-255
        public byte HarmonyH2Gender { get; set; } = 128; // 0-255
        public byte HarmonyH3Gender { get; set; } = 128; // 0-255
        
        // Megaphone Efekt Parametreleri
        public byte MegaphoneType { get; set; } = 0; // 0-3
        public byte MegaphoneParam1 { get; set; } = 100; // 0-255
        public byte MegaphoneParam2 { get; set; } = 150; // 0-255
        public byte MegaphoneParam3 { get; set; } = 200; // 0-255
        public byte MegaphoneParam4 { get; set; } = 180; // 0-255
        
        // Reverb Efekt Parametreleri
        public byte ReverbType { get; set; } = 0; // 0-5
        public byte ReverbParam1 { get; set; } = 100; // 0-255
        public byte ReverbParam2 { get; set; } = 150; // 0-255
        public byte ReverbParam3 { get; set; } = 200; // 0-255
        public byte ReverbParam4 { get; set; } = 180; // 0-255
        
        // Vocoder Efekt Parametreleri
        public byte VocoderType { get; set; } = 0; // 0-4
        public byte VocoderParam1 { get; set; } = 100; // 0-255
        public byte VocoderParam2 { get; set; } = 150; // 0-255
        public byte VocoderParam3 { get; set; } = 200; // 0-255
        public byte VocoderParam4 { get; set; } = 180; // 0-255
        
        // Equalizer Efekt Parametreleri
        public byte EqualizerSwitch { get; set; } = 1; // 0-1
        public byte EqualizerLowShelfFreq { get; set; } = 40; // 0-127
        public byte EqualizerLowShelfGain { get; set; } = 20; // 0-40
        public byte EqualizerLowMidFreq { get; set; } = 60; // 0-127
        public byte EqualizerLowMidQ { get; set; } = 70; // 0-127
        public byte EqualizerLowMidGain { get; set; } = 15; // 0-40
        public byte EqualizerHighMidFreq { get; set; } = 80; // 0-127
        public byte EqualizerHighMidQ { get; set; } = 70; // 0-127
        public byte EqualizerHighMidGain { get; set; } = 10; // 0-40
        public byte EqualizerHighShelfFreq { get; set; } = 100; // 0-127
        public byte EqualizerHighShelfGain { get; set; } = 20; // 0-40
        
        // Genel Ses Ayarları
        public int MasterVolume { get; set; } = 100; // 0-100
        public bool IsMuted { get; set; } = false;
    }
}