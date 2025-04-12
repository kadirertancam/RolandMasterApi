using System.ComponentModel.DataAnnotations;

namespace RolandTestCore.Models
{
    /// <summary>
    /// Model for Megaphone effect parameters
    /// </summary>
    public class MegaphoneModel
    {
        /// <summary>
        /// Megaphone type (0-3)
        /// 0: Megaphone, 1: Radio, 2: BBD Chorus, 3: Strobo
        /// </summary>
        [Range(0, 3, ErrorMessage = "Type must be between 0-3")]
        public byte Type { get; set; }

        /// <summary>
        /// Parameter 1 (0-255) - Meaning depends on megaphone type
        /// </summary>
        [Range(0, 255, ErrorMessage = "Param1 must be between 0-255")]
        public byte Param1 { get; set; }

        /// <summary>
        /// Parameter 2 (0-255) - Meaning depends on megaphone type
        /// </summary>
        [Range(0, 255, ErrorMessage = "Param2 must be between 0-255")]
        public byte Param2 { get; set; }

        /// <summary>
        /// Parameter 3 (0-255) - Meaning depends on megaphone type
        /// </summary>
        [Range(0, 255, ErrorMessage = "Param3 must be between 0-255")]
        public byte Param3 { get; set; }

        /// <summary>
        /// Parameter 4 (0-255) - Meaning depends on megaphone type
        /// </summary>
        [Range(0, 255, ErrorMessage = "Param4 must be between 0-255")]
        public byte Param4 { get; set; }
    }
}