using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Web.Db;
using Web.Models;
using Web.Request;
using Web.Response;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class AlbumController : Controller
    {
        private const int ALBUMS_PER_PAGE = 15;
        private readonly IAlbumRepository _repository;
        private readonly IImageService _imageService;
        private readonly ITechInfoRepository _techInfoRepository;

        public AlbumController(IAlbumRepository albumRepository, IImageService imageService, ITechInfoRepository tinfoRepository)
        {
            _repository = albumRepository;
            _imageService = imageService;
            _techInfoRepository = tinfoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            AlbumViewModel albumViewModel = new AlbumViewModel
            {
                 Albums = await _repository.Albums
                 .Skip((page - 1) * ALBUMS_PER_PAGE)
                 .Take(ALBUMS_PER_PAGE)
                 .Include(a => a.Artist)
                 .ToListAsync()
            };
            return View("Index", albumViewModel);
        }
         
        [HttpGet("album/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id == 0 || id < 0)
            {
                return BadRequest();
            }
            
            Album? album = await _repository.Albums
                .Include(a => a.Artist)
                .Include(a => a.Country)
                .Include(a => a.Genre)
                .Include(a => a.Label)
                .Include(a => a.Reissue)
                .Include(a => a.Year)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (album == null)
            {
                return NotFound();
            }

            return View("AlbumDetails", new AlbumDetailsViewModel {  Album = album });
        }

        [HttpGet("album/create")]
        public async Task<IActionResult> Create()
        {
            return View("New");
        }

        [HttpPost]
        public async Task<IActionResult> NewAlbum(NewAlbumRequest request)
        {
            if (ModelState.IsValid 
                && !string.IsNullOrEmpty(request.Artist) 
                && !string.IsNullOrEmpty(request.Album) 
                && !string.IsNullOrEmpty(request.Genre))
            {
                var album = await _repository.CreateNewAlbum(request);
                if (request.AlbumCover != null)
                {
                    _imageService.SaveCover(album.Id, request.AlbumCover);
                }
                return new RedirectResult($"{album.Id}");
            } 
            else
            {
                return View("New");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0 || id > int.MaxValue)
                return BadRequest();
            var album = await _repository.Albums.Where(a => a.Id == id).SingleOrDefaultAsync();
            if (album == null)
                return NotFound();
            try
            {
                _imageService.RemoveCover(album.Id);
                await _techInfoRepository.TechInfos.Where(t => t.AlbumId == id).ExecuteDeleteAsync();
                await _repository.Albums.Where(a => a.Id == id).ExecuteDeleteAsync();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpGet("search/artist")]
        public async Task<IActionResult> SearchArtist(string term)
        {
            return Ok(await _repository.Artists.Where(x => x.Data.Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
        }
        
        [HttpGet("search/genre")]
        public async Task<IActionResult> SearchGenre(string term)
        {
            return Ok(await _repository.Genres.Where(x => x.Data.Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
        }

        [HttpGet("search/year")]
        public async Task<IActionResult> SearchYear(string term)
        {
            return Ok(await _repository.Years.Where(x => x.Data.ToString().Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data.ToString() }).ToArrayAsync());
        }

        [HttpPost("/uploadcover")]
        public async Task<IActionResult> UploadCover(IFormFile filedata)
        {
            var files = HttpContext.Request.Form.Files;
            if (files.Any())
            {
                var guid = Guid.NewGuid().ToString("N");
                var ext = Path.GetExtension(files[0].FileName);
                await using var target = new MemoryStream();
                await files[0].CopyToAsync(target);
                var physicalPath = $"{new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp")).Root}{$@"{guid}{ext}"}";
                await using FileStream fs = System.IO.File.Create(physicalPath);
                await files[0].CopyToAsync(fs);
                fs.Flush();
                return Json(new { Filename = $"{guid}{ext}" });
            }
            return Ok();
        }
    }
}
