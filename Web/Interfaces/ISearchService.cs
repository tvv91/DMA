using Web.Enums;
using Web.Response;

namespace Web.Interfaces
{
    public interface ISearchService
    {
        Task<List<AutocompleteResponse>> SearchAsync(EntityType entityType, string value);
    }
}

