using Microsoft.AspNetCore.Mvc;
using MediatR;
using Web.Enums;
using Web.Features.Supporting;

namespace Web.Controllers
{
    [Route("search")]
    public class SearchController(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        [HttpGet("{entityType}")]
        public async Task<IActionResult> Search(string entityType, [FromQuery] string? value)
        {
            if (!Enum.TryParse<EntityType>(entityType, true, out var entityTypeEnum))
                return BadRequest($"Invalid entity type: {entityType}");
            return Ok(await _sender.Send(new SearchQuery(entityTypeEnum, value ?? string.Empty)));
        }
    }
}
