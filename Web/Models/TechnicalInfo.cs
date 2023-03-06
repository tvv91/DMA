namespace Web.Models
{
    /// <summary>
    /// Information about album digitizing
    /// </summary>
    public class TechnicalInfo : Base
    {
        public int Id { get; set; }

        /// <summary>
        /// Source format (vinyl, cd, magnetic tape, etc.)
        /// </summary>
        public Format? Format { get; set; }
        
        /// <summary>
        /// Audio codec (FLAC, WAV, DSD, etc.)
        /// </summary>
        public Codec? Codec { get; set; }

        /// <summary>
        /// Audio bitness (24 / 32 bit, etc.)
        /// </summary>
        public Bitness? Bitness { get; set; }

        /// <summary>
        /// Audio sampling (44 / 96 / 192 kHz, etc.)
        /// </summary>
        public Sampling? Sampling { get; set; }

        /// <summary>
        /// Audio source state (mint, near mint, etc.)
        /// </summary>
        public State? State { get; set; }

        /// <summary>
        /// Sound device manufacturer 
        /// </summary>
        public Device? Device { get; set; }

        /// <summary>
        /// Cartrige or headshell manufacturer
        /// </summary>
        public Cartrige? Cartrige { get; set; }

        /// <summary>
        /// Amplifierl manufacturer
        /// </summary>
        public Amplifier? Amplifier { get; set; }

        /// <summary>
        /// Analog to digital converter manufacturer
        /// </summary>
        public Adc? Adc { get; set; }

        /// <summary>
        /// Any information about sound post-processing after digitizing (declicking, etc.)
        /// </summary>
        public Processing? Processing { get; set; }
    }
}
