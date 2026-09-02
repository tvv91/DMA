namespace DMA.Domain.Posts;

public interface IPostRepository
{
    IQueryable<Post> GetQueryable();
    Task<Post?> GetByIdWithCategoriesAsync(int id, CancellationToken cancellationToken = default);
    Task<Post?> FindTrackedWithCategoriesAsync(int postId, CancellationToken cancellationToken = default);
    Task<Post?> FindAsync(int id, CancellationToken cancellationToken = default);
    void Add(Post post);
    void Remove(Post post);
    Task<Category> FindOrCreateCategoryAsync(string categoryTitle, CancellationToken cancellationToken = default);
}
