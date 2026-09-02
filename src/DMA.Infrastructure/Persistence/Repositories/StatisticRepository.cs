using DMA.Domain.Albums;
using DMA.Domain.Equipment;
using DMA.Domain.ReferenceData;
using DMA.Domain.Statistics;
using Microsoft.EntityFrameworkCore;

namespace DMA.Infrastructure.Persistence.Repositories;

public class StatisticRepository(DmaDbContext context) : IStatisticRepository
{
    private const int TopStatisticsItems = 10;
    private static readonly double[] DsdFreq = { 2.8, 5.6, 11.2, 22.5 };

    private readonly DmaDbContext _context = context;

    public Task<Statistic?> GetFirstAsync(CancellationToken cancellationToken = default) =>
        _context.Statistics.FirstOrDefaultAsync(cancellationToken);

    public void Add(Statistic statistic) => _context.Statistics.Add(statistic);

    public IQueryable<Album> Albums => _context.Albums;
    public IQueryable<Release> Releases => _context.Releases;
    public IQueryable<Artist> Artists => _context.Artists;
    public IQueryable<Genre> Genres => _context.Genres;
    public IQueryable<Year> Years => _context.Years;
    public IQueryable<Country> Countries => _context.Countries;
    public IQueryable<Label> Labels => _context.Labels;
    public IQueryable<Storage> Storages => _context.Storages;
    public IQueryable<Bitness> Bitnesses => _context.Bitnesses;
    public IQueryable<Sampling> Samplings => _context.Samplings;
    public IQueryable<SourceFormat> SourceFormats => _context.SourceFormats;
    public IQueryable<DigitalFormat> DigitalFormats => _context.DigitalFormats;
    public IQueryable<VinylState> VinylStates => _context.VinylStates;
    public IQueryable<Adc> Adces => _context.Adces;
    public IQueryable<Amplifier> Amplifiers => _context.Amplifiers;
    public IQueryable<Cartridge> Cartridges => _context.Cartridges;
    public IQueryable<Player> Players => _context.Players;
    public IQueryable<Wire> Wires => _context.Wires;

    public async Task<StatisticCounters> ComputeCountersAsync(CancellationToken cancellationToken = default)
    {
        var totalEquipment = await _context.Adces.CountAsync(cancellationToken) +
                             await _context.Amplifiers.CountAsync(cancellationToken) +
                             await _context.Cartridges.CountAsync(cancellationToken) +
                             await _context.Players.CountAsync(cancellationToken) +
                             await _context.Wires.CountAsync(cancellationToken);

        return new StatisticCounters
        {
            TotalAlbums = await _context.Albums.CountAsync(cancellationToken),
            TotalSize = await _context.Releases.Where(d => d.Size != null).SumAsync(d => d.Size ?? 0, cancellationToken),
            StorageCount = await _context.Storages.CountAsync(cancellationToken),
            TotalReleases = await _context.Releases.CountAsync(cancellationToken),
            TotalArtists = await _context.Artists.CountAsync(cancellationToken),
            TotalEquipment = totalEquipment,
            Genre = await CountGenresAsync(cancellationToken),
            Artist = await CountArtistsAsync(cancellationToken),
            Year = await CountYearsAsync(cancellationToken),
            Country = await CountCountriesAsync(cancellationToken),
            Label = await CountLabelsAsync(cancellationToken),
            Bitness = await CountBitnessAsync(cancellationToken),
            Sampling = await CountSamplingAsync(cancellationToken),
            SourceFormat = await CountSourceFormatsAsync(cancellationToken),
            DigitalFormat = await CountDigitalFormatsAsync(cancellationToken),
            Adc = await CountAdcAsync(cancellationToken),
            Amplifier = await CountAmplifiersAsync(cancellationToken),
            Cartridge = await CountCartridgesAsync(cancellationToken),
            Player = await CountPlayersAsync(cancellationToken),
            VinylState = await CountVinylStatesAsync(cancellationToken),
            Wire = await CountWiresAsync(cancellationToken)
        };
    }

