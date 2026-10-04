using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Infrastructure.Persistence;
using Web.Enums;
using Web.Features.Albums;
using Web.Features.Albums.Releases;
using Web.Infrastructure.Storage;
using Web.Hubs;

namespace Web.Features.Albums.Update;

public sealed class UpdateAlbumCommandHandler(
    Context context,
    TimeProvider timeProvider,
    IImageService imageService,
    ISender sender) : IRequestHandler<UpdateAlbumCommand, int>
{
    public async Task<int> Handle(UpdateAlbumCommand request, CancellationToken cancellationToken)
    {
        var model = request.Request;
        if (model.AlbumId <= 0)
            throw new InvalidDataException("AlbumId is invalid");
        var album = await context.Albums
            .Include(a => a.Artist)
            .Include(a => a.Genre)
            .FirstOrDefaultAsync(a => a.Id == model.AlbumId, cancellationToken);
        if (album is null)
            throw new KeyNotFoundException($"Album {model.AlbumId} not found");

        album.Title = model.Title;
        album.UpdateDate = timeProvider.GetUtcNow().UtcDateTime;
        if (!string.IsNullOrWhiteSpace(model.Genre))
        {
            var genreName = model.Genre.Trim();
            var genre = await context.Genres.FirstOrDefaultAsync(g => g.Name == genreName, cancellationToken);
            if (genre is null)
            {
                genre = new Web.Models.Genre { Name = genreName };
                context.Genres.Add(genre);
                await context.SaveChangesAsync(cancellationToken);
            }
            album.GenreId = genre.Id;
        }
        if (!string.IsNullOrWhiteSpace(model.Artist))
        {
            var artistName = model.Artist.Trim();
            var artist = await context.Artists.FirstOrDefaultAsync(a => a.Name == artistName, cancellationToken);
            if (artist is null)
            {
                artist = new Web.Models.Artist { Name = artistName };
                context.Artists.Add(artist);
                await context.SaveChangesAsync(cancellationToken);
            }
            album.ArtistId = artist.Id;
        }
        await context.SaveChangesAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(model.AlbumCover))
            await imageService.RemoveAsync(album.Id, EntityType.AlbumCover);
        else if (model.AlbumCover != album.Id.ToString())
            await imageService.SaveAsync(album.Id, model.AlbumCover, EntityType.AlbumCover);

        if (model.ReleaseId > 0)
        {
            var releaseBelongsToAlbum = await context.Releases
                .AnyAsync(r => r.Id == model.ReleaseId && r.AlbumId == album.Id, cancellationToken);
            if (!releaseBelongsToAlbum)
                throw new InvalidDataException("Release does not belong to the album");

            await sender.Send(new UpdateReleaseCommand(new UpdateReleaseRequest
            {
                ReleaseId = model.ReleaseId,
                Source = model.Source,
                Discogs = model.Discogs,
                IsFirstPress = model.IsFirstPress,
                Country = model.Country,
                Label = model.Label,
                Storage = model.Storage,
                Year = model.Year,
                Reissue = model.Reissue,
                Size = model.Size,
                VinylState = model.VinylState,
                DigitalFormat = model.DigitalFormat,
                Bitness = model.Bitness,
                Sampling = model.Sampling,
                SourceFormat = model.SourceFormat,
                Player = model.Player,
                PlayerManufacturer = model.PlayerManufacturer,
                Cartridge = model.Cartridge,
                CartridgeManufacturer = model.CartridgeManufacturer,
                Amplifier = model.Amplifier,
                AmplifierManufacturer = model.AmplifierManufacturer,
                Adc = model.Adc,
                AdcManufacturer = model.AdcManufacturer,
                Wire = model.Wire,
                WireManufacturer = model.WireManufacturer
            }), cancellationToken);
        }

        AlbumHub.InvalidateAlbumCache(album.Id);
        return album.Id;
    }
}

