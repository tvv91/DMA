using DMA.Application.Images;
using Web.Interfaces;

namespace Web.Services;

public class LocalStorageImageService(IImageStorage imageStorage) : IImageService
{
    public Task<string> GetUrlAsync(int id, EntityType entity) =>
        imageStorage.GetUrlAsync(id, entity);

    public Task SaveAsync(int id, string filename, EntityType entity) =>
        imageStorage.SaveAsync(id, filename, entity);

    public Task RemoveAsync(int id, EntityType entity) =>
        imageStorage.RemoveAsync(id, entity);
}
