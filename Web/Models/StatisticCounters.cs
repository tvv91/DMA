namespace Web.Models
{
    public class StatisticCounters
    {
        public int TotalAlbums { get; set; }
        public double TotalSize { get; set; }
        public int StorageCount { get; set; }
        public int TotalDigitizations { get; set; }
        public int TotalArtists { get; set; }
        public int TotalEquipment { get; set; }
        public List<CounterItem>? Genre { get; set; }
        public List<CounterItem>? Artist { get; set; }
        public List<CounterItem>? Year { get; set; }
        public List<CounterItem>? Country { get; set; }
        public List<CounterItem>? Adc { get; set; }
        public List<CounterItem>? Amplifier { get; set; }
        public List<CounterItem>? Bitness { get; set; }
        public List<CounterItem>? Cartridge { get; set; }
        public List<CounterItem>? DigitalFormat { get; set; }
        public List<CounterItem>? Player { get; set; }
        public List<CounterItem>? SourceFormat { get; set; }
        public List<CounterItem>? Sampling { get; set; }
        public List<CounterItem>? VinylState { get; set; }
        public List<CounterItem>? Wire { get; set; }
        public List<CounterItem>? Label { get; set; }
    }

    public class CounterItem
    {
        public string? Description { get; set; }
        public int Count { get; set; }
    }
}
