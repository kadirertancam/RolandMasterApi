using System.ComponentModel.DataAnnotations;

namespace RolandTestCore.Models
{
    /// <summary>
    /// Model for Harmony effect parameters
    /// </summary>
    public class HarmonyModel
    {
        /// <summary>
        /// Harmony 1 level (0-255)
        /// </summary>
        [Range(0, 255, ErrorMessage = "H1Level must be between 0-255")]
        public byte H1Level { get; set; }

        /// <summary>
        /// Harmony 2 level (0-255)
        /// </summary>
        [Range(0, 255, ErrorMessage = "H2Level must be between 0-255")]
        public byte H2Level { get; set; }

        /// <summary>
        /// Harmony 3 level (0-255)
        /// </summary>
        [Range(0, 255, ErrorMessage = "H3Level must be between 0-255")]
        public byte H3Level { get; set; }

        /// <summary>
        /// Harmony 1 key (0-11)
        /// 0: C, 1: C#, 2: D, 3: D#, 4: E, 5: F, 6: F#, 7: G, 8: G#, 9: A, 10: A#, 11: B
        /// </summary>
        [Range(0, 11, ErrorMessage = "H1Key must be between 0-11")]
        public byte H1Key { get; set; }

        /// <summary>
        /// Harmony 2 key (0-11)
        /// 0: C, 1: C#, 2: D, 3: D#, 4: E, 5: F, 6: F#, 7: G, 8: G#, 9: A, 10: A#, 11: B
        /// </summary>
        [Range(0, 11, ErrorMessage = "H2Key must be between 0-11")]
        public byte H2Key { get; set; }

        /// <summary>
        /// Harmony 3 key (0-11)
        /// 0: C, 1: C#, 2: D, 3: D#, 4: E, 5: F, 6: F#, 7: G, 8: G#, 9: A, 10: A#, 11: B
        /// </summary>
        [Range(0, 11, ErrorMessage = "H3Key must be between 0-11")]
        public byte H3Key { get; set; }

        /// <summary>
        /// Harmony 1 gender (0-255)
        /// </summary>
        [Range(0, 255, ErrorMessage = "H1Gender must be between 0-255")]
        public byte H1Gender { get; set; }

        /// <summary>
        /// Harmony 2 gender (0-255)
        /// </summary>
        [Range(0, 255, ErrorMessage = "H2Gender must be between 0-255")]
        public byte H2Gender { get; set; }

        /// <summary>
        /// Harmony 3 gender (0-255)
        /// </summary>
        [Range(0, 255, ErrorMessage = "H3Gender must be between 0-255")]
        public byte H3Gender { get; set; }
    }
}