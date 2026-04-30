using Web.Enums;

namespace Web.Interfaces
{
    public interface IResourceIconService
    {
        Task<string> GetIconUrlAsync(int id, EntityType entity);
    }
}
