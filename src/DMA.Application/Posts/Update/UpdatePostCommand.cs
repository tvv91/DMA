using DMA.Domain.Common;
using DMA.Domain.Posts;
using MediatR;

namespace DMA.Application.Posts.Update;

public sealed record UpdatePostCommand(int PostId, string Title, string Description, string Content, string? Category) : IRequest<Post>;

public sealed class UpdatePostCommandHandler(
    IPostRepository postRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<UpdatePostCommand, Post>
{
    public async Task<Post> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var existing = await postRepository.FindTrackedWithCategoriesAsync(request.PostId, cancellationToken)
            ?? throw new KeyNotFoundException($"Post with Id {request.PostId} not found.");

        existing.Title = request.Title;
        existing.Description = request.Description;
        existing.Content = request.Content;
        existing.UpdatedDate = timeProvider.GetUtcNow().UtcDateTime;

        var currentCategory = existing.PostCategories.FirstOrDefault()?.Category?.Title;
        var newCategory = request.Category?.Trim();

        if (newCategory != currentCategory && !string.IsNullOrWhiteSpace(newCategory) && newCategory != "Category")
        {
            existing.PostCategories.Clear();
            var category = await postRepository.FindOrCreateCategoryAsync(newCategory, cancellationToken);
            existing.PostCategories.Add(new PostCategory { Category = category });
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return existing;
    }
}
