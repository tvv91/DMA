using Web.Models;

namespace Web.Interfaces
{
    public interface IDigitizationRepository : IRepository<Digitization>
    {
        Task<IEnumerable<Digitization>> GetByAlbumIdAsync(int albumId);
        Task<bool> ExistsByAlbumIdAsync(int albumId);
        Task<bool> ExistsByAlbumIdAndSourceAsync(int albumId, string source);
    }
}
