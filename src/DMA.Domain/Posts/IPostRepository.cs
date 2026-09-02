using DMA.Domain.Common;

namespace DMA.Domain.Posts;

public interface IPostRepository
{
    Task<PagedResult<Post>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PagedResult<Post>> GetFilteredListAsync(int page, int pageSize, string? searchText, string? category, string? year, bool onlyDrafts, bool excludeDrafts = false, CancellationToken cancellationToken = default);
    IQueryable<Post> GetQueryable();
    Task<Post?> GetByIdWithCategoriesAsync(int id, CancellationToken cancellationToken = default);
    Task<Post?> FindTrackedWithCategoriesAsync(int postId, CancellationToken cancellationToken = default);
    Task<Post?> FindAsync(int id, CancellationToken cancellationToken = default);
    void Add(Post post);
    void Remove(Post post);
    Task<Category> FindOrCreateCategoryAsync(string categoryTitle, CancellationToken cancellationToken = default);
}
