using Web.Enums;

namespace Web.Interfaces
{
    public interface ICoverService
    {
        Task<string> GetCoverUrlAsync(int id, EntityType entity);
        Task SaveCoverAsync(int id, string filename, EntityType entity);
        Task RemoveCoverAsync(int id, EntityType entity);
    }
}
