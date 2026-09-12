using MediatR;
using Web.Interfaces;
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

public sealed class GetPostQueryHandler(IPostService postService) : IRequestHandler<GetPostQuery, PostViewModel>
{
    public Task<PostViewModel> Handle(GetPostQuery request, CancellationToken cancellationToken) => postService.GetPostViewModelAsync(request.Id);
}

public sealed class CreatePostCommandHandler(IPostService postService) : IRequestHandler<CreatePostCommand, int>
{
    public async Task<int> Handle(CreatePostCommand request, CancellationToken cancellationToken) =>
        (await postService.CreatePostAsync(request.Model)).Id;
}

public sealed class UpdatePostCommandHandler(IPostService postService) : IRequestHandler<UpdatePostCommand, int>
{
    public async Task<int> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        if (request.Model.Id is null)
            throw new ArgumentException("Post id is required", nameof(request.Model));
        return (await postService.UpdatePostAsync(request.Model.Id.Value, request.Model)).Id;
    }
}

public sealed class DeletePostCommandHandler(IPostService postService) : IRequestHandler<DeletePostCommand, bool>
{
    public Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken) => postService.DeletePostAsync(request.Id);
}
