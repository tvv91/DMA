using Web.Common;
using Web.Models;

namespace Web.Interfaces
{
    public interface IPostRepository : IRepository<Post>
    {
        public Task<PagedResult<Post>> GetFilteredListAsync(int page, int pageSize, string? searchText, string? category, string? year, bool onlyDrafts);
    }
}
