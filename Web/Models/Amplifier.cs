﻿namespace Web.Models
{
    /// <summary>
    /// Information about amplifier
    /// </summary>
    public class Amplifier
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public AmplifierManufacturer AmplifierManufacturer { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
