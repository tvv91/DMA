using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Models;
using Web.ViewModels;

namespace Web.Features.Posts;

public sealed record PostPageQuery : IRequest<Unit>;
public sealed record NewPostQuery : IRequest<PostViewModel>;
public sealed record GetPostQuery(int Id) : IRequest<PostViewModel>;
public sealed record CreatePostCommand(PostViewModel Model) : IRequest<int>;
public sealed record UpdatePostCommand(PostViewModel Model) : IRequest<int>;
public sealed record DeletePostCommand(int Id) : IRequest<bool>;

public sealed class PostPageQueryHandler : IRequestHandler<PostPageQuery, Unit>
{
    public Task<Unit> Handle(PostPageQuery request, CancellationToken cancellationToken) => Task.FromResult(Unit.Value);
}

public sealed class NewPostQueryHandler : IRequestHandler<NewPostQuery, PostViewModel>
{
    public Task<PostViewModel> Handle(NewPostQuery request, CancellationToken cancellationToken) => Task.FromResult(new PostViewModel());
}

public sealed class GetPostQueryHandler(Context context) : IRequestHandler<GetPostQuery, PostViewModel>
{
    public async Task<PostViewModel> Handle(GetPostQuery request, CancellationToken cancellationToken)
    {
        var post = await PostFeatureHelpers.GetByIdAsync(context, request.Id);
        if (post is null) throw new KeyNotFoundException($"Post with id {request.Id} not found");
        return PostFeatureHelpers.ToViewModel(post);
    }
}

public sealed class CreatePostCommandHandler(Context context, TimeProvider timeProvider) : IRequestHandler<CreatePostCommand, int>
{
    public async Task<int> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await PostFeatureHelpers.CreateAsync(context, timeProvider, request.Model, false);
        return post.Id;
    }
}

public sealed class UpdatePostCommandHandler(Context context, TimeProvider timeProvider) : IRequestHandler<UpdatePostCommand, int>
{
    public async Task<int> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        if (request.Model.Id is null)
            throw new ArgumentException("Post id is required", nameof(request.Model));
        var post = await PostFeatureHelpers.GetByIdAsync(context, request.Model.Id.Value, tracked: true);
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
            post.PostCategories.Add(new PostCategory { Category = await PostFeatureHelpers.FindOrCreateCategoryAsync(context, category) });
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

internal static class PostFeatureHelpers
{
    internal static Task<Post?> GetByIdAsync(Context context, int id, bool tracked = false)
    {
        var query = context.Posts.Include(p => p.PostCategories).ThenInclude(pc => pc.Category).AsQueryable();
        if (!tracked) query = query.AsNoTracking();
        return query.FirstOrDefaultAsync(p => p.Id == id);
    }

    internal static PostViewModel ToViewModel(Post post) => new()
    {
        Id = post.Id, Title = post.Title, Description = post.Description, Content = post.Content,
        CreatedDate = post.CreatedDate, UpdatedTime = post.UpdatedDate,
        Category = post.PostCategories.FirstOrDefault()?.Category?.Title, IsDraft = post.IsDraft
    };

    internal static async Task<Post> CreateAsync(Context context, TimeProvider timeProvider, PostViewModel model, bool draft)
    {
        var post = new Post { Title = model.Title, Description = model.Description, Content = model.Content, CreatedDate = timeProvider.GetUtcNow().UtcDateTime, IsDraft = draft };
        if (!string.IsNullOrWhiteSpace(model.Category) && model.Category != "Category")
            post.PostCategories.Add(new PostCategory { Category = await FindOrCreateCategoryAsync(context, model.Category) });
        context.Posts.Add(post);
        await context.SaveChangesAsync();
        return post;
    }

    internal static async Task<Category> FindOrCreateCategoryAsync(Context context, string title)
    {
        var normalized = title.Trim();
        var category = await context.Categories.FirstOrDefaultAsync(c => c.Title == normalized);
        if (category is not null) return category;
        category = new Category { Title = normalized };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }
}
