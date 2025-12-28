using Web.Models;

namespace Web.Interfaces
{
    public interface IDigitizationService
    {
        Task<IEnumerable<Digitization>> GetByAlbumIdAsync(int albumId);
        Task<Digitization> AddAsync(Digitization digitization);
        Task<Digitization> UpdateAsync(Digitization digitization);
        Task<bool> DeleteAsync(int id);
    }
}

