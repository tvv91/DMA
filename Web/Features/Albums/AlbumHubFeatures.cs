using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.Request;

namespace Web.Features.Albums;

public sealed record CheckAlbumQuery(int AlbumId, string Album, string Artist, string Source) : IRequest<(int Status, int Id)>;
public sealed record AddReleaseCommand(CreateUpdateReleaseRequest Request) : IRequest<ReleaseHubResult>;
public sealed record UpdateReleaseCommand(CreateUpdateReleaseRequest Request) : IRequest<ReleaseHubResult>;
public sealed record DeleteReleaseCommand(int ReleaseId) : IRequest<ReleaseHubResult>;
public sealed record GetTechnicalInfoIconsQuery(int ReleaseId) : IRequest<TechnicalInfoResult?>;
public sealed record ReleaseHubResult(bool Success, string Error, int AlbumId, IReadOnlyList<ReleaseHubDto> Releases);
public sealed record TechnicalInfoResult(IReadOnlyDictionary<string, (int? Id, EntityType Type, bool Resource)> Values);

public sealed record ReleaseHubDto(
    int Id, string? VinylState, int? Bitness, double? Sampling, string? DigitalFormat, string? SourceFormat,
    string? Player, string? PlayerManufacturer, string? Cartridge, string? CartridgeManufacturer,
    string? Amplifier, string? AmplifierManufacturer, string? Adc, string? AdcManufacturer,
    string? Wire, string? WireManufacturer, string? Source, int? Year, int? Reissue, string? Country,
    string? Label, string? Storage, string? Discogs, double? Size, bool IsFirstPress);

internal static class ReleaseHubQueries
{
    internal static IQueryable<Release> WithDetails(IQueryable<Release> query) => query
        .Include(x => x.Year).Include(x => x.Reissue).Include(x => x.Country).Include(x => x.Label).Include(x => x.Storage)
        .Include(x => x.FormatInfo).ThenInclude(x => x!.VinylState)
        .Include(x => x.FormatInfo).ThenInclude(x => x!.Bitness)
        .Include(x => x.FormatInfo).ThenInclude(x => x!.Sampling)
        .Include(x => x.FormatInfo).ThenInclude(x => x!.DigitalFormat)
        .Include(x => x.FormatInfo).ThenInclude(x => x!.SourceFormat)
        .Include(x => x.EquipmentInfo).ThenInclude(x => x!.Player).ThenInclude(x => x!.Manufacturer)
        .Include(x => x.EquipmentInfo).ThenInclude(x => x!.Cartridge).ThenInclude(x => x!.Manufacturer)
        .Include(x => x.EquipmentInfo).ThenInclude(x => x!.Amplifier).ThenInclude(x => x!.Manufacturer)
        .Include(x => x.EquipmentInfo).ThenInclude(x => x!.Adc).ThenInclude(x => x!.Manufacturer)
        .Include(x => x.EquipmentInfo).ThenInclude(x => x!.Wire).ThenInclude(x => x!.Manufacturer);

    internal static ReleaseHubDto ToDto(Release x) => new(x.Id, x.FormatInfo?.VinylState?.Name, x.FormatInfo?.Bitness?.Value, x.FormatInfo?.Sampling?.Value, x.FormatInfo?.DigitalFormat?.Name, x.FormatInfo?.SourceFormat?.Name,
        x.EquipmentInfo?.Player?.Name, x.EquipmentInfo?.Player?.Manufacturer?.Name, x.EquipmentInfo?.Cartridge?.Name, x.EquipmentInfo?.Cartridge?.Manufacturer?.Name,
        x.EquipmentInfo?.Amplifier?.Name, x.EquipmentInfo?.Amplifier?.Manufacturer?.Name, x.EquipmentInfo?.Adc?.Name, x.EquipmentInfo?.Adc?.Manufacturer?.Name,
        x.EquipmentInfo?.Wire?.Name, x.EquipmentInfo?.Wire?.Manufacturer?.Name, x.Source, x.Year?.Value, x.Reissue?.Value, x.Country?.Name, x.Label?.Name, x.Storage?.Name, x.Discogs, x.Size, x.IsFirstPress ?? false);
}

