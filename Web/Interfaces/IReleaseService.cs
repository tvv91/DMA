namespace Web.Interfaces
{
    public interface IReleaseService
    {
        Task<IEnumerable<Release>> GetByAlbumIdAsync(int albumId);
        Task<Release?> GetByIdAsync(int id);
        Task<PagedResult<Album>> GetAlbumsReleasedByEquipmentPagedAsync(EntityType equipmentType, int equipmentId, int page, int pageSize);
        Task<bool> ExistsByAlbumIdAndSourceAsync(int albumId, string source);
        Task<Release> AddAsync(Release release);
        Task<Release> UpdateAsync(Release release);
        Task<bool> DeleteAsync(int id);
    }
}

