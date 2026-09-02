using DMA.Application.Releases;
using MediatR;
using Web.Interfaces;

namespace Web.Services;

public class ReleaseService(IMediator mediator) : IReleaseService
{
    public Task<IEnumerable<Release>> GetByAlbumIdAsync(int albumId) =>
        mediator.Send(new GetReleasesByAlbumIdQuery(albumId));

    public Task<Release?> GetByIdAsync(int id) =>
        mediator.Send(new GetReleaseByIdQuery(id));

    public Task<PagedResult<Album>> GetAlbumsReleasedByEquipmentPagedAsync(EntityType equipmentType, int equipmentId, int page, int pageSize) =>
        mediator.Send(new GetAlbumsReleasedByEquipmentPagedQuery(equipmentType, equipmentId, page, pageSize));

    public Task<bool> ExistsByAlbumIdAndSourceAsync(int albumId, string source) =>
        mediator.Send(new ExistsReleaseByAlbumIdAndSourceQuery(albumId, source));

    public Task<Release> AddAsync(Release release) =>
        mediator.Send(new AddReleaseCommand(release));

    public Task<Release> UpdateAsync(Release release) =>
        mediator.Send(new UpdateReleaseCommand(release));

    public Task<bool> DeleteAsync(int id) =>
        mediator.Send(new DeleteReleaseCommand(id));
}
