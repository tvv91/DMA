using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading;
using Web.Db;
using Web.Interfaces;
using Web.Models;

namespace Web.Implementation
{
    public class StatisticRepository : IStatisticRepository
    {
        private readonly DMADbContext _context;
        private static readonly double[] _dsdFreq = { 2.8, 5.6, 11.2, 22.5 };
        private static readonly SemaphoreSlim _refreshLock = new SemaphoreSlim(1, 1);
        private static DateTime? _lastRefreshAttempt = null;
        private static readonly TimeSpan _refreshCooldown = TimeSpan.FromMinutes(5);

        public StatisticRepository(DMADbContext context) => _context = context;

        public async Task<Statistic> Process()
        {
            var stat = await _context.Statistics.FirstOrDefaultAsync();

            if (stat == null)
            {
                await _refreshLock.WaitAsync();
                try
                {
                    // Double-check after acquiring lock
                    stat = await _context.Statistics.FirstOrDefaultAsync();
                    if (stat == null)
                    {
                        stat = await RefreshStatistic();
                        await _context.Statistics.AddAsync(stat);
                        await _context.SaveChangesAsync();
                    }
                }
                finally
                {
                    _refreshLock.Release();
                }
                return stat;
            }

            // Check if refresh is needed (older than 1 day)
            var needsRefresh = DateTime.UtcNow - stat.LastUpdate > TimeSpan.FromDays(1);
            
            // Also check if we recently attempted a refresh (to prevent multiple concurrent refreshes)
            var canRefresh = _lastRefreshAttempt == null || 
                            DateTime.UtcNow - _lastRefreshAttempt.Value > _refreshCooldown;

            if (needsRefresh && canRefresh)
            {
                await _refreshLock.WaitAsync();
                try
                {
                    // Double-check after acquiring lock
                    stat = await _context.Statistics.FirstOrDefaultAsync();
                    if (stat != null && DateTime.UtcNow - stat.LastUpdate > TimeSpan.FromDays(1))
                    {
                        _lastRefreshAttempt = DateTime.UtcNow;
                        var refreshed = await RefreshStatistic();
                        stat.Name = refreshed.Name;
                        stat.LastUpdate = refreshed.LastUpdate;
                        await _context.SaveChangesAsync();
                    }
                }
                finally
                {
                    _refreshLock.Release();
                }
            }

            return stat;
        }

        private async Task<Statistic> RefreshStatistic()
        {
            var totalEquipment = await _context.Adces.CountAsync() +
                                 await _context.Amplifiers.CountAsync() +
                                 await _context.Cartridges.CountAsync() +
                                 await _context.Players.CountAsync() +
                                 await _context.Wires.CountAsync();

            var data = new StatisticCounters
            {
                TotalAlbums = await _context.Albums.CountAsync(),
                TotalSize = await _context.Digitizations.Where(d => d.FormatInfo != null && d.FormatInfo.Size != null).SumAsync(d => d.FormatInfo!.Size ?? 0),
                StorageCount = await _context.Storages.CountAsync(),
                TotalDigitizations = await _context.Digitizations.CountAsync(),
                TotalArtists = await _context.Artists.CountAsync(),
                TotalEquipment = totalEquipment,
                Genre = await CountGenresAsync(),
                Artist = await CountArtistsAsync(),
                Year = await CountYearsAsync(),
                Country = await CountCountriesAsync(),
                Label = await CountLabelsAsync(),
                Bitness = await CountBitnessAsync(),
                Sampling = await CountSamplingAsync(),
                SourceFormat = await CountSourceFormatsAsync(),
                DigitalFormat = await CountDigitalFormatsAsync(),
                Adc = await CountAdcAsync(),
                Amplifier = await CountAmplifiersAsync(),
                Cartridge = await CountCartridgesAsync(),
                Player = await CountPlayersAsync(),
                VinylState = await CountVinylStatesAsync(),
                Wire = await CountWiresAsync()
            };

            return new Statistic
            {
                Name = JsonSerializer.Serialize(data),
                LastUpdate = DateTime.UtcNow
            };
        }

