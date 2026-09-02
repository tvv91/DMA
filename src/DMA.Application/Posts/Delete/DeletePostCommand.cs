using DMA.Domain.Common;
using DMA.Domain.Posts;
using MediatR;

namespace DMA.Application.Posts.Delete;

public sealed record DeletePostCommand(int Id) : IRequest<bool>;

public sealed class DeletePostCommandHandler(IPostRepository postRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeletePostCommand, bool>
{
    public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await postRepository.FindAsync(request.Id, cancellationToken);
        if (post is null)
            return false;

        postRepository.Remove(post);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
