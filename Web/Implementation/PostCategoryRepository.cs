using Web.Common;
using Web.Db;
using Web.Interfaces;
using Web.Models;

namespace Web.Implementation
{
    public class PostCategoryRepository : IPostCategoryRepository
    {
        private readonly DMADbContext _context;
        
        public PostCategoryRepository(DMADbContext ctx) => _context = ctx;

        public Task<PostCategory> AddAsync(PostCategory entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<PostCategory>> GetListAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<PostCategory?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PostCategory> UpdateAsync(PostCategory entity)
        {
            throw new NotImplementedException();
        }
    }
}
