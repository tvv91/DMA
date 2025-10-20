using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Web.Db;
using Web.Interfaces;
using Web.Models;

namespace Web.Implementation
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly DMADbContext _context;
        private static readonly double[] _dsdFreq = { 2.8, 5.6, 11.2, 22.5 };

        public StatisticRepository(DMADbContext context) => _context = context;

        public async Task<Statistic> Process()
        {
            var stat = await _context.Statistics.FirstOrDefaultAsync();

            if (stat == null)
            {
                stat = await RefreshStatistic();
                await _context.Statistics.AddAsync(stat);
                await _context.SaveChangesAsync();
                return stat;
            }

            if (DateTime.UtcNow - stat.LastUpdate > TimeSpan.FromDays(1))
            {
                var refreshed = await RefreshStatistic();
                stat.Name = refreshed.Name;
                stat.LastUpdate = refreshed.LastUpdate;
                await _context.SaveChangesAsync();
            }

            return stat;
        }

        private async Task<Statistic> RefreshStatistic()
        {
            var data = new StatisticCounters
            {
                TotalAlbums = await _context.Albums.CountAsync(),
                TotalSize = await _context.Albums.SumAsync(a => a.Digitizations.Sum(d => d.FormatInfo!.Size) ?? 0),
                StorageCount = await _context.Storages.CountAsync(),
                Genre = await CountGeneric(_context.Genres, g => g.Albums.SelectMany(a => a.Digitizations)),
                //Year = await CountGeneric(_context.Years, y => y.Al.SelectMany(a => a.Digitizations)),
                Country = await CountGeneric(_context.Countries, c => c.Digitizations),
                Label = await CountGeneric(_context.Labels, l => l.Digitizations),
                //Bitness = await CountGeneric(_context.Bitnesses, b => b.Digitizations.Where(d => d.FormatInfo != null && d.FormatInfo.BitnessId == b.Id), d => $"{d.FormatInfo!.Bitness!.Value} bit/s"),
                //Sampling = await CountGeneric(_context.Samplings, s => s.Digitizations.Where(d => d.FormatInfo != null && d.FormatInfo.SamplingId == s.Id), d => $"{d.FormatInfo!.Sampling!.Value}{(_dsdFreq.Contains(d.FormatInfo.Sampling!.Value) ? " MHz" : " kHz")}"),
                //SourceFormat = await CountGeneric(_context.SourceFormats, s => s.Digitizations),
                //DigitalFormat = await CountGeneric(_context.DigitalFormats, d => d.Digitizations),
                //Adc = await CountGeneric(_context.Adces, e => e.Digitizations, f => $"{f.EquipmentInfo!.Adc!.Manufacturer?.Name} {f.EquipmentInfo.Adc.Name}"),
                //Amplifier = await CountGeneric(_context.Amplifiers, e => e.Digitizations, f => $"{f.EquipmentInfo!.Amplifier!.Manufacturer?.Name} {f.EquipmentInfo.Amplifier.Name}"),
                //Cartridge = await CountGeneric(_context.Cartridges, e => e.Digitizations, f => $"{f.EquipmentInfo!.Cartridge!.Manufacturer?.Name} {f.EquipmentInfo.Cartridge.Name}"),
                //Player = await CountGeneric(_context.Players, e => e.Digitizations, f => $"{f.EquipmentInfo!.Player!.Manufacturer?.Name} {f.EquipmentInfo.Player.Name}"),
                //VinylState = await CountGeneric(_context.VinylStates, v => v.Digitizations, f => f.FormatInfo!.VinylState!.Name),
                //Wire = await CountGeneric(_context.Wires, e => e.Digitizations, f => $"{f.EquipmentInfo!.Wire!.Manufacturer?.Name} {f.EquipmentInfo.Wire.Name}")
            };

            return new Statistic
            {
                Name = JsonSerializer.Serialize(data),
                LastUpdate = DateTime.UtcNow
            };
        }

        private async Task<List<CounterItem>> CountGeneric<TEntity>(
            IQueryable<TEntity> entities,
            Func<TEntity, IEnumerable<Digitization>> digitizationsSelector,
            Func<Digitization, string>? descriptionSelector = null)
            where TEntity : class
        {
            descriptionSelector ??= d =>
            {
                var f = d.FormatInfo;
                return f switch
                {
                    not null when f.Bitness != null => f.Bitness.Value.ToString(),
                    _ => d.Id.ToString()
                };
            };

            return await entities
                .Select(e => new CounterItem
                {
                    Description = descriptionSelector(digitizationsSelector(e).FirstOrDefault()!),
                    Count = digitizationsSelector(e).Count()
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }
    }
}