    private async Task<List<CounterItem>> CountGenresAsync(CancellationToken cancellationToken) =>
        await _context.Genres
            .Select(g => new CounterItem { Description = g.Name, Count = g.Albums.Count })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountArtistsAsync(CancellationToken cancellationToken) =>
        await _context.Artists
            .Select(a => new CounterItem { Description = a.Name, Count = a.Albums.Count })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountYearsAsync(CancellationToken cancellationToken) =>
        await _context.Years
            .Where(y => y.Releases.Any())
            .Select(y => new CounterItem { Description = y.Value.ToString(), Count = y.Releases.Count })
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountCountriesAsync(CancellationToken cancellationToken) =>
        await _context.Countries
            .Select(c => new CounterItem { Description = c.Name, Count = c.Releases.Count })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountLabelsAsync(CancellationToken cancellationToken) =>
        await _context.Labels
            .Select(l => new CounterItem { Description = l.Name, Count = l.Releases.Count })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountBitnessAsync(CancellationToken cancellationToken) =>
        await _context.Bitnesses
            .Select(b => new CounterItem
            {
                Description = $"{b.Value} bit",
                Count = _context.Releases.Count(d => d.FormatInfo != null && d.FormatInfo.BitnessId == b.Id)
            })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountSamplingAsync(CancellationToken cancellationToken) =>
        await _context.Samplings
            .Select(s => new CounterItem
            {
                Description = $"{s.Value}{(DsdFreq.Contains(s.Value) ? " MHz" : " kHz")}",
                Count = _context.Releases.Count(d => d.FormatInfo != null && d.FormatInfo.SamplingId == s.Id)
            })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountSourceFormatsAsync(CancellationToken cancellationToken) =>
        await _context.SourceFormats
            .Select(s => new CounterItem
            {
                Description = s.Name,
                Count = _context.Releases.Count(d => d.FormatInfo != null && d.FormatInfo.SourceFormatId == s.Id)
            })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountDigitalFormatsAsync(CancellationToken cancellationToken) =>
        await _context.DigitalFormats
            .Select(d => new CounterItem
            {
                Description = d.Name,
                Count = _context.Releases.Count(dig => dig.FormatInfo != null && dig.FormatInfo.DigitalFormatId == d.Id)
            })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountVinylStatesAsync(CancellationToken cancellationToken) =>
        await _context.VinylStates
            .Select(v => new CounterItem
            {
                Description = v.Name,
                Count = _context.Releases.Count(d => d.FormatInfo != null && d.FormatInfo.VinylStateId == v.Id)
            })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountAdcAsync(CancellationToken cancellationToken) =>
        await _context.Adces
            .Select(a => new CounterItem
            {
                Description = a.Manufacturer != null ? $"{a.Manufacturer.Name} {a.Name}" : a.Name,
                Count = _context.Releases.Count(d => d.EquipmentInfo != null && d.EquipmentInfo.AdcId == a.Id)
            })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountAmplifiersAsync(CancellationToken cancellationToken) =>
        await _context.Amplifiers
            .Select(a => new CounterItem
            {
                Description = a.Manufacturer != null ? $"{a.Manufacturer.Name} {a.Name}" : a.Name,
                Count = _context.Releases.Count(d => d.EquipmentInfo != null && d.EquipmentInfo.AmplifierId == a.Id)
            })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountCartridgesAsync(CancellationToken cancellationToken) =>
        await _context.Cartridges
            .Select(c => new CounterItem
            {
                Description = c.Manufacturer != null ? $"{c.Manufacturer.Name} {c.Name}" : c.Name,
                Count = _context.Releases.Count(d => d.EquipmentInfo != null && d.EquipmentInfo.CartridgeId == c.Id)
            })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountPlayersAsync(CancellationToken cancellationToken) =>
        await _context.Players
            .Select(p => new CounterItem
            {
                Description = p.Manufacturer != null ? $"{p.Manufacturer.Name} {p.Name}" : p.Name,
                Count = _context.Releases.Count(d => d.EquipmentInfo != null && d.EquipmentInfo.PlayerId == p.Id)
            })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);

    private async Task<List<CounterItem>> CountWiresAsync(CancellationToken cancellationToken) =>
        await _context.Wires
            .Select(w => new CounterItem
            {
                Description = w.Manufacturer != null ? $"{w.Manufacturer.Name} {w.Name}" : w.Name,
                Count = _context.Releases.Count(d => d.EquipmentInfo != null && d.EquipmentInfo.WireId == w.Id)
            })
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .Take(TopStatisticsItems)
            .ToListAsync(cancellationToken);
}
