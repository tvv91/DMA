using Web.Common;
using Web.Models;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IAlbumService
    {
        Task<PagedResult<Album>> GetIndexListAsync(int page, int pageSize, string? artistName = null, string? genreName = null, string? yearValue = null, string? albumTitle = null);
        Task<bool> HasAnyAlbumsAsync();
        Task<Album?> GetByIdAsync(int id);
        Task<Album?> FindByAlbumAndArtistAsync(string title, string artist);
        Task<AlbumDetailsViewModel> GetAlbumDetailsAsync(int id);
        Task<Album> CreateOrFindAlbumAsync(string title, string artist, string genre);
        Task<Album> UpdateAlbumAsync(int albumId, string title, string? artist, string? genre);
        Task<bool> DeleteAlbumAsync(int id);
        Digitization MapViewModelToDigitization(int albumId, AlbumCreateUpdateViewModel request);
        AlbumDetailsViewModel MapAlbumToAlbumDetailsVM(Album album, IEnumerable<Digitization>? digitizations = null);
        Task<AlbumCreateUpdateViewModel> MapAlbumToCreateUpdateVMAsync(Album album);
    }
}
