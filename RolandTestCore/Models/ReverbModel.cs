using System.ComponentModel.DataAnnotations;

namespace RolandTestCore.Models
{
    /// <summary>
    /// Model for Reverb effect parameters
    /// </summary>
    public class ReverbModel
    {
        /// <summary>
        /// Reverb type (0-5)
        /// 0: Reverb, 1: Echo, 2: Delay, 3: Dub Echo, 4: Deep Reverb, 5: VT Reverb
        /// </summary>
        [Range(0, 5, ErrorMessage = "Type must be between 0-5")]
        public byte Type { get; set; }

        /// <summary>
        /// Parameter 1 (0-255) - Meaning depends on reverb type
        /// </summary>
        [Range(0, 255, ErrorMessage = "Param1 must be between 0-255")]
        public byte Param1 { get; set; }

        /// <summary>
        /// Parameter 2 (0-255) - Meaning depends on reverb type
        /// </summary>
        [Range(0, 255, ErrorMessage = "Param2 must be between 0-255")]
        public byte Param2 { get; set; }

        /// <summary>
        /// Parameter 3 (0-255) - Meaning depends on reverb type
        /// </summary>
        [Range(0, 255, ErrorMessage = "Param3 must be between 0-255")]
        public byte Param3 { get; set; }

        /// <summary>
        /// Parameter 4 (0-255) - Meaning depends on reverb type
        /// </summary>
        [Range(0, 255, ErrorMessage = "Param4 must be between 0-255")]
        public byte Param4 { get; set; }
    }
}