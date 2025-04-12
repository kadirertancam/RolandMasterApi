namespace RolandClient.Models
{
    // API modelleriyle aynı yapılar, istemci tarafında kullanılacak
    
    /// <summary>
    /// Robot efekt parametreleri
    /// </summary>
    public class RobotParameters
    {
        /// <summary>
        /// Oktav ayarı (0-3)
        /// </summary>
        public byte Octave { get; set; }

        /// <summary>
        /// Geri besleme açık/kapalı (0-1)
        /// </summary>
        public byte FeedbackSwitch { get; set; }

        /// <summary>
        /// Geri besleme rezonans değeri (0-255)
        /// </summary>
        public byte FeedbackResonance { get; set; }

        /// <summary>
        /// Geri besleme seviyesi (0-255)
        /// </summary>
        public byte FeedbackLevel { get; set; }
    }

    /// <summary>
    /// Harmony efekt parametreleri
    /// </summary>
    public class HarmonyParameters
    {
        /// <summary>
        /// Harmony 1 seviyesi (0-255)
        /// </summary>
        public byte H1Level { get; set; }

        /// <summary>
        /// Harmony 2 seviyesi (0-255)
        /// </summary>
        public byte H2Level { get; set; }

        /// <summary>
        /// Harmony 3 seviyesi (0-255)
        /// </summary>
        public byte H3Level { get; set; }

        /// <summary>
        /// Harmony 1 tonu (0-11)
        /// </summary>
        public byte H1Key { get; set; }

        /// <summary>
        /// Harmony 2 tonu (0-11)
        /// </summary>
        public byte H2Key { get; set; }

        /// <summary>
        /// Harmony 3 tonu (0-11)
        /// </summary>
        public byte H3Key { get; set; }

        /// <summary>
        /// Harmony 1 cinsiyet (0-255)
        /// </summary>
        public byte H1Gender { get; set; }

        /// <summary>
        /// Harmony 2 cinsiyet (0-255)
        /// </summary>
        public byte H2Gender { get; set; }

        /// <summary>
        /// Harmony 3 cinsiyet (0-255)
        /// </summary>
        public byte H3Gender { get; set; }
    }

    /// <summary>
    /// Megafon efekt parametreleri
    /// </summary>
    public class MegaphoneParameters
    {
        /// <summary>
        /// Megafon tipi (0-3)
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Parametre 1 (0-255)
        /// </summary>
        public byte Param1 { get; set; }

        /// <summary>
        /// Parametre 2 (0-255)
        /// </summary>
        public byte Param2 { get; set; }

        /// <summary>
        /// Parametre 3 (0-255)
        /// </summary>
        public byte Param3 { get; set; }

        /// <summary>
        /// Parametre 4 (0-255)
        /// </summary>
        public byte Param4 { get; set; }
    }

    /// <summary>
    /// Reverb efekt parametreleri
    /// </summary>
    public class ReverbParameters
    {
        /// <summary>
        /// Reverb tipi (0-5)
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Parametre 1 (0-255)
        /// </summary>
        public byte Param1 { get; set; }

        /// <summary>
        /// Parametre 2 (0-255)
        /// </summary>
        public byte Param2 { get; set; }

        /// <summary>
        /// Parametre 3 (0-255)
        /// </summary>
        public byte Param3 { get; set; }

        /// <summary>
        /// Parametre 4 (0-255)
        /// </summary>
        public byte Param4 { get; set; }
    }

    /// <summary>
    /// Vocoder efekt parametreleri
    /// </summary>
    public class VocoderParameters
    {
        /// <summary>
        /// Vocoder tipi (0-4)
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Parametre 1 (0-255)
        /// </summary>
        public byte Param1 { get; set; }

        /// <summary>
        /// Parametre 2 (0-255)
        /// </summary>
        public byte Param2 { get; set; }

        /// <summary>
        /// Parametre 3 (0-255)
        /// </summary>
        public byte Param3 { get; set; }

        /// <summary>
        /// Parametre 4 (0-255)
        /// </summary>
        public byte Param4 { get; set; }
    }

    /// <summary>
    /// Ekolayzer efekt parametreleri
    /// </summary>
    public class EqualizerParameters
    {
        /// <summary>
        /// Ekolayzer açık/kapalı (0-1)
        /// </summary>
        public byte EqSwitch { get; set; }

        /// <summary>
        /// Düşük raf frekansı (0-127)
        /// </summary>
        public byte LowShelfFreq { get; set; }

        /// <summary>
        /// Düşük raf kazancı (0-40)
        /// </summary>
        public byte LowShelfGain { get; set; }

        /// <summary>
        /// Düşük orta frekans (0-127)
        /// </summary>
        public byte LowMidFreq { get; set; }

        /// <summary>
        /// Düşük orta Q (0-127)
        /// </summary>
        public byte LowMidQ { get; set; }

        /// <summary>
        /// Düşük orta kazanç (0-40)
        /// </summary>
        public byte LowMidGain { get; set; }

        /// <summary>
        /// Yüksek orta frekans (0-127)
        /// </summary>
        public byte HighMidFreq { get; set; }

        /// <summary>
        /// Yüksek orta Q (0-127)
        /// </summary>
        public byte HighMidQ { get; set; }

        /// <summary>
        /// Yüksek orta kazanç (0-40)
        /// </summary>
        public byte HighMidGain { get; set; }

        /// <summary>
        /// Yüksek raf frekansı (0-127)
        /// </summary>
        public byte HighShelfFreq { get; set; }

        /// <summary>
        /// Yüksek raf kazancı (0-40)
        /// </summary>
        public byte HighShelfGain { get; set; }
    }
}
