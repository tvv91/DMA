namespace Web.Models
{
    /// <summary>
    /// Information about playback device
    /// </summary>
    public class Device
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
