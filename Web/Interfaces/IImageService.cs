using Web.Enums;

namespace Web.Interfaces
{
    public interface IImageService
    {
        Task<string> GetUrlAsync(int id, EntityType entity);
        Task SaveAsync(int id, string filename, EntityType entity);
        Task RemoveAsync(int id, EntityType entity);
    }
}
