using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Enums;
using Web.Infrastructure.Persistence;
using Web.Infrastructure.Storage;
using Web.Models;

namespace Web.Features.Albums.Create;

public sealed class CreateAlbumCommandHandler(
    Context context,
    TimeProvider timeProvider,
    IImageService imageService) : IRequestHandler<CreateAlbumCommand, int>
{
    public async Task<int> Handle(CreateAlbumCommand request, CancellationToken cancellationToken)
    {
        var title = request.Request.Title.Trim();
        var artistName = request.Request.Artist.Trim();
        var genreName = request.Request.Genre.Trim();
        var album = await context.Albums
            .Include(a => a.Artist)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Artist != null && a.Title == title && a.Artist.Name == artistName, cancellationToken);

        if (album is null)
        {
            var artist = await context.Artists.FirstOrDefaultAsync(a => a.Name == artistName, cancellationToken);
            if (artist is null)
            {
                artist = new Artist { Name = artistName };
                context.Artists.Add(artist);
                await context.SaveChangesAsync(cancellationToken);
            }

            var genre = await context.Genres.FirstOrDefaultAsync(g => g.Name == genreName, cancellationToken);
            if (genre is null)
            {
                genre = new Genre { Name = genreName };
                context.Genres.Add(genre);
                await context.SaveChangesAsync(cancellationToken);
            }

            album = new Album
            {
                AddedDate = timeProvider.GetLocalNow().LocalDateTime,
                Title = title,
                Artist = artist,
                Genre = genre
            };
            context.Albums.Add(album);
            await context.SaveChangesAsync(cancellationToken);
        }

        if (request.Request.AlbumCover is not null)
            await imageService.SaveAsync(album.Id, request.Request.AlbumCover, EntityType.AlbumCover);

        return album.Id;
    }
}

