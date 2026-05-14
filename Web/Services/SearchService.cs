using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Response;

namespace Web.Services
{
    public class SearchService(Context context) : ISearchService
    {
        private const int AutocompleteMaxItems = 10;

        private readonly Context _context = context;
        private readonly Dictionary<EntityType, Func<string, Task<List<AutocompleteResponse>>>> _searchMap = new()
        {
            { EntityType.Artist, v => SearchStringAsync(context.Artists, x => x.Name, v) },
            { EntityType.Genre, v => SearchStringAsync(context.Genres, x => x.Name, v) },
            { EntityType.Year, v => SearchNumberAsync(context.Years, x => x.Value, v) },
            { EntityType.Reissue, v => SearchNumberAsync(context.Reissues, x => x.Value, v) },

            { EntityType.VinylState, v => SearchStringAsync(context.VinylStates, x => x.Name, v) },
            { EntityType.DigitalFormat, v => SearchStringAsync(context.DigitalFormats, x => x.Name, v) },
            { EntityType.Bitness, v => SearchNumberAsync(context.Bitnesses, x => x.Value, v) },
            { EntityType.Sampling, v => SearchSamplingAsync(context, v) },
            { EntityType.SourceFormat, v => SearchStringAsync(context.SourceFormats, x => x.Name, v) },
            { EntityType.Country, v => SearchStringAsync(context.Countries, x => x.Name, v) },
            { EntityType.Label, v => SearchStringAsync(context.Labels, x => x.Name, v) },
            { EntityType.Storage, v => SearchStringAsync(context.Storages, x => x.Name, v) },

            { EntityType.Player, v => SearchStringAsync(context.Players, x => x.Name, v) },
            { EntityType.Cartridge, v => SearchStringAsync(context.Cartridges, x => x.Name, v) },
            { EntityType.Amplifier, v => SearchStringAsync(context.Amplifiers, x => x.Name, v) },
            { EntityType.Adc, v => SearchStringAsync(context.Adces, x => x.Name, v) },
            { EntityType.Wire, v => SearchStringAsync(context.Wires, x => x.Name, v) },

            { EntityType.PlayerManufacturer, v => SearchManufacturerAsync(context, v) },
            { EntityType.CartridgeManufacturer, v => SearchManufacturerAsync(context, v) },
            { EntityType.AmplifierManufacturer, v => SearchManufacturerAsync(context, v) },
            { EntityType.AdcManufacturer, v => SearchManufacturerAsync(context, v) },
            { EntityType.WireManufacturer, v => SearchManufacturerAsync(context, v) },
        };

        public async Task<List<AutocompleteResponse>> SearchAsync(EntityType entityType, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return await ListAllForAutocompleteAsync(entityType);

            if (_searchMap.TryGetValue(entityType, out var func))
                return await func(value);

            return [];
        }

