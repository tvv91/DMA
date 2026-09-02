using DMA.Domain.Posts;
using MediatR;

namespace DMA.Application.Posts.GetById;

public sealed record GetPostByIdQuery(int Id) : IRequest<Post?>;

public sealed class GetPostByIdQueryHandler(IPostRepository postRepository)
    : IRequestHandler<GetPostByIdQuery, Post?>
{
    public Task<Post?> Handle(GetPostByIdQuery request, CancellationToken cancellationToken) =>
        postRepository.GetByIdWithCategoriesAsync(request.Id, cancellationToken);
}
