﻿using System.ComponentModel.DataAnnotations;

namespace Web.Request
{
    public class AlbumDataRequest
    {
        #region Main info
        [Required]
        public string Album { get; set; }
        [Required]
        public string Artist { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public int Year { get; set; }
        public int? Reissue { get; set; }
        public string? Country { get; set; }
        public string? Label { get; set; }
        public string? Source { get; set; }
        public double? Size { get; set; }
        public string? Storage { get; set; }
        public string? AlbumCover { get; set; }

        #endregion

        #region Technical Info
        public string? Adc { get; set; }
        public string? Amplifier { get; set; }
        public int? Bitness { get; set; }
        public string? Cartridge { get; set; }
        public string? Codec { get; set; }
        public string? Device { get; set; }
        public string? Format { get; set; }
        public string? Processing { get; set; }
        public double? Sampling { get; set; }
        public string? State { get; set; }
        #endregion

        public bool IsEdit { get; set; }
        public int AlbumId { get; set; }
    }
}
