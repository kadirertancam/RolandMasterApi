using System.ComponentModel.DataAnnotations;

namespace RolandTestCore.Models
{
    /// <summary>
    /// Model for Vocoder effect parameters
    /// </summary>
    public class VocoderModel
    {
        /// <summary>
        /// Vocoder type (0-4)
        /// 0: Vintage, 1: Advanced, 2: Talk Box, 3: Spell Toy
        /// </summary>
        [Range(0, 4, ErrorMessage = "Type must be between 0-4")]
        public byte Type { get; set; }

        /// <summary>
        /// Parameter 1 (0-255) - Meaning depends on vocoder type
        /// </summary>
        [Range(0, 255, ErrorMessage = "Param1 must be between 0-255")]
        public byte Param1 { get; set; }

        /// <summary>
        /// Parameter 2 (0-255) - Meaning depends on vocoder type
        /// </summary>
        [Range(0, 255, ErrorMessage = "Param2 must be between 0-255")]
        public byte Param2 { get; set; }

        /// <summary>
        /// Parameter 3 (0-255) - Meaning depends on vocoder type
        /// </summary>
        [Range(0, 255, ErrorMessage = "Param3 must be between 0-255")]
        public byte Param3 { get; set; }

        /// <summary>
        /// Parameter 4 (0-255) - Meaning depends on vocoder type
        /// </summary>
        [Range(0, 255, ErrorMessage = "Param4 must be between 0-255")]
        public byte Param4 { get; set; }
    }
}