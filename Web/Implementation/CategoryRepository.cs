using Web.Common;
using Web.Interfaces;
using Web.Models;

namespace Web.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        public Task<Category> AddAsync(Category entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<Category>> GetListAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<Category?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Category> UpdateAsync(Category entity)
        {
            throw new NotImplementedException();
        }
    }
}
