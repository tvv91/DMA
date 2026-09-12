using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Models;
using Web.ViewModels;

namespace Web.Features.Albums.GetById;

public sealed class GetByIdQueryHandler(Context context) : IRequestHandler<GetByIdQuery, AlbumDetailsViewModel>
{
    public async Task<AlbumDetailsViewModel> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var album = await context.Albums
            .Include(a => a.Artist)
            .Include(a => a.Genre)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        if (album is null) throw new KeyNotFoundException($"Album with id {request.Id} not found");

        var releases = await context.Releases
            .Where(r => r.AlbumId == album.Id)
            .Include(r => r.Year)
            .Include(r => r.Reissue)
            .Include(r => r.Country)
            .Include(r => r.Label)
            .Include(r => r.Storage)
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
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new AlbumDetailsViewModel
        {
            AlbumId = album.Id,
            Title = album.Title,
            Artist = album.Artist?.Name ?? string.Empty,
            Genre = album.Genre?.Name ?? string.Empty,
            AddedDate = album.AddedDate,
            UpdateDate = album.UpdateDate,
            Releases = releases
        };
    }
}
