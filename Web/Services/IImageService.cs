using Web.Enums;

namespace Web.Services
{
    public interface IImageService
    {
        Task<string> GetImageUrlAsync(int id, EntityType entity);
        Task SaveCoverAsync(int id, string filename, EntityType entity);
        Task RemoveCoverAsync(int id, EntityType entity);
    }
}
