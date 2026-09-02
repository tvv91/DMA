using DMA.Domain.Common;
using DMA.Domain.Posts;
using MediatR;

namespace DMA.Application.Posts.GetFilteredList;

public sealed record GetFilteredPostListQuery(
    int Page,
    int PageSize,
    string? SearchText,
    string? Category,
    string? Year,
    bool OnlyDrafts,
    bool ExcludeDrafts = false) : IRequest<PagedResult<Post>>;

public sealed class GetFilteredPostListQueryHandler(IPostRepository postRepository)
    : IRequestHandler<GetFilteredPostListQuery, PagedResult<Post>>
{
    public Task<PagedResult<Post>> Handle(GetFilteredPostListQuery request, CancellationToken cancellationToken) =>
        postRepository.GetFilteredListAsync(
            request.Page,
            request.PageSize,
            request.SearchText,
            request.Category,
            request.Year,
            request.OnlyDrafts,
            request.ExcludeDrafts,
            cancellationToken);
}
