namespace RolandClient.Services
{
    /// <summary>
    /// MIDI servis arayüzü
    /// </summary>
    public interface IMidiService
    {
        /// <summary>
        /// MIDI cihazını başlatır
        /// </summary>
        /// <param name="deviceId">MIDI cihaz ID'si</param>
        /// <returns>Başarılı olup olmadığı</returns>
        Task<bool> InitializeAsync(int deviceId);
        
        /// <summary>
        /// Robot efektini uygular
        /// </summary>
        /// <param name="octave">Oktav</param>
        /// <param name="feedbackSwitch">Geri besleme açık/kapalı</param>
        /// <param name="feedbackResonance">Geri besleme rezonansı</param>
        /// <param name="feedbackLevel">Geri besleme seviyesi</param>
        /// <returns>Başarılı olup olmadığı</returns>
        Task<bool> ApplyRobotEffectAsync(byte octave, byte feedbackSwitch, byte feedbackResonance, byte feedbackLevel);
        
        /// <summary>
        /// Harmony efektini uygular
        /// </summary>
        Task<bool> ApplyHarmonyEffectAsync(byte h1Level, byte h2Level, byte h3Level,
                                  byte h1Key, byte h2Key, byte h3Key,
                                  byte h1Gender, byte h2Gender, byte h3Gender);
        
        /// <summary>
        /// Megafon efektini uygular
        /// </summary>
        Task<bool> ApplyMegaphoneEffectAsync(byte type, byte param1, byte param2, byte param3, byte param4);
        
        /// <summary>
        /// Reverb efektini uygular
        /// </summary>
        Task<bool> ApplyReverbEffectAsync(byte type, byte param1, byte param2, byte param3, byte param4);
        
        /// <summary>
        /// Vocoder efektini uygular
        /// </summary>
        Task<bool> ApplyVocoderEffectAsync(byte type, byte param1, byte param2, byte param3, byte param4);
        
        /// <summary>
        /// Ekolayzer efektini uygular
        /// </summary>
        Task<bool> ApplyEqualizerEffectAsync(byte eqSwitch, byte lowShelfFreq, byte lowShelfGain,
                                   byte lowMidFreq, byte lowMidQ, byte lowMidGain,
                                   byte highMidFreq, byte highMidQ, byte highMidGain,
                                   byte highShelfFreq, byte highShelfGain);
    }
}
