namespace DMA.Domain.Statistics;

public interface IStatisticRepository
{
    Task<Statistic?> GetFirstAsync(CancellationToken cancellationToken = default);
    void Add(Statistic statistic);
    IQueryable<Albums.Album> Albums { get; }
    IQueryable<Albums.Release> Releases { get; }
    IQueryable<ReferenceData.Artist> Artists { get; }
    IQueryable<ReferenceData.Genre> Genres { get; }
    IQueryable<ReferenceData.Year> Years { get; }
    IQueryable<ReferenceData.Country> Countries { get; }
    IQueryable<ReferenceData.Label> Labels { get; }
    IQueryable<ReferenceData.Storage> Storages { get; }
    IQueryable<ReferenceData.Bitness> Bitnesses { get; }
    IQueryable<ReferenceData.Sampling> Samplings { get; }
    IQueryable<ReferenceData.SourceFormat> SourceFormats { get; }
    IQueryable<ReferenceData.DigitalFormat> DigitalFormats { get; }
    IQueryable<ReferenceData.VinylState> VinylStates { get; }
    IQueryable<Equipment.Adc> Adces { get; }
    IQueryable<Equipment.Amplifier> Amplifiers { get; }
    IQueryable<Equipment.Cartridge> Cartridges { get; }
    IQueryable<Equipment.Player> Players { get; }
    IQueryable<Equipment.Wire> Wires { get; }
}
