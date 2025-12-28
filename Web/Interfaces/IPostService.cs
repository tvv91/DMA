using Web.Models;
using Web.ViewModels;

namespace Web.Interfaces
{
    public interface IPostService
    {
        Task<Post?> GetByIdAsync(int id);
        Task<PostViewModel> GetPostViewModelAsync(int id);
        Task<Post> CreatePostAsync(PostViewModel model);
        Task<Post> UpdatePostAsync(int postId, PostViewModel model);
        Task<bool> DeletePostAsync(int id);
        PostViewModel MapPostToViewModel(Post post);
    }
}

