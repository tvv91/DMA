using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Web.Db;
using Web.Interfaces;
using Web.Models;

namespace Web.Implementation
{
    //public class StatisticRepository : IStatisticRepository
    //{
    //    private readonly DMADbContext _context;
    //    private readonly double[] _dsdFreq = [2.8, 5.6, 11.2, 22.5];

    //    public StatisticRepository(DMADbContext ctx) => _context = ctx;

    //    public async Task<Statistic> Process()
    //    {
    //        var result = await _context.Statistics.FirstOrDefaultAsync();

    //        if (result == null)
    //        {
    //            result = await RefreshStatistic();
    //            await _context.Statistics.AddAsync(result);
    //            await _context.SaveChangesAsync();
    //        }
    //        else
    //        {
    //            var lastUpdate = result.LastUpdate;
    //            // check that last time statistic updated less then 1 day ago
    //            if ((DateTime.UtcNow - lastUpdate).Days > 1)
    //            {
    //                result = await RefreshStatistic();
    //                await _context.Statistics
    //                    .Where(x => x.Id == 1)
    //                    .ExecuteUpdateAsync(x => x.SetProperty(e => e.Name, result.Name).SetProperty(e => e.LastUpdate, result.LastUpdate));
    //            }
    //        }

    //        return result;
    //    }

    //    private async Task<Statistic> RefreshStatistic()
    //    {
    //        var statisticData = new StatisticCounters
    //        {
    //            TotalAlbums = await CountAlbums(),
    //            TotalSize = await CountSize(),
    //            StorageCount = await CountStorages(),
    //            Genre = await CountGenres(),
    //            Year = await CountYears(),
    //            Country = await CountCountries(),
    //            Label = await CountLabels(),
    //            Bitness = await CountBitness(),
    //            Sampling = await CountSampling(),
    //            SourceFormat = await CountSourceFormat(),
    //            DigitalFormat = await CountDigitalFormat(),
    //            Adc = await CountAdc(),
    //            Amplifier = await CountAmplifier(),
    //            Cartridge = await CountCartridge(),
    //            Player = await CountPlayer(),
    //            VinylState = await CountVinylState(),
    //            Wire = await CountWire(),
    //        };

    //        var statistic = new Statistic
    //        {
    //            Name = JsonSerializer.Serialize(statisticData),
    //            LastUpdate = DateTime.UtcNow
    //        };

    //        return statistic;
    //    }

    //    public async Task<List<CounterItem>> CountWire()
    //    {
    //        return await _context.Wires.GroupJoin(_context.TechnicalInfos, x => x.Id, y => y.WireId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = x.Data,
    //            Count = y.Count()
    //        }).ToListAsync();
    //    }

    //    public async Task<List<CounterItem>> CountVinylState()
    //    {
    //        return await _context.VinylStates.GroupJoin(_context.TechnicalInfos, x => x.Id, y => y.VinylStateId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = x.Data,
    //            Count = y.Count()
    //        }).Where(x => x.Count > 0).ToListAsync();
    //    }

    //    public async Task<List<CounterItem>> CountPlayer()
    //    {

    //        return await _context.Players.GroupJoin(_context.TechnicalInfos, x => x.Id, y => y.PlayerId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = $"{x.Manufacturer.Data} {x.Data}",
    //            Count = y.Count()
    //        }).Where(x => x.Count > 0).ToListAsync();
    //    }

    //    public async Task<List<CounterItem>> CountCartridge()
    //    {
    //        return await _context.Cartridges.GroupJoin(_context.TechnicalInfos, x => x.Id, y => y.AmplifierId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = $"{x.Manufacturer.Data} {x.Data}",
    //            Count = y.Count()
    //        }).Where(x => x.Count > 0).ToListAsync();
    //    }

    //    public async Task<List<CounterItem>> CountAmplifier()
    //    {
    //        return await _context.Amplifiers.GroupJoin(_context.TechnicalInfos, x => x.Id, y => y.AmplifierId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = $"{x.Manufacturer.Data} {x.Data}",
    //            Count = y.Count()
    //        }).Where(x => x.Count > 0).ToListAsync();
    //    }

    //    public async Task<List<CounterItem>> CountAdc()
    //    {
    //        return await _context.Adces.GroupJoin(_context.TechnicalInfos, x => x.Id, y => y.AdcId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = $"{x.Manufacturer.Data} {x.Data}",
    //            Count = y.Count()
    //        }).Where(x => x.Count > 0).ToListAsync();
    //    }

    //    public async Task<List<CounterItem>> CountDigitalFormat()
    //    {
    //        return await _context.DigitalFormats.GroupJoin(_context.TechnicalInfos, x => x.Id, y => y.DigitalFormatId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = x.Data,
    //            Count = y.Count()
    //        }).Where(x => x.Count > 0).ToListAsync();
    //    }

    //    public async Task<List<CounterItem>> CountSourceFormat()
    //    {
    //        return await _context.SourceFormats.GroupJoin(_context.TechnicalInfos, x => x.Id, y => y.SourceFormatId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = x.Data,
    //            Count = y.Count()
    //        }).Where(x => x.Count > 0).ToListAsync();
    //    }

    //    public async Task<List<CounterItem>> CountSampling()
    //    {
    //        return await _context.Samplings.GroupJoin(_context.TechnicalInfos, x => x.Id, y => y.SamplingId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = x.Data.ToString() + (_dsdFreq.Contains(x.Data) ? " MHz" : " kHz"),
    //            Count = y.Count()
    //        }).Where(x => x.Count > 0).ToListAsync();
    //    }

    //    public async Task<List<CounterItem>> CountBitness()
    //    {
    //        return await _context.Bitnesses.GroupJoin(_context.TechnicalInfos, x => x.Id, y => y.BitnessId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = x.Data.ToString() + " bit/s",
    //            Count = y.Count()
    //        }).Where(x => x.Count > 0).ToListAsync();
    //    }

    //    public async Task<List<CounterItem>> CountLabels()
    //    {
    //        return await _context.Labels.GroupJoin(_context.Albums, x => x.Id, y => y.LabelId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = x.Data,
    //            Count = y.Count()
    //        }).Where(x => x.Count > 0).ToListAsync();
    //    }

    //    public async Task<List<CounterItem>> CountCountries()
    //    {
    //        return await _context.Countries.GroupJoin(_context.Albums, x => x.Id, y => y.CountryId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = x.Name,
    //            Count = y.Count()
    //        }).Where(x => x.Count > 0).ToListAsync();
    //    }

    //    public async Task<List<CounterItem>> CountYears()
    //    {
    //        return await _context.Years.GroupJoin(_context.Albums, x => x.Id, y => y.YearId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = x.YearValue.ToString(),
    //            Count = y.Count()
    //        }).Where(x => x.Count > 0).ToListAsync();
    //    }

    //    public async Task<List<CounterItem>> CountGenres()
    //    {
    //        return await _context.Genres.GroupJoin(_context.Albums, x => x.Id, y => y.GenreId, (x, y) =>
    //        new CounterItem
    //        {
    //            Description = x.Name,
    //            Count = y.Count()
    //        }).Where(x => x.Count > 0).ToListAsync();
    //    }

    //    public async Task<int> CountStorages()
    //    {
    //        return await _context.Storages.CountAsync();
    //    }

    //    public async Task<double?> CountSize()
    //    {
    //        return await _context.Albums.SumAsync(x => x.Size);
    //    }

    //    public async Task<int> CountAlbums()
    //    {
    //        return await _context.Albums.CountAsync();
    //    }
    //}
}
