using DMA.Domain.Common;

namespace DMA.Domain.Albums;

public interface IReleaseRepository
{
    Task<IEnumerable<Release>> GetByAlbumIdAsync(int albumId, CancellationToken cancellationToken = default);
    Task<Release?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedResult<Album>> GetAlbumsReleasedByEquipmentPagedAsync(EntityType equipmentType, int equipmentId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> ExistsByAlbumIdAndSourceAsync(int albumId, string source, CancellationToken cancellationToken = default);
    void Add(Release release);
    Task<Release?> FindTrackedWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    void AddFormatInfo(FormatInfo formatInfo);
    void AddEquipmentInfo(EquipmentInfo equipmentInfo);
    Task<Release?> FindAsync(int id, CancellationToken cancellationToken = default);
    void Remove(Release release);
}
