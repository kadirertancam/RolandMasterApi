using System.ComponentModel.DataAnnotations;

namespace RolandTestCore.Models
{
    /// <summary>
    /// Model for Equalizer effect parameters
    /// </summary>
    public class EqualizerModel
    {
        /// <summary>
        /// Equalizer on/off switch (0-1)
        /// </summary>
        [Range(0, 1, ErrorMessage = "EqSwitch must be 0 or 1")]
        public byte EqSwitch { get; set; }

        /// <summary>
        /// Low shelf frequency (0-127)
        /// </summary>
        [Range(0, 127, ErrorMessage = "LowShelfFreq must be between 0-127")]
        public byte LowShelfFreq { get; set; }

        /// <summary>
        /// Low shelf gain (0-40)
        /// </summary>
        [Range(0, 40, ErrorMessage = "LowShelfGain must be between 0-40")]
        public byte LowShelfGain { get; set; }

        /// <summary>
        /// Low mid frequency (0-127)
        /// </summary>
        [Range(0, 127, ErrorMessage = "LowMidFreq must be between 0-127")]
        public byte LowMidFreq { get; set; }

        /// <summary>
        /// Low mid Q (0-127)
        /// </summary>
        [Range(0, 127, ErrorMessage = "LowMidQ must be between 0-127")]
        public byte LowMidQ { get; set; }

        /// <summary>
        /// Low mid gain (0-40)
        /// </summary>
        [Range(0, 40, ErrorMessage = "LowMidGain must be between 0-40")]
        public byte LowMidGain { get; set; }

        /// <summary>
        /// High mid frequency (0-127)
        /// </summary>
        [Range(0, 127, ErrorMessage = "HighMidFreq must be between 0-127")]
        public byte HighMidFreq { get; set; }

        /// <summary>
        /// High mid Q (0-127)
        /// </summary>
        [Range(0, 127, ErrorMessage = "HighMidQ must be between 0-127")]
        public byte HighMidQ { get; set; }

        /// <summary>
        /// High mid gain (0-40)
        /// </summary>
        [Range(0, 40, ErrorMessage = "HighMidGain must be between 0-40")]
        public byte HighMidGain { get; set; }

        /// <summary>
        /// High shelf frequency (0-127)
        /// </summary>
        [Range(0, 127, ErrorMessage = "HighShelfFreq must be between 0-127")]
        public byte HighShelfFreq { get; set; }

        /// <summary>
        /// High shelf gain (0-40)
        /// </summary>
        [Range(0, 40, ErrorMessage = "HighShelfGain must be between 0-40")]
        public byte HighShelfGain { get; set; }
    }
}