using AppResourceIconService = DMA.Application.Images.IResourceIconService;
using Web.Interfaces;

namespace Web.Services;

public class LocalResourceIconService(AppResourceIconService resourceIconService) : IResourceIconService
{
    public Task<string> GetIconUrlAsync(int id, EntityType entity) =>
        resourceIconService.GetIconUrlAsync(id, entity);
}
