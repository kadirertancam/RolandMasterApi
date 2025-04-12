namespace RolandTestCore.Services
{
    /// <summary>
    /// Interface for MIDI service operations
    /// </summary>
    public interface IMidiService
    {
        /// <summary>
        /// Initializes MIDI output device
        /// </summary>
        /// <param name="deviceId">MIDI device ID to use (default: 1)</param>
        void Initialize(int deviceId = 1);

        /// <summary>
        /// Gets a list of available MIDI output devices
        /// </summary>
        /// <returns>List of device names and IDs</returns>
        List<(int Id, string Name)> GetAvailableMidiDevices();

        /// <summary>
        /// Sends a System Exclusive message to the MIDI device
        /// </summary>
        /// <param name="sysExMessage">The System Exclusive message to send</param>
        void SendSysExMessage(byte[] sysExMessage);

        /// <summary>
        /// Creates and sends a Robot effect message
        /// </summary>
        /// <param name="octave">Octave value (0-3)</param>
        /// <param name="feedbackSwitch">Feedback on/off (0-1)</param>
        /// <param name="feedbackResonance">Feedback resonance (0-255)</param>
        /// <param name="feedbackLevel">Feedback level (0-255)</param>
        void SendRobotEffect(byte octave, byte feedbackSwitch, byte feedbackResonance, byte feedbackLevel);

        /// <summary>
        /// Creates and sends a Harmony effect message
        /// </summary>
        /// <param name="h1Level">Harmony 1 level (0-255)</param>
        /// <param name="h2Level">Harmony 2 level (0-255)</param>
        /// <param name="h3Level">Harmony 3 level (0-255)</param>
        /// <param name="h1Key">Harmony 1 key (0-11)</param>
        /// <param name="h2Key">Harmony 2 key (0-11)</param>
        /// <param name="h3Key">Harmony 3 key (0-11)</param>
        /// <param name="h1Gender">Harmony 1 gender (0-255)</param>
        /// <param name="h2Gender">Harmony 2 gender (0-255)</param>
        /// <param name="h3Gender">Harmony 3 gender (0-255)</param>
        void SendHarmonyEffect(byte h1Level, byte h2Level, byte h3Level,
                               byte h1Key, byte h2Key, byte h3Key,
                               byte h1Gender, byte h2Gender, byte h3Gender);

        /// <summary>
        /// Creates and sends a Megaphone effect message
        /// </summary>
        /// <param name="type">Megaphone type (0-3)</param>
        /// <param name="param1">Parameter 1 (0-255)</param>
        /// <param name="param2">Parameter 2 (0-255)</param>
        /// <param name="param3">Parameter 3 (0-255)</param>
        /// <param name="param4">Parameter 4 (0-255)</param>
        void SendMegaphoneEffect(byte type, byte param1, byte param2, byte param3, byte param4);

        /// <summary>
        /// Creates and sends a Reverb effect message
        /// </summary>
        /// <param name="type">Reverb type (0-5)</param>
        /// <param name="param1">Parameter 1 (0-255)</param>
        /// <param name="param2">Parameter 2 (0-255)</param>
        /// <param name="param3">Parameter 3 (0-255)</param>
        /// <param name="param4">Parameter 4 (0-255)</param>
        void SendReverbEffect(byte type, byte param1, byte param2, byte param3, byte param4);

        /// <summary>
        /// Creates and sends a Vocoder effect message
        /// </summary>
        /// <param name="type">Vocoder type (0-4)</param>
        /// <param name="param1">Parameter 1 (0-255)</param>
        /// <param name="param2">Parameter 2 (0-255)</param>
        /// <param name="param3">Parameter 3 (0-255)</param>
        /// <param name="param4">Parameter 4 (0-255)</param>
        void SendVocoderEffect(byte type, byte param1, byte param2, byte param3, byte param4);

        /// <summary>
        /// Creates and sends an Equalizer effect message
        /// </summary>
        /// <param name="eqSwitch">Equalizer on/off (0-1)</param>
        /// <param name="lowShelfFreq">Low shelf frequency (0-127)</param>
        /// <param name="lowShelfGain">Low shelf gain (0-40)</param>
        /// <param name="lowMidFreq">Low mid frequency (0-127)</param>
        /// <param name="lowMidQ">Low mid Q (0-127)</param>
        /// <param name="lowMidGain">Low mid gain (0-40)</param>
        /// <param name="highMidFreq">High mid frequency (0-127)</param>
        /// <param name="highMidQ">High mid Q (0-127)</param>
        /// <param name="highMidGain">High mid gain (0-40)</param>
        /// <param name="highShelfFreq">High shelf frequency (0-127)</param>
        /// <param name="highShelfGain">High shelf gain (0-40)</param>
        void SendEqualizerEffect(byte eqSwitch, byte lowShelfFreq, byte lowShelfGain,
                                byte lowMidFreq, byte lowMidQ, byte lowMidGain,
                                byte highMidFreq, byte highMidQ, byte highMidGain,
                                byte highShelfFreq, byte highShelfGain);

        /// <summary>
        /// Disposes of MIDI resources
        /// </summary>
        void Dispose();
    }
}
