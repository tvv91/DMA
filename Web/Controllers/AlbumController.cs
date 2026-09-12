using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Enums;
using Web.Authorization;
using Web.ViewModels;
using MediatR;
using Web.Features.Albums.Create;
using Web.Features.Albums.Delete;
using Web.Features.Albums.Edit;
using Web.Features.Albums.GetById;
using Web.Features.Albums.Index;
using Web.Features.Albums.Update;

namespace Web.Controllers
{
    public class AlbumController(ISender sender) : Controller
    {
        private const int DEFAULT_ALBUMS_PER_PAGE = 15;
        private const int MAX_ALBUMS_PER_PAGE = 30;
        private readonly ISender _sender = sender;

        [HttpGet("album")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 0, string? artistName = null, string? genreName = null, string? yearValue = null, string? albumTitle = null)
        {
            if (page < 1)
                return BadRequest("Page number should be positive");

            // Use default if pageSize is 0 or invalid, otherwise clamp to max
            if (pageSize <= 0)
                pageSize = DEFAULT_ALBUMS_PER_PAGE;
            else if (pageSize > MAX_ALBUMS_PER_PAGE)
                pageSize = MAX_ALBUMS_PER_PAGE;

            var vm = await _sender.Send(new IndexQuery(page, pageSize, artistName, genreName, yearValue, albumTitle));
            return View("Index", vm);
        }

        [HttpGet("album/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id < 1)
                return BadRequest();

            try
            {
                var vm = await _sender.Send(new GetByIdQuery(id));
                return View("Details", vm);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("album/create")]
        public IActionResult Create()
        {
            return View("CreateUpdate", new AlbumCreateUpdateViewModel 
            { 
                Action = ActionType.Create 
            });
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("album/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id < 1)
                return BadRequest("Invalid album Id");

            try
            {
                var vm = await _sender.Send(new EditAlbumQuery(id));
                if (vm is null)
                    return NotFound();

                return View("CreateUpdate", vm);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(AlbumCreateUpdateViewModel request)
        {
            if (!ModelState.IsValid)
                return View("CreateUpdate", request);

            try
            {
                var albumId = await _sender.Send(new CreateAlbumCommand(request));

                return RedirectToAction("GetById", "Album", new { id = albumId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to save album. {ex.Message}");
                return View("CreateUpdate", request);
            }
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpPost("album/update")]
        public async Task<IActionResult> Update(AlbumCreateUpdateViewModel request)
        {
            if (request.AlbumId < 1)
                return BadRequest("Invalid album ID");

            if (!ModelState.IsValid)
                return View("CreateUpdate", request);

            try
            {
                var albumId = await _sender.Send(new UpdateAlbumCommand(request));

                return RedirectToAction("GetById", "Album", new { id = albumId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to update album. " + ex.Message);
                return View("CreateUpdate", request);
            }
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1)
                return BadRequest("Invalid album ID");

            if (!await _sender.Send(new DeleteAlbumCommand(id)))
                return NotFound();                
            return Ok();
        }
    }
}