        /// <summary>Full list for combobox “open on focus” (empty query). Only entity types used that way; others return [].</summary>
        private async Task<List<AutocompleteResponse>> ListAllForAutocompleteAsync(EntityType entityType)
        {
            return entityType switch
            {
                EntityType.VinylState => await _context.VinylStates.AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name })
                    .Take(AutocompleteMaxItems)
                    .ToListAsync(),
                EntityType.DigitalFormat => await _context.DigitalFormats.AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name })
                    .Take(AutocompleteMaxItems)
                    .ToListAsync(),
                EntityType.SourceFormat => await _context.SourceFormats.AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name })
                    .Take(AutocompleteMaxItems)
                    .ToListAsync(),
                EntityType.Country => await _context.Countries.AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name })
                    .Take(AutocompleteMaxItems)
                    .ToListAsync(),
                EntityType.Label => await _context.Labels.AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name })
                    .Take(AutocompleteMaxItems)
                    .ToListAsync(),
                EntityType.Bitness => (await _context.Bitnesses.AsNoTracking()
                    .OrderBy(x => x.Value)
                    .Select(x => x.Value)
                    .Take(AutocompleteMaxItems)
                    .ToListAsync())
                    .ConvertAll(v =>
                    {
                        var s = v.ToString(CultureInfo.InvariantCulture);
                        return new AutocompleteResponse { Label = s, Value = s };
                    }),
                EntityType.Year => (await _context.Years.AsNoTracking()
                    .OrderBy(x => x.Value)
                    .Select(x => x.Value)
                    .Take(AutocompleteMaxItems)
                    .ToListAsync())
                    .ConvertAll(v =>
                    {
                        var s = v.ToString(CultureInfo.InvariantCulture);
                        return new AutocompleteResponse { Label = s, Value = s };
                    }),
                EntityType.Reissue => (await _context.Reissues.AsNoTracking()
                    .OrderBy(x => x.Value)
                    .Select(x => x.Value)
                    .Take(AutocompleteMaxItems)
                    .ToListAsync())
                    .ConvertAll(v =>
                    {
                        var s = v.ToString(CultureInfo.InvariantCulture);
                        return new AutocompleteResponse { Label = s, Value = s };
                    }),
                EntityType.Sampling => await ListAllSamplingForAutocompleteAsync(_context, AutocompleteMaxItems),
                EntityType.Player => await _context.Players.AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name })
                    .Take(AutocompleteMaxItems)
                    .ToListAsync(),
                EntityType.Cartridge => await _context.Cartridges.AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name })
                    .Take(AutocompleteMaxItems)
                    .ToListAsync(),
                EntityType.Amplifier => await _context.Amplifiers.AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name })
                    .Take(AutocompleteMaxItems)
                    .ToListAsync(),
                EntityType.Adc => await _context.Adces.AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name })
                    .Take(AutocompleteMaxItems)
                    .ToListAsync(),
                EntityType.Wire => await _context.Wires.AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name })
                    .Take(AutocompleteMaxItems)
                    .ToListAsync(),
                EntityType.PlayerManufacturer or EntityType.CartridgeManufacturer or EntityType.AmplifierManufacturer
                    or EntityType.AdcManufacturer or EntityType.WireManufacturer => await _context.Manufacturer.AsNoTracking()
                    .OrderBy(m => m.Name)
                    .Select(m => new AutocompleteResponse { Label = m.Name, Value = m.Name })
                    .Take(AutocompleteMaxItems)
                    .ToListAsync(),
                _ => [],
            };
        }

        private static async Task<List<AutocompleteResponse>> ListAllSamplingForAutocompleteAsync(Context context, int take)
        {
            var values = await context.Samplings.AsNoTracking()
                .OrderBy(x => x.Value)
                .Select(x => x.Value)
                .Take(take)
                .ToListAsync();
            return values.ConvertAll(v => new AutocompleteResponse
            {
                Value = SamplingAutocompleteValue(v),
                Label = SamplingAutocompleteLabel(v)
            });
        }

        private static async Task<List<AutocompleteResponse>> SearchSamplingAsync(Context context, string value)
        {
            var needle = value.Trim();
            var list = await context.Samplings.AsNoTracking().ToListAsync();
            return list
                .Where(s =>
                {
                    var token = SamplingAutocompleteValue(s.Value);
                    var label = SamplingAutocompleteLabel(s.Value);
                    return token.Contains(needle, StringComparison.OrdinalIgnoreCase)
                        || label.Contains(needle, StringComparison.OrdinalIgnoreCase);
                })
                .OrderBy(s => s.Value)
                .Take(AutocompleteMaxItems)
                .Select(s => new AutocompleteResponse
                {
                    Value = SamplingAutocompleteValue(s.Value),
                    Label = SamplingAutocompleteLabel(s.Value)
                })
                .ToList();
        }

        /// <summary>DSD nominal rates in MHz (aligned with seeded rows and <see cref="StatisticService"/>).</summary>
        private static ReadOnlySpan<double> DsdSamplingMhzValues => [2.8, 5.6, 11.2, 22.5];

        private static bool IsDsdSamplingMhz(double value)
        {
            foreach (var d in DsdSamplingMhzValues)
            {
                if (Math.Abs(value - d) < 0.0001)
                    return true;
            }

            return false;
        }

        private static string SamplingAutocompleteValue(double v) =>
            v.ToString(CultureInfo.InvariantCulture);

        private static string SamplingAutocompleteLabel(double v)
        {
            var n = SamplingAutocompleteValue(v);
            return IsDsdSamplingMhz(v) ? $"{n} MHz" : $"{n} kHz";
        }

        private static async Task<List<AutocompleteResponse>> SearchStringAsync<TEntity>(
            IQueryable<TEntity> query,
            Expression<Func<TEntity, string>> selector,
            string value)
            where TEntity : class
        {
            var likePattern = $"%{value}%";
            var propertyName = ((MemberExpression)selector.Body).Member.Name;

            return await query
                .AsNoTracking()
                .Where(x => EF.Functions.Like(EF.Property<string>(x, propertyName), likePattern))
                .Select(x => new AutocompleteResponse
                {
                    Label = EF.Property<string>(x, propertyName),
                    Value = EF.Property<string>(x, propertyName)
                })
                .Distinct()
                .Take(AutocompleteMaxItems)
                .ToListAsync();
        }

        private static async Task<List<AutocompleteResponse>> SearchNumberAsync<TEntity, TProperty>(
            IQueryable<TEntity> query,
            Expression<Func<TEntity, TProperty>> selector,
            string value)
            where TEntity : class
        {
            var func = selector.Compile();
            var list = await query.AsNoTracking().ToListAsync();

            return list
                .Where(x => func(x)?.ToString()?
                .Contains(value, StringComparison.OrdinalIgnoreCase) == true)
                .Select(x => {
                    var val = func(x)?.ToString() ?? "";
                    return new AutocompleteResponse { Label = val, Value = val };
                })
                .Distinct()
                .Take(AutocompleteMaxItems)
                .ToList();
        }

        private static async Task<List<AutocompleteResponse>> SearchManufacturerAsync(Context context, string value)
        {
            var likePattern = $"%{value}%";

            return await context.Manufacturer
                .AsNoTracking()
                .Where(m => EF.Functions.Like(m.Name, likePattern))
                .Select(m => new AutocompleteResponse { Label = m.Name, Value = m.Name })
                .Distinct()
                .Take(AutocompleteMaxItems)
                .ToListAsync();
        }
    }
}

