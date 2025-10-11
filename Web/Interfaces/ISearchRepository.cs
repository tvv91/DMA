using Web.Enums;
using Web.Response;

namespace Web.Interfaces
{
    public interface ISearchRepository
    {
        Task<List<AutocompleteResponse>> SearchAsync(EntityType entity, string value);
    }
}
