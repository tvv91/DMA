using Web.Enums;

namespace Web.Services
{
    public interface IImageService
    {
        string GetImageUrl(int id, Entity entity);
        void SaveCover(int albumId, string filename);
        void RemoveCover(int albumId);
    }
}
