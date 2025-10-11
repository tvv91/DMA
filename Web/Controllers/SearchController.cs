using Microsoft.AspNetCore.Mvc;
using Web.Enums;
using Web.Interfaces;

namespace Web.Controllers
{
    public class SearchController : Controller
    {
        private ISearchRepository _searchRepository;

        public SearchController(ISearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        [HttpGet("{entityType}")]
        public async Task<IActionResult> Search(EntityType entityType, [FromQuery] string value)
        {
            var results = await _searchRepository.SearchAsync(entityType, value);
            return Ok(results);
        }
    }
}
