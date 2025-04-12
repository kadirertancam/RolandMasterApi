using NAudio.Midi;

namespace RolandTestCore.Services
{
    /// <summary>
    /// Implementation of the MIDI service for Roland VT-4
    /// </summary>
    public class MidiService : IMidiService, IDisposable
    {
        private MidiOut? midiOut;
        private bool isInitialized = false;
        private readonly ILogger<MidiService> _logger;

        public MidiService(ILogger<MidiService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Initializes the MIDI output device
        /// </summary>
        /// <param name="deviceId">MIDI device ID to use</param>
        public void Initialize(int deviceId = 1)
        {
            try
            {
                // Check if there are any MIDI devices available
                if (MidiOut.NumberOfDevices == 0)
                {
                    _logger.LogWarning("No MIDI output devices found");
                    throw new InvalidOperationException("No MIDI output devices found");
                }

                // Validate device ID
                if (deviceId < 0 || deviceId >= MidiOut.NumberOfDevices)
                {
                    _logger.LogWarning($"Invalid MIDI device ID: {deviceId}. Valid range: 0-{MidiOut.NumberOfDevices - 1}");
                    throw new ArgumentException($"Invalid MIDI device ID: {deviceId}. Valid range: 0-{MidiOut.NumberOfDevices - 1}");
                }

                // Close any existing MIDI device
                midiOut?.Dispose();

                // Open the selected MIDI device
                midiOut = new MidiOut(deviceId);
                isInitialized = true;

                _logger.LogInformation($"MIDI device initialized: {MidiOut.DeviceInfo(deviceId).ProductName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize MIDI device");
                throw;
            }
        }

        /// <summary>
        /// Gets a list of available MIDI output devices
        /// </summary>
        /// <returns>List of device names and IDs</returns>
        public List<(int Id, string Name)> GetAvailableMidiDevices()
        {
            var devices = new List<(int Id, string Name)>();
            for (int i = 0; i < MidiOut.NumberOfDevices; i++)
            {
                devices.Add((i, MidiOut.DeviceInfo(i).ProductName));
            }
            return devices;
        }

        /// <summary>
        /// Sends a System Exclusive message to the MIDI device
        /// </summary>
        /// <param name="sysExMessage">The System Exclusive message to send</param>
        public void SendSysExMessage(byte[] sysExMessage)
        {
            EnsureInitialized();

            try
            {
                midiOut!.SendBuffer(sysExMessage);
                _logger.LogDebug($"SysEx message sent: {BitConverter.ToString(sysExMessage)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SysEx message");
                throw;
            }
        }

        #region Effect Methods

        /// <summary>
        /// Creates and sends a Robot effect message
        /// </summary>
        public void SendRobotEffect(byte octave, byte feedbackSwitch, byte feedbackResonance, byte feedbackLevel)
        {
            // Validate parameters
            if (octave > 3) throw new ArgumentOutOfRangeException(nameof(octave), "Value must be between 0-3");
            if (feedbackSwitch > 1) throw new ArgumentOutOfRangeException(nameof(feedbackSwitch), "Value must be 0 or 1");

            byte[] sysEx = ConstructRobotSysExMessage(octave, feedbackSwitch, feedbackResonance, feedbackLevel);
            SendSysExMessage(sysEx);
        }

        /// <summary>
        /// Creates and sends a Harmony effect message
        /// </summary>
        public void SendHarmonyEffect(byte h1Level, byte h2Level, byte h3Level,
                               byte h1Key, byte h2Key, byte h3Key,
                               byte h1Gender, byte h2Gender, byte h3Gender)
        {
            // Validate parameters
            if (h1Key > 11) throw new ArgumentOutOfRangeException(nameof(h1Key), "Value must be between 0-11");
            if (h2Key > 11) throw new ArgumentOutOfRangeException(nameof(h2Key), "Value must be between 0-11");
            if (h3Key > 11) throw new ArgumentOutOfRangeException(nameof(h3Key), "Value must be between 0-11");

            byte[] sysEx = ConstructHarmonySysExMessage(h1Level, h2Level, h3Level, h1Key, h2Key, h3Key, h1Gender, h2Gender, h3Gender);
            SendSysExMessage(sysEx);
        }

        /// <summary>
        /// Creates and sends a Megaphone effect message
        /// </summary>
        public void SendMegaphoneEffect(byte type, byte param1, byte param2, byte param3, byte param4)
        {
            // Validate parameters
            if (type > 3) throw new ArgumentOutOfRangeException(nameof(type), "Value must be between 0-3");

            byte[] sysEx = ConstructMegaphoneSysExMessage(type, param1, param2, param3, param4);
            SendSysExMessage(sysEx);
        }

        /// <summary>
        /// Creates and sends a Reverb effect message
        /// </summary>
        public void SendReverbEffect(byte type, byte param1, byte param2, byte param3, byte param4)
        {
            // Validate parameters
            if (type > 5) throw new ArgumentOutOfRangeException(nameof(type), "Value must be between 0-5");

            byte[] sysEx = ConstructReverbSysExMessage(type, param1, param2, param3, param4);
            SendSysExMessage(sysEx);
        }

        /// <summary>
        /// Creates and sends a Vocoder effect message
        /// </summary>
        public void SendVocoderEffect(byte type, byte param1, byte param2, byte param3, byte param4)
        {
            // Validate parameters
            if (type > 4) throw new ArgumentOutOfRangeException(nameof(type), "Value must be between 0-4");

            byte[] sysEx = ConstructVocoderSysExMessage(type, param1, param2, param3, param4);
            SendSysExMessage(sysEx);
        }

        /// <summary>
        /// Creates and sends an Equalizer effect message
        /// </summary>
        public void SendEqualizerEffect(byte eqSwitch, byte lowShelfFreq, byte lowShelfGain,
                                byte lowMidFreq, byte lowMidQ, byte lowMidGain,
                                byte highMidFreq, byte highMidQ, byte highMidGain,
                                byte highShelfFreq, byte highShelfGain)
        {
            // Validate parameters
            if (eqSwitch > 1) throw new ArgumentOutOfRangeException(nameof(eqSwitch), "Value must be 0 or 1");
            if (lowShelfFreq > 127) throw new ArgumentOutOfRangeException(nameof(lowShelfFreq), "Value must be between 0-127");
            if (lowShelfGain > 40) throw new ArgumentOutOfRangeException(nameof(lowShelfGain), "Value must be between 0-40");
            if (lowMidFreq > 127) throw new ArgumentOutOfRangeException(nameof(lowMidFreq), "Value must be between 0-127");
            if (lowMidQ > 127) throw new ArgumentOutOfRangeException(nameof(lowMidQ), "Value must be between 0-127");
            if (lowMidGain > 40) throw new ArgumentOutOfRangeException(nameof(lowMidGain), "Value must be between 0-40");
            if (highMidFreq > 127) throw new ArgumentOutOfRangeException(nameof(highMidFreq), "Value must be between 0-127");
            if (highMidQ > 127) throw new ArgumentOutOfRangeException(nameof(highMidQ), "Value must be between 0-127");
            if (highMidGain > 40) throw new ArgumentOutOfRangeException(nameof(highMidGain), "Value must be between 0-40");
            if (highShelfFreq > 127) throw new ArgumentOutOfRangeException(nameof(highShelfFreq), "Value must be between 0-127");
            if (highShelfGain > 40) throw new ArgumentOutOfRangeException(nameof(highShelfGain), "Value must be between 0-40");

            byte[] sysEx = ConstructEqualizerSysExMessage(eqSwitch, lowShelfFreq, lowShelfGain,
                                                         lowMidFreq, lowMidQ, lowMidGain,
                                                         highMidFreq, highMidQ, highMidGain,
                                                         highShelfFreq, highShelfGain);
            SendSysExMessage(sysEx);
        }

        #endregion

        #region SysEx Message Construction Methods

        private byte[] ConstructRobotSysExMessage(byte octave, byte feedbackSwitch, byte feedbackResonance, byte feedbackLevel)
        {
            // F0 41 10 00 00 00 51 12 [Address: 00 00 00 30] [Data Length: 04] [octave, fbSwitch, fbResonance, fbLevel] [Checksum] F7
            byte[] message = new byte[]
            {
                0xF0,
                0x41,       // Roland manufacturer ID
                0x10,       // Device ID
                0x00, 0x00, 0x00, // Model ID (example)
                0x51,
                0x12,       // Robot effect command ID
                0x00, 0x00, 0x00, 0x30, // Address
                0x04,       // Data length (4 bytes)
                octave,
                feedbackSwitch,
                feedbackResonance,
                feedbackLevel,
                0x00,       // Checksum placeholder
                0xF7
            };
            
            // Calculate checksum: address (4 bytes) + data length + data (4 bytes) => total 9 bytes
            message[message.Length - 2] = CalculateRolandChecksum(message, 8, 9);
            return message;
        }

        private byte[] ConstructHarmonySysExMessage(byte h1Level, byte h2Level, byte h3Level,
                                                  byte h1Key, byte h2Key, byte h3Key,
                                                  byte h1Gender, byte h2Gender, byte h3Gender)
        {
            // F0 41 10 00 00 00 51 13 [Address: 31 00 00 00] [Data Length: 09] 
            // [h1Level, h2Level, h3Level, h1Key, h2Key, h3Key, h1Gender, h2Gender, h3Gender] [Checksum] F7
            byte[] message = new byte[]
            {
                0xF0,
                0x41,
                0x10,
                0x00, 0x00, 0x00,
                0x51,
                0x13,       // Harmony effect command ID
                0x31, 0x00, 0x00, 0x00, // Address
                0x09,       // Data length: 9 bytes
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
            
            message[message.Length - 2] = CalculateRolandChecksum(message, 8, 14); // 4 address + 1 length + 9 data = 14 bytes
            return message;
        }

        private byte[] ConstructMegaphoneSysExMessage(byte type, byte param1, byte param2, byte param3, byte param4)
        {
            // F0 41 10 00 00 00 51 14 [Address: 41 00 00 00] [Data Length: 05]
            // [type, param1, param2, param3, param4] [Checksum] F7
            byte[] message = new byte[]
            {
                0xF0,
                0x41,
                0x10,
                0x00, 0x00, 0x00,
                0x51,
                0x14,       // Megaphone effect command ID
                0x41, 0x00, 0x00, 0x00, // Address
                0x05,       // Data length: 5 bytes
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

        private byte[] ConstructReverbSysExMessage(byte type, byte param1, byte param2, byte param3, byte param4)
        {
            // F0 41 10 00 00 00 51 15 [Address: 51 00 00 00] [Data Length: 05]
            // [type, param1, param2, param3, param4] [Checksum] F7
            byte[] message = new byte[]
            {
                0xF0,
                0x41,
                0x10,
                0x00, 0x00, 0x00,
                0x51,
                0x15,       // Reverb effect command ID
                0x51, 0x00, 0x00, 0x00, // Address
                0x05,       // Data length: 5 bytes
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

        private byte[] ConstructVocoderSysExMessage(byte type, byte param1, byte param2, byte param3, byte param4)
        {
            // F0 41 10 00 00 00 51 16 [Address: 61 00 00 00] [Data Length: 05]
            // [type, param1, param2, param3, param4] [Checksum] F7
            byte[] message = new byte[]
            {
                0xF0,
                0x41,
                0x10,
                0x00, 0x00, 0x00,
                0x51,
                0x16,       // Vocoder effect command ID
                0x61, 0x00, 0x00, 0x00, // Address
                0x05,       // Data length: 5 bytes
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

        private byte[] ConstructEqualizerSysExMessage(byte eqSwitch, byte lowShelfFreq, byte lowShelfGain,
                                                   byte lowMidFreq, byte lowMidQ, byte lowMidGain,
                                                   byte highMidFreq, byte highMidQ, byte highMidGain,
                                                   byte highShelfFreq, byte highShelfGain)
        {
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
                0x17,       // Equalizer effect command ID
                0x63, 0x00, 0x00, 0x00, // Address
                0x0B,       // Data length: 11 bytes
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

        #endregion

        #region Utility Methods

        /// <summary>
        /// Calculates Roland checksum: (128 - (sum % 128)) % 128
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
        /// Ensures the MIDI device is initialized
        /// </summary>
        private void EnsureInitialized()
        {
            if (!isInitialized || midiOut == null)
            {
                _logger.LogWarning("MIDI device not initialized");
                throw new InvalidOperationException("MIDI device not initialized. Call Initialize() first.");
            }
        }

        #endregion

        /// <summary>
        /// Disposes of MIDI resources
        /// </summary>
        public void Dispose()
        {
            midiOut?.Dispose();
            midiOut = null;
            isInitialized = false;
            GC.SuppressFinalize(this);
        }
    }
}
