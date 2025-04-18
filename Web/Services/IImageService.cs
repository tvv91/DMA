using Web.Enums;

namespace Web.Services
{
    public interface IImageService
    {
        string GetImageUrl(int id, EntityType entity);
        string GetIconUrl(int Id, EntityType entity);
        void SaveCover(int id, string filename, EntityType entity);
        void RemoveCover(int id, EntityType entity);
    }
}
