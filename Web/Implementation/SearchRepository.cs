using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Response;

namespace Web.Implementation
{
    public class SearchRepository : ISearchRepository
    {
        private readonly DMADbContext _context;
        private readonly Dictionary<EntityType, Func<string, Task<List<AutocompleteResponse>>>> _searchMap;

        public SearchRepository(DMADbContext ctx)
        {
            _context = ctx;

            _searchMap = new()
            {
                { EntityType.Artist, v => SearchStringAsync(_context.Artists, x => x.Name, v) },
                { EntityType.Genre, v => SearchStringAsync(_context.Genres, x => x.Name, v) },
                { EntityType.Year, v => SearchNumberAsync(_context.Years, x => x.Value, v) },
                { EntityType.Reissue, v => SearchNumberAsync(_context.Reissues, x => x.Value, v) },

                { EntityType.VinylState, v => SearchStringAsync(_context.VinylStates, x => x.Name, v) },
                { EntityType.DigitalFormat, v => SearchStringAsync(_context.DigitalFormats, x => x.Name, v) },
                { EntityType.Bitness, v => SearchNumberAsync(_context.Bitnesses, x => x.Value, v) },
                { EntityType.Sampling, v => SearchNumberAsync(_context.Samplings, x => x.Value, v) },
                { EntityType.SourceFormat, v => SearchStringAsync(_context.SourceFormats, x => x.Name, v) },

                { EntityType.Player, v => SearchStringAsync(_context.Players, x => x.Name, v) },
                { EntityType.Cartridge, v => SearchStringAsync(_context.Cartridges, x => x.Name, v) },
                { EntityType.Amplifier, v => SearchStringAsync(_context.Amplifiers, x => x.Name, v) },
                { EntityType.Adc, v => SearchStringAsync(_context.Adces, x => x.Name, v) },
                { EntityType.Wire, v => SearchStringAsync(_context.Wires, x => x.Name, v) },

                { EntityType.PlayerManufacturer, v => SearchManufacturerAsync(EntityType.PlayerManufacturer, v) },
                { EntityType.CartridgeManufacturer, v => SearchManufacturerAsync(EntityType.CartridgeManufacturer, v) },
                { EntityType.AmplifierManufacturer, v => SearchManufacturerAsync(EntityType.AmplifierManufacturer, v) },
                { EntityType.AdcManufacturer, v => SearchManufacturerAsync(EntityType.AdcManufacturer, v) },
                { EntityType.WireManufacturer, v => SearchManufacturerAsync(EntityType.WireManufacturer, v) },
            };
        }

        public async Task<List<AutocompleteResponse>> SearchAsync(EntityType entityType, string value)
        {
            if (_searchMap.TryGetValue(entityType, out var func))
            {
                return await func(value);
            }

            return new List<AutocompleteResponse>();
        }

        private async Task<List<AutocompleteResponse>> SearchStringAsync<TEntity>(
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
                    Label = EF.Property<string>(x, propertyName)
                })
                .ToListAsync();
        }

        private async Task<List<AutocompleteResponse>> SearchNumberAsync<TEntity, TProperty>(
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
                .Select(x => new AutocompleteResponse { Label = func(x)?.ToString() ?? "" })
                .ToList();
        }

        private async Task<List<AutocompleteResponse>> SearchManufacturerAsync(EntityType type, string value)
        {
            var likePattern = $"%{value}%";

            return await _context.Manufacturer
                .AsNoTracking()
                .Where(m => m.Type == type && EF.Functions.Like(m.Name, likePattern))
                .Select(m => new AutocompleteResponse { Label = m.Name })
                .ToListAsync();
        }
    }
}
