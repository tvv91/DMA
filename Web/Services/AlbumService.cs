using DMA.Application.Albums;
using DMA.Application.Releases;
using MediatR;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Services;

public class AlbumService(
    IMediator mediator,
    IImageService imageService,
    TimeProvider timeProvider) : IAlbumService
{
    public Task<bool> HasAnyAlbumsAsync() => mediator.Send(new HasAnyAlbumsQuery());

    public Task<PagedResult<Album>> GetIndexListAsync(int page, int pageSize, string? artistName = null, string? genreName = null, string? yearValue = null, string? albumTitle = null) =>
        mediator.Send(new GetAlbumIndexListQuery(page, pageSize, artistName, genreName, yearValue, albumTitle));

    public Task<Album?> GetByIdAsync(int id) => mediator.Send(new GetAlbumByIdQuery(id));

    public Task<Album?> FindByAlbumAndArtistAsync(string title, string artist) =>
        mediator.Send(new FindAlbumByTitleAndArtistQuery(title, artist));

    public async Task<AlbumDetailsViewModel> GetAlbumDetailsAsync(int id)
    {
        var (album, releases) = await mediator.Send(new GetAlbumDetailsQuery(id));
        return MapAlbumToAlbumDetailsVM(album, releases);
    }

    public Task<Album> CreateOrFindAlbumAsync(string title, string artist, string genre) =>
        mediator.Send(new CreateOrFindAlbumCommand(title, artist, genre));

    public Task<Album> UpdateAlbumAsync(int albumId, string title, string? artist, string? genre) =>
        mediator.Send(new UpdateAlbumCommand(albumId, title, artist, genre));

    public Task<bool> DeleteAlbumAsync(int id) => mediator.Send(new DeleteAlbumCommand(id));

    public Release MapViewModelToRelease(int albumId, AlbumCreateUpdateViewModel request) => new()
    {
        AlbumId = albumId,
        AddedDate = timeProvider.GetLocalNow().LocalDateTime,
        Source = request.Source,
        Discogs = request.Discogs,
        IsFirstPress = false,
        YearId = request.Year,
        ReissueId = request.Reissue,
        Country = !string.IsNullOrEmpty(request.Country) ? new Country { Name = request.Country } : null,
        Label = !string.IsNullOrEmpty(request.Label) ? new Label { Name = request.Label } : null,
        Storage = !string.IsNullOrEmpty(request.Storage) ? new Storage { Name = request.Storage } : null,
        FormatInfo = new FormatInfo
        {
            BitnessId = request.Bitness,
            Sampling = request.Sampling.HasValue ? new Sampling { Value = request.Sampling.Value } : null,
            DigitalFormat = !string.IsNullOrEmpty(request.DigitalFormat) ? new DigitalFormat { Name = request.DigitalFormat } : null,
            SourceFormat = !string.IsNullOrEmpty(request.SourceFormat) ? new SourceFormat { Name = request.SourceFormat } : null,
            VinylState = !string.IsNullOrEmpty(request.VinylState) ? new VinylState { Name = request.VinylState } : null
        },
        EquipmentInfo = new EquipmentInfo
        {
            Player = !string.IsNullOrEmpty(request.Player) ? new Player { Name = request.Player } : null,
            Cartridge = !string.IsNullOrEmpty(request.Cartridge) ? new Cartridge { Name = request.Cartridge } : null,
            Amplifier = !string.IsNullOrEmpty(request.Amplifier) ? new Amplifier { Name = request.Amplifier } : null,
            Adc = !string.IsNullOrEmpty(request.Adc) ? new Adc { Name = request.Adc } : null,
            Wire = !string.IsNullOrEmpty(request.Wire) ? new Wire { Name = request.Wire } : null
        }
    };

    public AlbumDetailsViewModel MapAlbumToAlbumDetailsVM(Album album, IEnumerable<Release>? releases = null) => new()
    {
        AlbumId = album.Id,
        Title = album.Title,
        Artist = album.Artist?.Name ?? string.Empty,
        Genre = album.Genre?.Name ?? string.Empty,
        AddedDate = album.AddedDate,
        UpdateDate = album.UpdateDate,
        Releases = releases
    };

    public async Task<AlbumCreateUpdateViewModel> MapAlbumToCreateUpdateVMAsync(Album album)
    {
        var releases = await mediator.Send(new GetReleasesByAlbumIdQuery(album.Id));
        var coverUrl = await imageService.GetUrlAsync(album.Id, EntityType.AlbumCover);
        var albumCover = coverUrl.Contains("nocover") ? null : album.Id.ToString();

        return new AlbumCreateUpdateViewModel
        {
            AlbumId = album.Id,
            Title = album.Title,
            Artist = album.Artist?.Name ?? string.Empty,
            Genre = album.Genre?.Name ?? string.Empty,
            AlbumCover = albumCover,
            Action = ActionType.Update,
            Releases = releases.ToList()
        };
    }
}
