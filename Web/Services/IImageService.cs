using Web.Enum;

namespace Web.Services
{
    public interface IImageService
    {
        string GetImageUrl(int id, EntityType entity);
    }
}
