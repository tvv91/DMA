using Web.Common;

namespace Web.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<PagedResult<T>> GetListAsync(int page, int pageSize);
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}