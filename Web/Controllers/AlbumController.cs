using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Services;
using Web.SignalRHubs;
using Web.ViewModels;

namespace Web.Controllers
{
    public class AlbumController : Controller
    {
        private const int DEFAULT_ALBUMS_PER_PAGE = 15;
        private const int MAX_ALBUMS_PER_PAGE = 30;
        private readonly IAlbumService _albumService;
        private readonly IImageService _imageService;
        private readonly IDigitizationService _digitizationService;
        private readonly DMADbContext _context;

        public AlbumController(IAlbumService albumService, IImageService imageService, IDigitizationService digitizationService, DMADbContext context)
        {
            _albumService = albumService;
            _imageService = imageService;
            _digitizationService = digitizationService;
            _context = context;
        }

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

            var result = await _albumService.GetIndexListAsync(page, pageSize, artistName, genreName, yearValue, albumTitle);

            // Check if there are any albums in the database at all (unfiltered)
            var hasAnyAlbumsInDb = await _context.Albums.AnyAsync();

            var vm = new AlbumIndexViewModel
            {
                CurrentPage = page,
                PageCount = result.TotalPages,
                Albums = result.Items,
                PageSize = pageSize,
                HasAnyAlbumsInDb = hasAnyAlbumsInDb,
                ArtistName = artistName,
                GenreName = genreName,
                YearValue = yearValue,
                AlbumTitle = albumTitle
            };

            return View("Index", vm);
        }

        [HttpGet("album/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id < 1)
                return BadRequest();

            try
            {
                var vm = await _albumService.GetAlbumDetailsAsync(id);
                return View("Details", vm);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("album/create")]
        public IActionResult Create()
        {
            return View("CreateUpdate", new AlbumCreateUpdateViewModel 
            { 
                Action = ActionType.Create 
            });
        }

        [HttpGet("album/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id < 1)
                return BadRequest("Invalid album Id");

            try
            {
                var album = await _albumService.GetByIdAsync(id);
                if (album == null)
                    return NotFound();

                var vm = await _albumService.MapAlbumToCreateUpdateVMAsync(album);
                return View("CreateUpdate", vm);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(AlbumCreateUpdateViewModel request)
        {
            if (!ModelState.IsValid)
                return View("CreateUpdate", request);

            try
            {
                var album = await _albumService.CreateOrFindAlbumAsync(request.Title, request.Artist, request.Genre);

                if (request.AlbumCover != null)
                    await _imageService.SaveCoverAsync(album.Id, request.AlbumCover, EntityType.AlbumCover);

                return RedirectToAction("GetById", "Album", new { id = album.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to save album. {ex.Message}");
                return View("CreateUpdate", request);
            }
        }

        [HttpPost("album/update")]
        public async Task<IActionResult> Update(AlbumCreateUpdateViewModel request)
        {
            if (request.AlbumId < 1)
                return BadRequest("Invalid album ID");

            if (!ModelState.IsValid)
                return View("CreateUpdate", request);

            try
            {
                var album = await _albumService.UpdateAlbumAsync(request.AlbumId, request.Title, request.Artist, request.Genre);

                // Handle album cover:
                // - If AlbumCover is the album ID, the image hasn't changed (don't do anything)
                // - If AlbumCover is a temp filename, save it (new image uploaded)
                // - If AlbumCover is null/empty, remove the cover (user removed it)
                if (string.IsNullOrWhiteSpace(request.AlbumCover))
                {
                    await _imageService.RemoveCoverAsync(album.Id, EntityType.AlbumCover);
                }
                else if (request.AlbumCover != album.Id.ToString())
                {
                    // It's a new temp filename, save it
                    await _imageService.SaveCoverAsync(album.Id, request.AlbumCover, EntityType.AlbumCover);
                }
                // If AlbumCover equals album ID, image hasn't changed - do nothing

                AlbumHub.InvalidateAlbumCache(album.Id);

                return RedirectToAction("GetById", "Album", new { id = request.AlbumId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to update album. " + ex.Message);
                return View("CreateUpdate", request);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1)
                return BadRequest("Invalid album ID");

            if (!await _albumService.DeleteAlbumAsync(id))
                return NotFound();                

            try
            {
                await _imageService.RemoveCoverAsync(id, EntityType.AlbumCover);
            }
            catch (Exception ex)
            {
                // TODO: Add logging
                return BadRequest("Failed to delete album");
            }
            return Ok();
        }
    }
}
