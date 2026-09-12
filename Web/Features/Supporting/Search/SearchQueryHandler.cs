using System.Globalization;
using System.Linq.Expressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Enums;
using Web.Infrastructure.Persistence;

namespace Web.Features.Supporting.Search;

public sealed class SearchQueryHandler(Context context) : IRequestHandler<SearchQuery, List<AutocompleteResponse>>
{
    private const int MaxItems = 10;

    public async Task<List<AutocompleteResponse>> Handle(SearchQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Value))
        {
            return request.EntityType switch
            {
                EntityType.VinylState => await context.VinylStates.AsNoTracking().OrderBy(x => x.Name).Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name }).Take(MaxItems).ToListAsync(cancellationToken),
                EntityType.DigitalFormat => await context.DigitalFormats.AsNoTracking().OrderBy(x => x.Name).Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name }).Take(MaxItems).ToListAsync(cancellationToken),
                EntityType.SourceFormat => await context.SourceFormats.AsNoTracking().OrderBy(x => x.Name).Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name }).Take(MaxItems).ToListAsync(cancellationToken),
                EntityType.Country => await context.Countries.AsNoTracking().OrderBy(x => x.Name).Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name }).Take(MaxItems).ToListAsync(cancellationToken),
                EntityType.Label => await context.Labels.AsNoTracking().OrderBy(x => x.Name).Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name }).Take(MaxItems).ToListAsync(cancellationToken),
                EntityType.Storage => await context.Storages.AsNoTracking().OrderBy(x => x.Name).Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name }).Take(MaxItems).ToListAsync(cancellationToken),
                EntityType.Player => await context.Players.AsNoTracking().OrderBy(x => x.Name).Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name }).Take(MaxItems).ToListAsync(cancellationToken),
                EntityType.Cartridge => await context.Cartridges.AsNoTracking().OrderBy(x => x.Name).Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name }).Take(MaxItems).ToListAsync(cancellationToken),
                EntityType.Amplifier => await context.Amplifiers.AsNoTracking().OrderBy(x => x.Name).Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name }).Take(MaxItems).ToListAsync(cancellationToken),
                EntityType.Adc => await context.Adces.AsNoTracking().OrderBy(x => x.Name).Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name }).Take(MaxItems).ToListAsync(cancellationToken),
                EntityType.Wire => await context.Wires.AsNoTracking().OrderBy(x => x.Name).Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name }).Take(MaxItems).ToListAsync(cancellationToken),
                EntityType.PlayerManufacturer or EntityType.CartridgeManufacturer or EntityType.AmplifierManufacturer or EntityType.AdcManufacturer or EntityType.WireManufacturer => await context.Manufacturer.AsNoTracking().OrderBy(x => x.Name).Select(x => new AutocompleteResponse { Label = x.Name, Value = x.Name }).Take(MaxItems).ToListAsync(cancellationToken),
                EntityType.Sampling => (await context.Samplings.AsNoTracking().OrderBy(x => x.Value).Select(x => x.Value).Take(MaxItems).ToListAsync(cancellationToken)).ConvertAll(x => new AutocompleteResponse { Label = $"{x} kHz", Value = x.ToString(CultureInfo.InvariantCulture) }),
                EntityType.Bitness => (await context.Bitnesses.AsNoTracking().OrderBy(x => x.Value).Select(x => x.Value).Take(MaxItems).ToListAsync(cancellationToken)).ConvertAll(x => new AutocompleteResponse { Label = x.ToString(CultureInfo.InvariantCulture), Value = x.ToString(CultureInfo.InvariantCulture) }),
                EntityType.Year => (await context.Years.AsNoTracking().OrderBy(x => x.Value).Select(x => x.Value).Take(MaxItems).ToListAsync(cancellationToken)).ConvertAll(x => new AutocompleteResponse { Label = x.ToString(CultureInfo.InvariantCulture), Value = x.ToString(CultureInfo.InvariantCulture) }),
                EntityType.Reissue => (await context.Reissues.AsNoTracking().OrderBy(x => x.Value).Select(x => x.Value).Take(MaxItems).ToListAsync(cancellationToken)).ConvertAll(x => new AutocompleteResponse { Label = x.ToString(CultureInfo.InvariantCulture), Value = x.ToString(CultureInfo.InvariantCulture) }),
                _ => []
            };
        }

        var value = request.Value.Trim();
        return request.EntityType switch
        {
            EntityType.Artist => await SearchString(context.Artists, x => x.Name, value, cancellationToken),
            EntityType.Genre => await SearchString(context.Genres, x => x.Name, value, cancellationToken),
            EntityType.VinylState => await SearchString(context.VinylStates, x => x.Name, value, cancellationToken),
            EntityType.DigitalFormat => await SearchString(context.DigitalFormats, x => x.Name, value, cancellationToken),
            EntityType.SourceFormat => await SearchString(context.SourceFormats, x => x.Name, value, cancellationToken),
            EntityType.Country => await SearchString(context.Countries, x => x.Name, value, cancellationToken),
            EntityType.Label => await SearchString(context.Labels, x => x.Name, value, cancellationToken),
            EntityType.Storage => await SearchString(context.Storages, x => x.Name, value, cancellationToken),
            EntityType.Player => await SearchString(context.Players, x => x.Name, value, cancellationToken),
            EntityType.Cartridge => await SearchString(context.Cartridges, x => x.Name, value, cancellationToken),
            EntityType.Amplifier => await SearchString(context.Amplifiers, x => x.Name, value, cancellationToken),
            EntityType.Adc => await SearchString(context.Adces, x => x.Name, value, cancellationToken),
            EntityType.Wire => await SearchString(context.Wires, x => x.Name, value, cancellationToken),
            EntityType.PlayerManufacturer or EntityType.CartridgeManufacturer or EntityType.AmplifierManufacturer or EntityType.AdcManufacturer or EntityType.WireManufacturer => await SearchString(context.Manufacturer, x => x.Name, value, cancellationToken),
            EntityType.Year => await SearchNumber(context.Years, x => x.Value, value, cancellationToken),
            EntityType.Reissue => await SearchNumber(context.Reissues, x => x.Value, value, cancellationToken),
            EntityType.Bitness => await SearchNumber(context.Bitnesses, x => x.Value, value, cancellationToken),
            EntityType.Sampling => (await context.Samplings.AsNoTracking().ToListAsync(cancellationToken)).Where(x => x.Value.ToString(CultureInfo.InvariantCulture).Contains(value, StringComparison.OrdinalIgnoreCase)).Take(MaxItems).Select(x => new AutocompleteResponse { Label = $"{x.Value} kHz", Value = x.Value.ToString(CultureInfo.InvariantCulture) }).ToList(),
            _ => []
        };
    }

    private static async Task<List<AutocompleteResponse>> SearchString<TEntity>(IQueryable<TEntity> query, Expression<Func<TEntity, string>> selector, string value, CancellationToken cancellationToken) where TEntity : class
    {
        var property = ((MemberExpression)selector.Body).Member.Name;
        return await query.AsNoTracking().Where(x => EF.Functions.Like(EF.Property<string>(x, property), $"%{value}%")).Select(x => new AutocompleteResponse { Label = EF.Property<string>(x, property), Value = EF.Property<string>(x, property) }).Distinct().Take(MaxItems).ToListAsync(cancellationToken);
    }

    private static async Task<List<AutocompleteResponse>> SearchNumber<TEntity>(IQueryable<TEntity> query, Expression<Func<TEntity, int>> selector, string value, CancellationToken cancellationToken) where TEntity : class
    {
        var property = selector.Compile();
        return (await query.AsNoTracking().ToListAsync(cancellationToken)).Where(x => property(x).ToString(CultureInfo.InvariantCulture).Contains(value, StringComparison.OrdinalIgnoreCase)).Take(MaxItems).Select(x => { var text = property(x).ToString(CultureInfo.InvariantCulture); return new AutocompleteResponse { Label = text, Value = text }; }).ToList();
    }
}