        private async Task<List<CounterItem>> CountGenresAsync()
        {
            return await _context.Genres
                .Select(g => new CounterItem
                {
                    Description = g.Name,
                    Count = g.Albums.Count
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountArtistsAsync()
        {
            return await _context.Artists
                .Select(a => new CounterItem
                {
                    Description = a.Name,
                    Count = a.Albums.Count
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountYearsAsync()
        {
            return await _context.Years
                .Where(y => y.Digitizations.Any())
                .OrderByDescending(y => y.Value)
                .Select(y => new CounterItem
                {
                    Description = y.Value.ToString(),
                    Count = y.Digitizations.Count
                })
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountCountriesAsync()
        {
            return await _context.Countries
                .Select(c => new CounterItem
                {
                    Description = c.Name,
                    Count = c.Digitizations.Count
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountLabelsAsync()
        {
            return await _context.Labels
                .Select(l => new CounterItem
                {
                    Description = l.Name,
                    Count = l.Digitizations.Count
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountBitnessAsync()
        {
            return await _context.Bitnesses
                .Select(b => new CounterItem
                {
                    Description = $"{b.Value} bit",
                    Count = _context.Digitizations.Count(d => d.FormatInfo != null && d.FormatInfo.BitnessId == b.Id)
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountSamplingAsync()
        {
            return await _context.Samplings
                .Select(s => new CounterItem
                {
                    Description = $"{s.Value}{(_dsdFreq.Contains(s.Value) ? " MHz" : " kHz")}",
                    Count = _context.Digitizations.Count(d => d.FormatInfo != null && d.FormatInfo.SamplingId == s.Id)
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountSourceFormatsAsync()
        {
            return await _context.SourceFormats
                .Select(s => new CounterItem
                {
                    Description = s.Name,
                    Count = _context.Digitizations.Count(d => d.FormatInfo != null && d.FormatInfo.SourceFormatId == s.Id)
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountDigitalFormatsAsync()
        {
            return await _context.DigitalFormats
                .Select(d => new CounterItem
                {
                    Description = d.Name,
                    Count = _context.Digitizations.Count(dig => dig.FormatInfo != null && dig.FormatInfo.DigitalFormatId == d.Id)
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountVinylStatesAsync()
        {
            return await _context.VinylStates
                .Select(v => new CounterItem
                {
                    Description = v.Name,
                    Count = _context.Digitizations.Count(d => d.FormatInfo != null && d.FormatInfo.VinylStateId == v.Id)
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountAdcAsync()
        {
            return await _context.Adces
                .Select(a => new CounterItem
                {
                    Description = a.Manufacturer != null ? $"{a.Manufacturer.Name} {a.Name}" : a.Name,
                    Count = _context.Digitizations.Count(d => d.EquipmentInfo != null && d.EquipmentInfo.AdcId == a.Id)
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountAmplifiersAsync()
        {
            return await _context.Amplifiers
                .Select(a => new CounterItem
                {
                    Description = a.Manufacturer != null ? $"{a.Manufacturer.Name} {a.Name}" : a.Name,
                    Count = _context.Digitizations.Count(d => d.EquipmentInfo != null && d.EquipmentInfo.AmplifierId == a.Id)
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountCartridgesAsync()
        {
            return await _context.Cartridges
                .Select(c => new CounterItem
                {
                    Description = c.Manufacturer != null ? $"{c.Manufacturer.Name} {c.Name}" : c.Name,
                    Count = _context.Digitizations.Count(d => d.EquipmentInfo != null && d.EquipmentInfo.CartridgeId == c.Id)
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountPlayersAsync()
        {
            return await _context.Players
                .Select(p => new CounterItem
                {
                    Description = p.Manufacturer != null ? $"{p.Manufacturer.Name} {p.Name}" : p.Name,
                    Count = _context.Digitizations.Count(d => d.EquipmentInfo != null && d.EquipmentInfo.PlayerId == p.Id)
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        private async Task<List<CounterItem>> CountWiresAsync()
        {
            return await _context.Wires
                .Select(w => new CounterItem
                {
                    Description = w.Manufacturer != null ? $"{w.Manufacturer.Name} {w.Name}" : w.Name,
                    Count = _context.Digitizations.Count(d => d.EquipmentInfo != null && d.EquipmentInfo.WireId == w.Id)
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }
    }
}
