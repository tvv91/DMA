using Web.Enums;

namespace Web.Infrastructure.Icons
{
    public interface IResourceIconService
    {
        Task<string> GetIconUrlAsync(int id, EntityType entity);
    }
}
