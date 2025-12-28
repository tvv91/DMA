using Microsoft.AspNetCore.Mvc;
using Web.Enums;
using Web.Interfaces;
using Web.Response;

namespace Web.Controllers
{
    [Route("search")]
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet("{entityType}")]
        public async Task<IActionResult> Search(string entityType, [FromQuery] string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Ok(new List<AutocompleteResponse>());

            if (Enum.TryParse<EntityType>(entityType, true, out var entityTypeEnum))
            {
                var results = await _searchService.SearchAsync(entityTypeEnum, value);
                return Ok(results);
            }

            return BadRequest($"Invalid entity type: {entityType}");
        }
    }
}
