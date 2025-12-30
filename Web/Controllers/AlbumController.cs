using Microsoft.AspNetCore.Mvc;
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

        public AlbumController(IAlbumService albumService, IImageService imageService, IDigitizationService digitizationService)
        {
            _albumService = albumService;
            _imageService = imageService;
            _digitizationService = digitizationService;
        }

        [HttpGet("album")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 0)
        {
            if (page < 1)
                return BadRequest("Page number should be positive");

            // Use default if pageSize is 0 or invalid, otherwise clamp to max
            if (pageSize <= 0)
                pageSize = DEFAULT_ALBUMS_PER_PAGE;
            else if (pageSize > MAX_ALBUMS_PER_PAGE)
                pageSize = MAX_ALBUMS_PER_PAGE;

            var result = await _albumService.GetIndexListAsync(page, pageSize);
            
            var vm = new AlbumIndexViewModel
            {
                CurrentPage = page,
                PageCount = result.TotalPages,
                Albums = result.Items,
                PageSize = pageSize
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
                    _imageService.SaveCover(album.Id, request.AlbumCover, EntityType.AlbumCover);

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

                if (request.AlbumCover is not null)
                    _imageService.SaveCover(album.Id, request.AlbumCover, EntityType.AlbumCover);
                else
                    _imageService.RemoveCover(album.Id, EntityType.AlbumCover);

                DefaultHub.InvalidateAlbumCache(album.Id);

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
                _imageService.RemoveCover(id, EntityType.AlbumCover);
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
