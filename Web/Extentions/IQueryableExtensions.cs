using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Web.Common;

namespace Web.Extentions
{
    public static class IQueryableExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, int page, int pageSize)
        {
            if (page < 1) 
                page = 1;

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(e => 0)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>(items, totalItems, page, pageSize);
        }

        public static async Task<PagedResult<T>> ToPagedResultAsync<T, TKey>(this IQueryable<T> query, int page, int pageSize, Expression<Func<T, TKey>> orderBy)
        {
            if (page < 1) 
                page = 1;

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(orderBy)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<T>(items, totalItems, page, pageSize);
        }
    }
}
