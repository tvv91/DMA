using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Web.Db;
using Web.Enum;
using Web.Request;
using Web.Response;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class AlbumController : Controller
    {
        private const int ALBUMS_PER_PAGE = 15;
        private readonly IAlbumRepository _albumRepository;
        private readonly IImageService _imageService;
        private readonly ITechInfoRepository _techInfoRepository;

        public AlbumController(IAlbumRepository albumRepository, IImageService imageService, ITechInfoRepository tinfoRepository)
        {
            _albumRepository = albumRepository;
            _imageService = imageService;
            _techInfoRepository = tinfoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            AlbumViewModel albumViewModel = new AlbumViewModel
            {
                 Albums = await _albumRepository.Albums
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
            
            var album = await _albumRepository.Albums
                .Include(a => a.Artist)
                .Include(a => a.Country)
                .Include(a => a.Genre)
                .Include(a => a.Label)
                .Include(a => a.Reissue)
                .Include(a => a.Year)
                .Include(a => a.Storage)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (album == null)
            {
                return NotFound();
            }

            var tinfo = await _techInfoRepository.TechInfos
                .Include(x => x.VinylState)
                .Include(x => x.DigitalFormat)
                .Include(x => x.Bitness)
                .Include(x => x.Sampling)
                .Include(x => x.SourceFormat)
                .Include(x => x.Player)
                .ThenInclude(x => x.Manufacturer)
                .Include(x => x.Cartrige)
                .ThenInclude(x => x.Manufacturer)
                .Include(x => x.Amplifier)
                .ThenInclude(x => x.Manufacturer)
                .Include(x => x.Adc)
                .ThenInclude(x => x.Manufacturer)
                .Include(x => x.Adc)
                .ThenInclude(x => x.Manufacturer)
                .Include(x => x.Wire)
                .ThenInclude(x => x.WireManufacturer)
                .Include(x => x.Processing)
                .FirstOrDefaultAsync(x => x.Id == album.Id);

            if (tinfo != null)
            {
                album.TechnicalInfo = tinfo;
            }

            return View("AlbumDetails", new AlbumDetailsViewModel {  Album = album });
        }

        [HttpGet("album/create")]
        public async Task<IActionResult> Create()
        {
            return View("CreateOrUpdate", new AlbumDataRequest());
        }

        [HttpGet("album/edit/{albumId}")]
        public async Task<IActionResult> Edit(int albumId)
        {
            if (albumId <= 0)
                return BadRequest();
            
            var album = await _albumRepository.Albums
                .Include(a => a.Artist)
                .Include(a => a.Country)
                .Include(a => a.Genre)
                .Include(a => a.Label)
                .Include(a => a.Reissue)
                .Include(a => a.Year)
                .Include(a => a.Storage)
                .FirstOrDefaultAsync(a => a.Id == albumId);

            if (album == null)
                return NotFound();
            
            var tInfo = await _techInfoRepository.TechInfos
                .Include(i => i.VinylState)
                .Include(i => i.DigitalFormat)
                .Include(i => i.Bitness)
                .Include(i => i.Sampling)
                .Include(i => i.SourceFormat)
                .Include(i => i.Player)
                .Include(i => i.Cartrige)
                .Include(i => i.Amplifier)
                .Include(i => i.Adc)
                .Include(i => i.Wire)
                .Include(i => i.Processing)
                .FirstOrDefaultAsync(i => i.AlbumId == albumId);            

            var cover = _imageService.GetImageUrl(album.Id, EntityType.AlbumDetailCover);

            var albumModel = new AlbumDataRequest
            {
                // Need for edit
                IsEdit = true,
                AlbumId = album.Id,
                // Base info
                Album = album.Data,
                Artist = album.Artist.Data,
                Genre = album.Genre.Data,
                Year = album.Year.Data,
                Reissue = album.Reissue?.Data,
                Country = album.Country?.Data,
                Label = album.Label?.Data,
                Source = album.Source,
                Size = album.Size,
                Storage = album.Storage?.Data,
                Discogs = album.Discogs,
                AlbumCover = cover,
                // Technical info
                VinylState = tInfo?.VinylState?.Data,
                DigitalFormat = tInfo?.DigitalFormat?.Data,
                Bitness = tInfo?.Bitness?.Data,
                Sampling = tInfo?.Sampling?.Data,
                SourceFormat = tInfo?.SourceFormat?.Data,
                Player = tInfo?.Player?.Data,
                Cartridge = tInfo?.Cartrige?.Data,
                Amplifier = tInfo?.Amplifier?.Data,
                Adc = tInfo?.Adc?.Data,
                Wire = tInfo?.Wire?.Data,
                Processing = tInfo?.Processing?.Data,
            };
            return View("CreateOrUpdate", albumModel);
        }

        [HttpPost]
        public async Task<IActionResult> NewAlbum(AlbumDataRequest request)
        {
            if (ModelState.IsValid 
                && !string.IsNullOrEmpty(request.Artist) 
                && !string.IsNullOrEmpty(request.Album) 
                && !string.IsNullOrEmpty(request.Genre))
            {
                var album = await _albumRepository.CreateNewAlbum(request);
                if (request.AlbumCover != null)
                {
                    _imageService.SaveCover(album.Id, request.AlbumCover);
                }

                return new RedirectResult($"{album.Id}");
            } 
            else
            {
                return View("CreateOrUpdate", request);
            }
        }

        [HttpPost("album/update/{albumId}")]
        public async Task<IActionResult> Update(AlbumDataRequest request, int albumId)
        {
            if (albumId <= 0 || albumId > int.MaxValue)
                return BadRequest();

            if (ModelState.IsValid
                && !string.IsNullOrEmpty(request.Artist)
                && !string.IsNullOrEmpty(request.Album)
                && !string.IsNullOrEmpty(request.Genre))
            {
                var album = await _albumRepository.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .Include(a => a.Year)
                .Include(a => a.Reissue)
                .Include(a => a.Country)
                .Include(a => a.Label)
                .Include(a => a.Storage)
                .FirstOrDefaultAsync(x => x.Id == albumId);

                if (album == null)
                {
                    return NotFound();
                }

                await _albumRepository.UpdateAlbum(album, request);
                await _techInfoRepository.UpdateTechnicalInfoAsync(album.Id, request);

                if (request.AlbumCover == null)
                {
                    _imageService.RemoveCover(album.Id);
                } 
                else
                {
                    _imageService.SaveCover(album.Id, request.AlbumCover);
                }

                return new RedirectResult($"../{albumId}");
            }
            else
            {
                return View("CreateOrUpdate", request);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0 || id > int.MaxValue)
                return BadRequest();
            var album = await _albumRepository.Albums.Where(a => a.Id == id).SingleOrDefaultAsync();
            if (album == null)
                return NotFound();
            try
            {
                _imageService.RemoveCover(album.Id);
                await _techInfoRepository.TechInfos.Where(t => t.AlbumId == id).ExecuteDeleteAsync();
                await _albumRepository.Albums.Where(a => a.Id == id).ExecuteDeleteAsync();
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
            return Ok(await _albumRepository.Artists.Where(x => x.Data.Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
        }
        
        [HttpGet("search/genre")]
        public async Task<IActionResult> SearchGenre(string term)
        {
            return Ok(await _albumRepository.Genres.Where(x => x.Data.Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
        }

        [HttpGet("search/year")]
        public async Task<IActionResult> SearchYear(string term)
        {
            return Ok(await _albumRepository.Years.Where(x => x.Data.ToString().Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data.ToString() }).ToArrayAsync());
        }

        [HttpGet("search/reissue")]
        public async Task<IActionResult> SearchReissue(string term)
        {
            return Ok(await _albumRepository.Reissues.Where(x => x.Data.ToString().Contains(term)).Select(x => new AutocompleteResponse { Label = x.Data.ToString() }).ToArrayAsync());
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