public sealed class CheckAlbumQueryHandler(Context context) : IRequestHandler<CheckAlbumQuery, (int Status, int Id)>
{
    public async Task<(int Status, int Id)> Handle(CheckAlbumQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Album) || string.IsNullOrWhiteSpace(request.Artist)) return (0, 0);
        var album = await context.Albums.Include(x => x.Artist).AsNoTracking().FirstOrDefaultAsync(x => x.Title == request.Album.Trim() && x.Artist != null && x.Artist.Name == request.Artist.Trim(), cancellationToken);
        if (album is null) return (0, 0);
        if (album.Id != request.AlbumId) return (1, album.Id);
        if (!string.IsNullOrWhiteSpace(request.Source) && await context.Releases.AnyAsync(x => x.AlbumId == album.Id && x.Source == request.Source, cancellationToken)) return (100, album.Id);
        return (0, 0);
    }
}

public sealed class AddReleaseCommandHandler(Context context, TimeProvider timeProvider) : IRequestHandler<AddReleaseCommand, ReleaseHubResult>
{
    public async Task<ReleaseHubResult> Handle(AddReleaseCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var album = request.AlbumId == 0 ? await CreateOrFindAlbum(request, cancellationToken) : await context.Albums.FindAsync([request.AlbumId], cancellationToken);
        if (album is null) return new(false, "Album not found", 0, []);
        var release = await MapRelease(album.Id, request, cancellationToken);
        context.Releases.Add(release);
        await context.SaveChangesAsync(cancellationToken);
        return new(true, string.Empty, album.Id, await GetDtos(album.Id, cancellationToken));
    }

    internal async Task<Album?> CreateOrFindAlbum(CreateUpdateReleaseRequest request, CancellationToken ct)
    {
        var title = request.Album.Trim(); var artistName = request.Artist.Trim();
        var album = await context.Albums.Include(x => x.Artist).FirstOrDefaultAsync(x => x.Title == title && x.Artist != null && x.Artist.Name == artistName, ct);
        if (album is not null) return album;
        var artist = await context.Artists.FirstOrDefaultAsync(x => x.Name == artistName, ct) ?? new Artist { Name = artistName };
        if (artist.Id == 0) context.Artists.Add(artist);
        var genreName = request.Genre.Trim();
        var genre = await context.Genres.FirstOrDefaultAsync(x => x.Name == genreName, ct) ?? new Genre { Name = genreName };
        if (genre.Id == 0) context.Genres.Add(genre);
        album = new Album { Title = title, Artist = artist, Genre = genre, AddedDate = timeProvider.GetLocalNow().LocalDateTime };
        context.Albums.Add(album); await context.SaveChangesAsync(ct); return album;
    }

    internal async Task<Release> MapRelease(int albumId, CreateUpdateReleaseRequest r, CancellationToken ct)
    {
        var x = new Release { AlbumId = albumId, AddedDate = timeProvider.GetLocalNow().LocalDateTime, Source = r.Source, Discogs = r.Discogs, IsFirstPress = r.IsFirstPress, Size = r.Size };
        x.YearId = (await Find(context.Years, y => y.Value == r.Year, r.Year, v => new Year { Value = v }, ct))?.Id;
        x.ReissueId = (await Find(context.Reissues, y => y.Value == r.Reissue, r.Reissue, v => new Reissue { Value = v }, ct))?.Id;
        x.CountryId = (await FindText(context.Countries, r.Country, (e, v) => e.Name == v, v => new Country { Name = v }, ct))?.Id;
        x.LabelId = (await FindText(context.Labels, r.Label, (e, v) => e.Name == v, v => new Label { Name = v }, ct))?.Id;
        x.StorageId = (await FindText(context.Storages, r.Storage, (e, v) => e.Name == v, v => new Storage { Name = v }, ct))?.Id;
        x.FormatInfo = new FormatInfo {
            BitnessId = (await Find(context.Bitnesses, y => y.Value == r.Bitness, r.Bitness, v => new Bitness { Value = v }, ct))?.Id,
            SamplingId = (await Find(context.Samplings, y => y.Value == r.Sampling, r.Sampling, v => new Sampling { Value = v }, ct))?.Id,
            DigitalFormatId = (await FindText(context.DigitalFormats, r.DigitalFormat, (e, v) => e.Name == v, v => new DigitalFormat { Name = v }, ct))?.Id,
            SourceFormatId = (await FindText(context.SourceFormats, r.SourceFormat, (e, v) => e.Name == v, v => new SourceFormat { Name = v }, ct))?.Id,
            VinylStateId = (await FindText(context.VinylStates, r.VinylState, (e, v) => e.Name == v, v => new VinylState { Name = v }, ct))?.Id
        };
        x.EquipmentInfo = new EquipmentInfo {
            PlayerId = (await Equipment(context.Players, r.Player, r.PlayerManufacturer, ct))?.Id,
            CartridgeId = (await Equipment(context.Cartridges, r.Cartridge, r.CartridgeManufacturer, ct))?.Id,
            AmplifierId = (await Equipment(context.Amplifiers, r.Amplifier, r.AmplifierManufacturer, ct))?.Id,
            AdcId = (await Equipment(context.Adces, r.Adc, r.AdcManufacturer, ct))?.Id,
            WireId = (await Equipment(context.Wires, r.Wire, r.WireManufacturer, ct))?.Id
        };
        return x;
    }

