using DMA.Domain.Common;
using DMA.Domain.Posts;
using Microsoft.EntityFrameworkCore;

namespace DMA.Infrastructure.Persistence.Repositories;

public class PostRepository(DmaDbContext context) : IPostRepository
{
    private readonly DmaDbContext _context = context;

    public IQueryable<Post> GetQueryable() => _context.Posts.AsQueryable();

    public Task<Post?> GetByIdWithCategoriesAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Posts
            .Include(p => p.PostCategories).ThenInclude(pc => pc.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Post?> FindTrackedWithCategoriesAsync(int postId, CancellationToken cancellationToken = default) =>
        _context.Posts
            .Include(p => p.PostCategories).ThenInclude(pc => pc.Category)
            .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);

    public Task<Post?> FindAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Posts.FindAsync([id], cancellationToken).AsTask();

    public void Add(Post post) => _context.Posts.Add(post);

    public void Remove(Post post) => _context.Posts.Remove(post);

    public async Task<Category> FindOrCreateCategoryAsync(string categoryTitle, CancellationToken cancellationToken = default)
    {
        var normalizedCategoryTitle = categoryTitle.Trim();
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Title == normalizedCategoryTitle, cancellationToken);

        if (category is null)
        {
            category = new Category { Title = categoryTitle };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return category;
    }

    public async Task<PagedResult<Post>> GetListAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Posts
            .Include(p => p.PostCategories).ThenInclude(pc => pc.Category)
            .AsNoTracking()
            .AsQueryable();

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedDate ?? DateTime.MinValue)
            .ThenByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Post>(items, totalItems, page, pageSize);
    }

    public async Task<PagedResult<Post>> GetFilteredListAsync(int page, int pageSize, string? searchText, string? category, string? year, bool onlyDrafts, bool excludeDrafts = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Posts
            .Include(p => p.PostCategories).ThenInclude(pc => pc.Category)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(p =>
                p.Title.Contains(searchText) ||
                p.Description.Contains(searchText) ||
                p.Content.Contains(searchText));
        }

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.PostCategories.Any(pc => pc.Category.Title == category));

        if (!string.IsNullOrWhiteSpace(year) && int.TryParse(year, out var yearValue))
            query = query.Where(p => p.CreatedDate.HasValue && p.CreatedDate.Value.Year == yearValue);

        if (onlyDrafts)
            query = query.Where(p => p.IsDraft);
        else if (excludeDrafts)
            query = query.Where(p => !p.IsDraft);

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedDate ?? DateTime.MinValue)
            .ThenByDescending(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Post>(items, totalItems, page, pageSize);
    }
}
