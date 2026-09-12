using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Web.Common;

namespace Web.Extensions;

public static class QueryablePaginationExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T, TKey>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        Expression<Func<T, TKey>> orderBy,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(orderBy);

        page = Math.Max(page, 1);
        pageSize = Math.Max(pageSize, 1);

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(orderBy)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(items, totalItems, page, pageSize);
    }
}
