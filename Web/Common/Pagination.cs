namespace Web.Common
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int TotalItems { get; }
        public int PageSize { get; }
        public int CurrentPage { get; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public PagedResult(IReadOnlyList<T> items, int totalItems, int currentPage, int pageSize)
        {
            Items = items;
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
        }
    }
}
