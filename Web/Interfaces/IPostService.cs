using Web.Common;
using Web.Models;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IPostService
    {
        Task<PagedResult<Post>> GetListAsync(int page, int pageSize);
        Task<PagedResult<Post>> GetFilteredListAsync(int page, int pageSize, string? searchText, string? category, string? year, bool onlyDrafts);
        Task<Post?> GetByIdAsync(int id);
        Task<PostViewModel> GetPostViewModelAsync(int id);
        Task<Post> CreatePostAsync(PostViewModel model);
        Task<Post> CreateDraftPostAsync(PostViewModel model);
        Task<Post> UpdatePostAsync(int postId, PostViewModel model);
        Task<bool> DeletePostAsync(int id);
        PostViewModel MapPostToViewModel(Post post);
    }
}

