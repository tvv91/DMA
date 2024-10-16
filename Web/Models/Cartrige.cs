﻿namespace Web.Models
{
    /// <summary>
    /// Information about playback device cartrige / headshell
    /// </summary>
    public class Cartrige
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public CartrigeManufacturer CartrigeManufacturer { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