    private async Task<T?> Find<T, V>(DbSet<T> set, Expression<Func<T, bool>> predicate, V? value, Func<V, T> create, CancellationToken ct) where T : class where V : struct { if (!value.HasValue) return null; var x = await set.FirstOrDefaultAsync(predicate, ct); if (x is not null) return x; x = create(value.Value); set.Add(x); await context.SaveChangesAsync(ct); return x; }
    private async Task<T?> FindText<T>(DbSet<T> set, string? value, Func<T, string, bool> match, Func<string, T> create, CancellationToken ct) where T : class { if (string.IsNullOrWhiteSpace(value)) return null; var v = value.Trim(); var x = await set.FirstOrDefaultAsync(e => EF.Property<string>(e, "Name") == v, ct); if (x is not null) return x; x = create(v); set.Add(x); await context.SaveChangesAsync(ct); return x; }
    private async Task<T?> Equipment<T>(DbSet<T> set, string? value, string? manufacturer, CancellationToken ct)
        where T : class, IEquipment, new()
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        var name = value.Trim();
        var equipment = await set.Include(e => e.Manufacturer).FirstOrDefaultAsync(e => e.Name == name, ct);
        var manufacturerEntity = string.IsNullOrWhiteSpace(manufacturer)
            ? null
            : await context.Manufacturer.FirstOrDefaultAsync(e => e.Name == manufacturer.Trim(), ct)
                ?? new Manufacturer { Name = manufacturer.Trim() };

        if (manufacturerEntity?.Id == 0)
        {
            context.Manufacturer.Add(manufacturerEntity);
            await context.SaveChangesAsync(ct);
        }

        if (equipment is null)
        {
            equipment = new T { Name = name, Manufacturer = manufacturerEntity };
            set.Add(equipment);
            await context.SaveChangesAsync(ct);
        }
        else if (manufacturerEntity is not null && equipment.ManufacturerId != manufacturerEntity.Id)
        {
            equipment.ManufacturerId = manufacturerEntity.Id;
            await context.SaveChangesAsync(ct);
        }

        return equipment;
    }
    private Task<List<ReleaseHubDto>> GetDtos(int albumId, CancellationToken ct) => ReleaseHubQueries.WithDetails(context.Releases.AsNoTracking().Where(x => x.AlbumId == albumId)).Select(x => new ReleaseHubDto(x.Id, x.FormatInfo!.VinylState!.Name, x.FormatInfo!.Bitness!.Value, x.FormatInfo!.Sampling!.Value, x.FormatInfo!.DigitalFormat!.Name, x.FormatInfo!.SourceFormat!.Name, x.EquipmentInfo!.Player!.Name, x.EquipmentInfo!.Player!.Manufacturer!.Name, x.EquipmentInfo!.Cartridge!.Name, x.EquipmentInfo!.Cartridge!.Manufacturer!.Name, x.EquipmentInfo!.Amplifier!.Name, x.EquipmentInfo!.Amplifier!.Manufacturer!.Name, x.EquipmentInfo!.Adc!.Name, x.EquipmentInfo!.Adc!.Manufacturer!.Name, x.EquipmentInfo!.Wire!.Name, x.EquipmentInfo!.Wire!.Manufacturer!.Name, x.Source, x.Year!.Value, x.Reissue!.Value, x.Country!.Name, x.Label!.Name, x.Storage!.Name, x.Discogs, x.Size, x.IsFirstPress ?? false)).ToListAsync(ct);
}

