using System.ComponentModel.DataAnnotations;

namespace RolandTestCore.Models
{
    /// <summary>
    /// Model for Robot effect parameters
    /// </summary>
    public class RobotModel
    {
        /// <summary>
        /// Octave setting (0-3)
        /// 0: 2DOWN, 1: DOWN, 2: ZERO, 3: UP
        /// </summary>
        [Range(0, 3, ErrorMessage = "Octave must be between 0-3")]
        public byte Octave { get; set; }

        /// <summary>
        /// Feedback switch on/off (0-1)
        /// </summary>
        [Range(0, 1, ErrorMessage = "FeedbackSwitch must be 0 or 1")]
        public byte FeedbackSwitch { get; set; }

        /// <summary>
        /// Feedback resonance value (0-255)
        /// </summary>
        [Range(0, 255, ErrorMessage = "FeedbackResonance must be between 0-255")]
        public byte FeedbackResonance { get; set; }

        /// <summary>
        /// Feedback level value (0-255)
        /// </summary>
        [Range(0, 255, ErrorMessage = "FeedbackLevel must be between 0-255")]
        public byte FeedbackLevel { get; set; }
    }
}