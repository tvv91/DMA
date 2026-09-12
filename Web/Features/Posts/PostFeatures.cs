using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Infrastructure.Persistence;
using Web.Models;
using Web.ViewModels;

namespace Web.Features.Posts;

public sealed record PostPageQuery : IRequest<Unit>;
public sealed record NewPostQuery : IRequest<PostViewModel>;
public sealed record GetPostQuery(int Id) : IRequest<PostViewModel>;
public sealed record CreatePostCommand(PostViewModel Model) : IRequest<int>;
public sealed record UpdatePostCommand(PostViewModel Model) : IRequest<int>;
public sealed record DeletePostCommand(int Id) : IRequest<bool>;
public sealed record GetHubPostsQuery(int Page, string? SearchText, string? Category, string? Year, bool OnlyDrafts, bool ExcludeDrafts) : IRequest<PostHubPage>;
public sealed record GetBlogTreeQuery : IRequest<IReadOnlyList<PostBlogCategory>>;
public sealed record CreateDraftPostCommand(PostViewModel Model) : IRequest<int>;

public sealed record PostHubPage(IReadOnlyList<PostHubItem> Items, int TotalPages);
public sealed record PostHubItem(int Id, string Title, string Description, bool IsDraft, string? Created, IReadOnlyList<string> Categories);
public sealed record PostBlogCategory(string Category, IReadOnlyList<PostBlogYear> Posts);
public sealed record PostBlogYear(int Year, IReadOnlyList<PostBlogItem> Posts);
public sealed record PostBlogItem(int Id, string Title, string Created);

public sealed class PostPageQueryHandler : IRequestHandler<PostPageQuery, Unit>
{
    public Task<Unit> Handle(PostPageQuery request, CancellationToken cancellationToken) => Task.FromResult(Unit.Value);
}

public sealed class GetHubPostsQueryHandler(Context context) : IRequestHandler<GetHubPostsQuery, PostHubPage>
{
    public async Task<PostHubPage> Handle(GetHubPostsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Posts.Include(p => p.PostCategories).ThenInclude(pc => pc.Category).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.SearchText)) query = query.Where(p => p.Title.Contains(request.SearchText) || p.Description.Contains(request.SearchText) || p.Content.Contains(request.SearchText));
        if (!string.IsNullOrWhiteSpace(request.Category)) query = query.Where(p => p.PostCategories.Any(pc => pc.Category.Title == request.Category));
        if (!string.IsNullOrWhiteSpace(request.Year) && int.TryParse(request.Year, out var year)) query = query.Where(p => p.CreatedDate.HasValue && p.CreatedDate.Value.Year == year);
        if (request.OnlyDrafts) query = query.Where(p => p.IsDraft);
        else if (request.ExcludeDrafts) query = query.Where(p => !p.IsDraft);
        var total = await query.CountAsync(cancellationToken);
        var posts = await query.OrderByDescending(p => p.CreatedDate ?? DateTime.MinValue).ThenByDescending(p => p.Id).Skip((request.Page - 1) * 5).Take(5).ToListAsync(cancellationToken);
        return new PostHubPage(posts.Select(p => new PostHubItem(p.Id, p.Title, p.Description, p.IsDraft, p.CreatedDate?.ToShortDateString(), p.PostCategories.Select(pc => pc.Category.Title).ToList())).ToList(), (int)Math.Ceiling(total / 5d));
    }
}

public sealed class GetBlogTreeQueryHandler(Context context) : IRequestHandler<GetBlogTreeQuery, IReadOnlyList<PostBlogCategory>>
{
    public async Task<IReadOnlyList<PostBlogCategory>> Handle(GetBlogTreeQuery request, CancellationToken cancellationToken)
    {
        var posts = await context.Posts.Include(p => p.PostCategories).ThenInclude(pc => pc.Category).AsNoTracking().Where(p => !p.IsDraft && p.CreatedDate.HasValue).ToListAsync(cancellationToken);
        return posts.SelectMany(p => p.PostCategories.Any() ? p.PostCategories.Select(pc => new { Post = p, Category = pc.Category.Title }) : [new { Post = p, Category = "Uncategorized" }])
            .GroupBy(x => x.Category).Select(group => new PostBlogCategory(group.Key, group.Select(x => x.Post).Distinct().GroupBy(p => p.CreatedDate!.Value.Year).OrderByDescending(x => x.Key).Select(year => new PostBlogYear(year.Key, year.Select(p => new PostBlogItem(p.Id, p.Title, p.CreatedDate!.Value.ToShortDateString())).ToList())).ToList())).OrderBy(x => x.Category).ToList();
    }
}

