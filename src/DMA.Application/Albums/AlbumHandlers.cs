using DMA.Domain.Albums;
using DMA.Domain.Common;
using DMA.Domain.ReferenceData;
using MediatR;

namespace DMA.Application.Albums;

public sealed record HasAnyAlbumsQuery : IRequest<bool>;

public sealed class HasAnyAlbumsQueryHandler(IAlbumRepository albumRepository)
    : IRequestHandler<HasAnyAlbumsQuery, bool>
{
    public Task<bool> Handle(HasAnyAlbumsQuery request, CancellationToken cancellationToken) =>
        albumRepository.AnyAsync(cancellationToken);
}

public sealed record GetAlbumIndexListQuery(
    int Page,
    int PageSize,
    string? ArtistName = null,
    string? GenreName = null,
    string? YearValue = null,
    string? AlbumTitle = null) : IRequest<PagedResult<Album>>;

public sealed class GetAlbumIndexListQueryHandler(IAlbumRepository albumRepository)
    : IRequestHandler<GetAlbumIndexListQuery, PagedResult<Album>>
{
    public Task<PagedResult<Album>> Handle(GetAlbumIndexListQuery request, CancellationToken cancellationToken) =>
        albumRepository.GetIndexListAsync(
            request.Page,
            request.PageSize,
            request.ArtistName,
            request.GenreName,
            request.YearValue,
            request.AlbumTitle,
            cancellationToken);
}

public sealed record GetAlbumByIdQuery(int Id) : IRequest<Album?>;

public sealed class GetAlbumByIdQueryHandler(IAlbumRepository albumRepository)
    : IRequestHandler<GetAlbumByIdQuery, Album?>
{
    public Task<Album?> Handle(GetAlbumByIdQuery request, CancellationToken cancellationToken) =>
        albumRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
}

public sealed record FindAlbumByTitleAndArtistQuery(string Title, string Artist) : IRequest<Album?>;

public sealed class FindAlbumByTitleAndArtistQueryHandler(IAlbumRepository albumRepository)
    : IRequestHandler<FindAlbumByTitleAndArtistQuery, Album?>
{
    public Task<Album?> Handle(FindAlbumByTitleAndArtistQuery request, CancellationToken cancellationToken) =>
        albumRepository.FindByTitleAndArtistAsync(request.Title, request.Artist, cancellationToken);
}

public sealed record GetAlbumDetailsQuery(int Id) : IRequest<(Album Album, IEnumerable<Release> Releases)>;

public sealed class GetAlbumDetailsQueryHandler(IAlbumRepository albumRepository, IReleaseRepository releaseRepository)
    : IRequestHandler<GetAlbumDetailsQuery, (Album Album, IEnumerable<Release> Releases)>
{
    public async Task<(Album Album, IEnumerable<Release> Releases)> Handle(GetAlbumDetailsQuery request, CancellationToken cancellationToken)
    {
        var album = await albumRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Album with id {request.Id} not found");

        var releases = await releaseRepository.GetByAlbumIdAsync(album.Id, cancellationToken);
        return (album, releases);
    }
}

public sealed record CreateOrFindAlbumCommand(string Title, string Artist, string Genre) : IRequest<Album>;

public sealed class CreateOrFindAlbumCommandHandler(
    IAlbumRepository albumRepository,
    IReferenceDataRepository referenceDataRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<CreateOrFindAlbumCommand, Album>
{
    public async Task<Album> Handle(CreateOrFindAlbumCommand request, CancellationToken cancellationToken)
    {
        var normalizedTitle = request.Title.Trim();
        var normalizedArtist = request.Artist.Trim();
        var normalizedGenre = request.Genre.Trim();

        var album = await albumRepository.FindByTitleAndArtistAsync(normalizedTitle, normalizedArtist, cancellationToken);
        if (album is not null)
            return album;

        album = new Album
        {
            AddedDate = timeProvider.GetLocalNow().LocalDateTime,
            Title = normalizedTitle,
            Artist = await referenceDataRepository.FindOrCreateArtistAsync(normalizedArtist, cancellationToken),
            Genre = await referenceDataRepository.FindOrCreateGenreAsync(normalizedGenre, cancellationToken)
        };

        albumRepository.Add(album);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return album;
    }
}

public sealed record UpdateAlbumCommand(int AlbumId, string Title, string? Artist, string? Genre) : IRequest<Album>;

public sealed class UpdateAlbumCommandHandler(
    IAlbumRepository albumRepository,
    IReferenceDataRepository referenceDataRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<UpdateAlbumCommand, Album>
{
    public async Task<Album> Handle(UpdateAlbumCommand request, CancellationToken cancellationToken)
    {
        if (request.AlbumId <= 0)
            throw new InvalidDataException("AlbumId is invalid");

        var existing = await albumRepository.GetByIdWithDetailsAsync(request.AlbumId, cancellationToken)
            ?? throw new KeyNotFoundException($"Album {request.AlbumId} not found");

        if (existing.Title == request.Title &&
            (string.IsNullOrWhiteSpace(request.Genre) || existing.Genre?.Name == request.Genre) &&
            (string.IsNullOrWhiteSpace(request.Artist) || existing.Artist?.Name == request.Artist))
        {
            return existing;
        }

        existing.Title = request.Title;
        existing.UpdateDate = timeProvider.GetUtcNow().UtcDateTime;

        if (!string.IsNullOrWhiteSpace(request.Genre))
        {
            var genreEntity = await referenceDataRepository.FindOrCreateGenreAsync(request.Genre, cancellationToken);
            existing.GenreId = genreEntity.Id;
        }

        if (!string.IsNullOrWhiteSpace(request.Artist))
        {
            var artistEntity = await referenceDataRepository.FindOrCreateArtistAsync(request.Artist, cancellationToken);
            existing.ArtistId = artistEntity.Id;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return existing;
    }
}

public sealed record DeleteAlbumCommand(int Id) : IRequest<bool>;

public sealed class DeleteAlbumCommandHandler(IAlbumRepository albumRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteAlbumCommand, bool>
{
    public async Task<bool> Handle(DeleteAlbumCommand request, CancellationToken cancellationToken)
    {
        var album = await albumRepository.FindAsync(request.Id, cancellationToken);
        if (album is null)
            return false;

        albumRepository.Remove(album);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
