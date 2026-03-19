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
            { EntityType.Sampling, v => SearchNumberAsync(context.Samplings, x => x.Value, v) },
            { EntityType.SourceFormat, v => SearchStringAsync(context.SourceFormats, x => x.Name, v) },
            { EntityType.Country, v => SearchStringAsync(context.Countries, x => x.Name, v) },
            { EntityType.Label, v => SearchStringAsync(context.Labels, x => x.Name, v) },
            { EntityType.Storage, v => SearchStringAsync(context.Storages, x => x.Data, v) },

            { EntityType.Player, v => SearchStringAsync(context.Players, x => x.Name, v) },
            { EntityType.Cartridge, v => SearchStringAsync(context.Cartridges, x => x.Name, v) },
            { EntityType.Amplifier, v => SearchStringAsync(context.Amplifiers, x => x.Name, v) },
            { EntityType.Adc, v => SearchStringAsync(context.Adces, x => x.Name, v) },
            { EntityType.Wire, v => SearchStringAsync(context.Wires, x => x.Name, v) },

            { EntityType.PlayerManufacturer, v => SearchManufacturerAsync(context, EntityType.PlayerManufacturer, v) },
            { EntityType.CartridgeManufacturer, v => SearchManufacturerAsync(context, EntityType.CartridgeManufacturer, v) },
            { EntityType.AmplifierManufacturer, v => SearchManufacturerAsync(context, EntityType.AmplifierManufacturer, v) },
            { EntityType.AdcManufacturer, v => SearchManufacturerAsync(context, EntityType.AdcManufacturer, v) },
            { EntityType.WireManufacturer, v => SearchManufacturerAsync(context, EntityType.WireManufacturer, v) },
        };

        public async Task<List<AutocompleteResponse>> SearchAsync(EntityType entityType, string value)
        {
            if (_searchMap.TryGetValue(entityType, out var func))
            {
                return await func(value);
            }

            return [];
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
                .ToList();
        }

        private static async Task<List<AutocompleteResponse>> SearchManufacturerAsync(Context context, EntityType type, string value)
        {
            var likePattern = $"%{value}%";

            return await context.Manufacturer
                .AsNoTracking()
                .Where(m => m.Type == type && EF.Functions.Like(m.Name, likePattern))
                .Select(m => new AutocompleteResponse { Label = m.Name, Value = m.Name })
                .Distinct()
                .ToListAsync();
        }
    }
}