public sealed class CreateDraftPostCommandHandler(Context context, TimeProvider timeProvider) : IRequestHandler<CreateDraftPostCommand, int>
{
    public async Task<int> Handle(CreateDraftPostCommand request, CancellationToken cancellationToken)
    {
        var post = new Post { Title = request.Model.Title, Description = request.Model.Description, Content = request.Model.Content, CreatedDate = timeProvider.GetUtcNow().UtcDateTime, IsDraft = true };
        if (!string.IsNullOrWhiteSpace(request.Model.Category) && request.Model.Category != "Category")
        {
            var name = request.Model.Category.Trim();
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Title == name, cancellationToken) ?? new Category { Title = name };
            if (category.Id == 0) context.Categories.Add(category);
            post.PostCategories.Add(new PostCategory { Category = category });
        }
        context.Posts.Add(post);
        await context.SaveChangesAsync(cancellationToken);
        return post.Id;
    }
}

public sealed class NewPostQueryHandler : IRequestHandler<NewPostQuery, PostViewModel>
{
    public Task<PostViewModel> Handle(NewPostQuery request, CancellationToken cancellationToken) => Task.FromResult(new PostViewModel());
}

public sealed class GetPostQueryHandler(Context context) : IRequestHandler<GetPostQuery, PostViewModel>
{
    public async Task<PostViewModel> Handle(GetPostQuery request, CancellationToken cancellationToken)
    {
        var post = await context.Posts
            .Include(p => p.PostCategories)
            .ThenInclude(pc => pc.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (post is null) throw new KeyNotFoundException($"Post with id {request.Id} not found");
        return new PostViewModel
        {
            Id = post.Id,
            Title = post.Title,
            Description = post.Description,
            Content = post.Content,
            CreatedDate = post.CreatedDate,
            UpdatedTime = post.UpdatedDate,
            Category = post.PostCategories.FirstOrDefault()?.Category?.Title,
            IsDraft = post.IsDraft
        };
    }
}

public sealed class CreatePostCommandHandler(Context context, TimeProvider timeProvider) : IRequestHandler<CreatePostCommand, int>
{
    public async Task<int> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = new Post
        {
            Title = request.Model.Title,
            Description = request.Model.Description,
            Content = request.Model.Content,
            CreatedDate = timeProvider.GetUtcNow().UtcDateTime,
            IsDraft = false
        };
        if (!string.IsNullOrWhiteSpace(request.Model.Category) && request.Model.Category != "Category")
        {
            var categoryName = request.Model.Category.Trim();
            var category = await context.Categories.FirstOrDefaultAsync(c => c.Title == categoryName, cancellationToken);
            if (category is null)
            {
                category = new Category { Title = categoryName };
                context.Categories.Add(category);
                await context.SaveChangesAsync(cancellationToken);
            }
            post.PostCategories.Add(new PostCategory { Category = category });
        }
        context.Posts.Add(post);
        await context.SaveChangesAsync(cancellationToken);
        return post.Id;
    }
}

public sealed class UpdatePostCommandHandler(Context context, TimeProvider timeProvider) : IRequestHandler<UpdatePostCommand, int>
{
    public async Task<int> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        if (request.Model.Id is null)
            throw new ArgumentException("Post id is required", nameof(request.Model));
        var post = await context.Posts
            .Include(p => p.PostCategories)
            .ThenInclude(pc => pc.Category)
            .FirstOrDefaultAsync(p => p.Id == request.Model.Id.Value, cancellationToken);
        if (post is null) throw new KeyNotFoundException($"Post with Id {request.Model.Id} not found.");
        post.Title = request.Model.Title;
        post.Description = request.Model.Description;
        post.Content = request.Model.Content;
        post.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;
        var current = post.PostCategories.FirstOrDefault()?.Category?.Title;
        var category = request.Model.Category?.Trim();
        if (category != current && !string.IsNullOrWhiteSpace(category) && category != "Category")
        {
            post.PostCategories.Clear();
            var categoryEntity = await context.Categories.FirstOrDefaultAsync(c => c.Title == category, cancellationToken);
            if (categoryEntity is null)
            {
                categoryEntity = new Category { Title = category };
                context.Categories.Add(categoryEntity);
                await context.SaveChangesAsync(cancellationToken);
            }
            post.PostCategories.Add(new PostCategory { Category = categoryEntity });
        }
        await context.SaveChangesAsync(cancellationToken);
        return post.Id;
    }
}

public sealed class DeletePostCommandHandler(Context context) : IRequestHandler<DeletePostCommand, bool>
{
    public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await context.Posts.FindAsync([request.Id], cancellationToken);
        if (post is null) return false;
        context.Posts.Remove(post);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

