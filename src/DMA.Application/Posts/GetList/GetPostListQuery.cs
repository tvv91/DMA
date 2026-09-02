using DMA.Domain.Common;
using DMA.Domain.Posts;
using MediatR;

namespace DMA.Application.Posts.GetList;

public sealed record GetPostListQuery(int Page, int PageSize) : IRequest<PagedResult<Post>>;

public sealed class GetPostListQueryHandler(IPostRepository postRepository)
    : IRequestHandler<GetPostListQuery, PagedResult<Post>>
{
    public Task<PagedResult<Post>> Handle(GetPostListQuery request, CancellationToken cancellationToken) =>
        postRepository.GetListAsync(request.Page, request.PageSize, cancellationToken);
}
