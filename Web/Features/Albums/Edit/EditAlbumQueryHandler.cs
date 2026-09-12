using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Infrastructure.Persistence;
using Web.Enums;
using Web.Infrastructure.Storage;
using Web.ViewModels;

namespace Web.Features.Albums.Edit;

public sealed class EditAlbumQueryHandler(Context context, IImageService imageService) : IRequestHandler<EditAlbumQuery, AlbumCreateUpdateViewModel?>
{
    public async Task<AlbumCreateUpdateViewModel?> Handle(EditAlbumQuery request, CancellationToken cancellationToken)
    {
        var album = await context.Albums
            .Include(a => a.Artist)
            .Include(a => a.Genre)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (album is null) return null;
        var coverUrl = await imageService.GetUrlAsync(album.Id, EntityType.AlbumCover);
        var releases = await context.Releases
            .Where(r => r.AlbumId == album.Id)
            .Include(r => r.Year).Include(r => r.Reissue).Include(r => r.Country)
            .Include(r => r.Label).Include(r => r.Storage)
            .Include(r => r.FormatInfo).ThenInclude(f => f!.Bitness)
            .Include(r => r.FormatInfo).ThenInclude(f => f!.Sampling)
            .Include(r => r.FormatInfo).ThenInclude(f => f!.DigitalFormat)
            .Include(r => r.FormatInfo).ThenInclude(f => f!.SourceFormat)
            .Include(r => r.FormatInfo).ThenInclude(f => f!.VinylState)
            .Include(r => r.EquipmentInfo).ThenInclude(e => e!.Player).ThenInclude(e => e!.Manufacturer)
            .Include(r => r.EquipmentInfo).ThenInclude(e => e!.Cartridge).ThenInclude(e => e!.Manufacturer)
            .Include(r => r.EquipmentInfo).ThenInclude(e => e!.Amplifier).ThenInclude(e => e!.Manufacturer)
            .Include(r => r.EquipmentInfo).ThenInclude(e => e!.Adc).ThenInclude(e => e!.Manufacturer)
            .Include(r => r.EquipmentInfo).ThenInclude(e => e!.Wire).ThenInclude(e => e!.Manufacturer)
            .AsNoTracking().ToListAsync(cancellationToken);
        return new AlbumCreateUpdateViewModel
        {
            AlbumId = album.Id,
            Title = album.Title,
            Artist = album.Artist?.Name ?? string.Empty,
            Genre = album.Genre?.Name ?? string.Empty,
            AlbumCover = coverUrl.Contains("nocover") ? null : album.Id.ToString(),
            Action = ActionType.Update,
            Releases = releases
        };
    }
}

