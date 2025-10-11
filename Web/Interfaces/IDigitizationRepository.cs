using Web.Common;
using Web.Models;

namespace Web.Interfaces
{
    public interface IDigitizationRepository : IRepository<Digitization>
    {
        Task<PagedResult<Digitization>> GetByAlbumIdAsync(int albumId, int page, int pageSize);
    }
}