public sealed class UpdateReleaseCommandHandler(Context context, TimeProvider timeProvider) : IRequestHandler<UpdateReleaseCommand, ReleaseHubResult>
{
    public async Task<ReleaseHubResult> Handle(UpdateReleaseCommand command, CancellationToken ct) { var r = command.Request; var existing = await context.Releases.FirstOrDefaultAsync(x => x.Id == r.ReleaseId, ct); if (existing is null) return new(false, "Release not found", 0, []); var add = new AddReleaseCommandHandler(context, timeProvider); var mapped = await add.MapRelease(existing.AlbumId, r, ct); existing.Source = mapped.Source; existing.Discogs = mapped.Discogs; existing.IsFirstPress = mapped.IsFirstPress; existing.Size = mapped.Size; existing.YearId = mapped.YearId; existing.ReissueId = mapped.ReissueId; existing.CountryId = mapped.CountryId; existing.LabelId = mapped.LabelId; existing.StorageId = mapped.StorageId; existing.FormatInfo = mapped.FormatInfo; existing.EquipmentInfo = mapped.EquipmentInfo; existing.UpdateDate = timeProvider.GetUtcNow().UtcDateTime; await context.SaveChangesAsync(ct); var releases = await ReleaseHubQueries.WithDetails(context.Releases.AsNoTracking().Where(x => x.AlbumId == existing.AlbumId)).Select(x => ReleaseHubQueries.ToDto(x)).ToListAsync(ct); return new(true, string.Empty, existing.AlbumId, releases); }
}

public sealed class DeleteReleaseCommandHandler(Context context) : IRequestHandler<DeleteReleaseCommand, ReleaseHubResult>
{
    public async Task<ReleaseHubResult> Handle(DeleteReleaseCommand command, CancellationToken ct) { var release = await context.Releases.FindAsync([command.ReleaseId], ct); if (release is null) return new(false, "Release not found", 0, []); var albumId = release.AlbumId; context.Releases.Remove(release); await context.SaveChangesAsync(ct); var releases = await ReleaseHubQueries.WithDetails(context.Releases.AsNoTracking().Where(x => x.AlbumId == albumId)).Select(x => ReleaseHubQueries.ToDto(x)).ToListAsync(ct); return new(true, string.Empty, albumId, releases); }
}

public sealed class GetTechnicalInfoIconsQueryHandler(Context context) : IRequestHandler<GetTechnicalInfoIconsQuery, TechnicalInfoResult?>
{
    public async Task<TechnicalInfoResult?> Handle(GetTechnicalInfoIconsQuery request, CancellationToken ct) { var r = await context.Releases.Include(x => x.FormatInfo).Include(x => x.EquipmentInfo).AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.ReleaseId, ct); if (r is null) return null; return new(new Dictionary<string, (int?, EntityType, bool)> { ["vinylstate"] = (r.FormatInfo?.VinylStateId, EntityType.VinylState, true), ["digitalformat"] = (r.FormatInfo?.DigitalFormatId, EntityType.DigitalFormat, true), ["bitness"] = (r.FormatInfo?.BitnessId, EntityType.Bitness, true), ["sampling"] = (r.FormatInfo?.SamplingId, EntityType.Sampling, true), ["format"] = (r.FormatInfo?.SourceFormatId, EntityType.SourceFormat, true), ["player"] = (r.EquipmentInfo?.PlayerId, EntityType.Player, false), ["cartridge"] = (r.EquipmentInfo?.CartridgeId, EntityType.Cartridge, false), ["amp"] = (r.EquipmentInfo?.AmplifierId, EntityType.Amplifier, false), ["adc"] = (r.EquipmentInfo?.AdcId, EntityType.Adc, false), ["wire"] = (r.EquipmentInfo?.WireId, EntityType.Wire, false) }); }
}
