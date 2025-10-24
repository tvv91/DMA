using Web.Models;

namespace Web.Interfaces
{
    public interface IDigitizationRepository : IRepository<Digitization>
    {
        Task<IEnumerable<Digitization>> GetByAlbumIdAsync(int albumId);
    }
}
