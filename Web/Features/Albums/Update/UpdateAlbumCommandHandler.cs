using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Infrastructure.Persistence;
using Web.Enums;
using Web.Features.Albums;
using Web.Infrastructure.Storage;
using Web.SignalRHubs;

namespace Web.Features.Albums.Update;

public sealed class UpdateAlbumCommandHandler(
    Context context,
    TimeProvider timeProvider,
    IImageService imageService) : IRequestHandler<UpdateAlbumCommand, int>
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

        AlbumHub.InvalidateAlbumCache(album.Id);
        return album.Id;
    }
}

