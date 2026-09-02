using DMA.Application.Posts.Create;
using DMA.Application.Posts.Delete;
using DMA.Application.Posts.GetById;
using DMA.Application.Posts.GetFilteredList;
using DMA.Application.Posts.GetList;
using DMA.Application.Posts.Update;
using MediatR;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Services;

public class PostService(IMediator mediator) : IPostService
{
    public Task<PagedResult<Post>> GetListAsync(int page, int pageSize) =>
        mediator.Send(new GetPostListQuery(page, pageSize));

    public Task<PagedResult<Post>> GetFilteredListAsync(int page, int pageSize, string? searchText, string? category, string? year, bool onlyDrafts, bool excludeDrafts = false) =>
        mediator.Send(new GetFilteredPostListQuery(page, pageSize, searchText, category, year, onlyDrafts, excludeDrafts));

    public Task<Post?> GetByIdAsync(int id) =>
        mediator.Send(new GetPostByIdQuery(id));

    public async Task<PostViewModel> GetPostViewModelAsync(int id)
    {
        var post = await GetByIdAsync(id);
        if (post is null)
            throw new KeyNotFoundException($"Post with id {id} not found");

        return MapPostToViewModel(post);
    }

    public Task<Post> CreatePostAsync(PostViewModel model) =>
        mediator.Send(new CreatePostCommand(model.Title, model.Description, model.Content, model.Category, IsDraft: false));

    public Task<Post> CreateDraftPostAsync(PostViewModel model) =>
        mediator.Send(new CreatePostCommand(model.Title, model.Description, model.Content, model.Category, IsDraft: true));

    public Task<Post> UpdatePostAsync(int postId, PostViewModel model) =>
        mediator.Send(new UpdatePostCommand(postId, model.Title, model.Description, model.Content, model.Category));

    public Task<bool> DeletePostAsync(int id) =>
        mediator.Send(new DeletePostCommand(id));

    public PostViewModel MapPostToViewModel(Post post) => new()
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
