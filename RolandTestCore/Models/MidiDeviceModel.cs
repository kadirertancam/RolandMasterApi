namespace RolandTestCore.Models
{
    /// <summary>
    /// Model for MIDI device information
    /// </summary>
    public class MidiDeviceModel
    {
        /// <summary>
        /// Device ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Device name
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Model for MIDI device initialization
    /// </summary>
    public class MidiInitializeModel
    {
        /// <summary>
        /// MIDI device ID to initialize
        /// </summary>
        public int DeviceId { get; set; } = 1;
    }
}