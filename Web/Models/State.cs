namespace Web.Models
{
    /// <summary>
    /// Information about audio source state
    /// </summary>
    public class State
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public ICollection<TechnicalInfo> TechnicalInfos { get; set; }
    }
}
