namespace Web.Models
{
    public class AdcManufacturer
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public ICollection<Adc> Adcs { get; set; }
    }
}
