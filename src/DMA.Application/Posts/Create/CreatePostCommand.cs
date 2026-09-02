using DMA.Domain.Common;
using DMA.Domain.Posts;
using MediatR;

namespace DMA.Application.Posts.Create;

public sealed record CreatePostCommand(string Title, string Description, string Content, string? Category, bool IsDraft) : IRequest<Post>;

public sealed class CreatePostCommandHandler(
    IPostRepository postRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IRequestHandler<CreatePostCommand, Post>
{
    public async Task<Post> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = new Post
        {
            Title = request.Title,
            Description = request.Description,
            Content = request.Content,
            CreatedDate = timeProvider.GetUtcNow().UtcDateTime,
            IsDraft = request.IsDraft
        };

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            if (!request.IsDraft || request.Category != "Category")
            {
                var category = await postRepository.FindOrCreateCategoryAsync(request.Category, cancellationToken);
                post.PostCategories.Add(new PostCategory { Category = category });
            }
        }

        postRepository.Add(post);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return post;
    }
}
