using Microsoft.AspNetCore.Mvc;
using Web.Enums;
using Web.Interfaces;
using Web.Response;

namespace Web.Controllers
{
    [Route("search")]
    public class SearchController : Controller
    {
        private ISearchRepository _searchRepository;

        public SearchController(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        [HttpGet("{entityType}")]
        public async Task<IActionResult> Search(string entityType, [FromQuery] string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Ok(new List<AutocompleteResponse>());

            if (Enum.TryParse<EntityType>(entityType, true, out var entityTypeEnum))
            {
                var results = await _searchRepository.SearchAsync(entityTypeEnum, value);
                return Ok(results);
            }

            return BadRequest($"Invalid entity type: {entityType}");
        }
    }
}
