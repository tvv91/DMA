using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Enums;
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
            if (page > 0)
            {
                int albumsCount = await _albumRepository.Albums.CountAsync();
                var albumViewModel = new AlbumViewModel
                {
                    CurrentPage = page,
                    PageCount = albumsCount % ALBUMS_PER_PAGE == 0 ? albumsCount / ALBUMS_PER_PAGE : albumsCount / ALBUMS_PER_PAGE + 1,
                    Albums = await _albumRepository.Albums
                     .Skip((page - 1) * ALBUMS_PER_PAGE)
                     .Take(ALBUMS_PER_PAGE)
                     .Include(a => a.Artist)
                     .AsNoTracking()
                     .ToListAsync()
                };
                return View("Index", albumViewModel);
            }
            else
            {
                return BadRequest("Page number should be positive");
            }            
        }
         
        [HttpGet("album/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id == 0 || id < 0)
            {
                return BadRequest();
            }
            
            var album = await _albumRepository.GetByIdAsync(id);
            
            if (album == null)
            {
                return NotFound();
            }

            var tinfo = await _techInfoRepository.GetByIdAsync(id);

            if (tinfo != null)
            {
                album.TechnicalInfo = tinfo;
            }

            return View("AlbumDetails", new AlbumDetailsViewModel {  Album = album });
        }

        [HttpGet("album/create")]
        public IActionResult Create()
        {
            return View("CreateOrUpdate", new AlbumCreateUpdateViewModel());
        }

        [HttpGet("album/edit/{albumId}")]
        public async Task<IActionResult> Edit(int albumId)
        {
            if (albumId <= 0)
                return BadRequest();
            
            var album = await _albumRepository.GetByIdAsync(albumId);

            if (album == null)
                return NotFound();
            
            var tInfo = await _techInfoRepository.GetByIdAsync(albumId);

            var cover = _imageService.GetImageUrl(album.Id, EntityType.AlbumCover);

            var albumModel = new AlbumCreateUpdateViewModel
            {
                Action = ActionType.Update,
                AlbumId = album.Id,
                // Base info
                Album = album.Data,
                Artist = album.Artist.Data,
                Genre = album.Genre?.Data,
                Year = album.Year?.Data,
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
                PlayerManufacturer = tInfo?.Player?.Manufacturer?.Data,
                Cartridge = tInfo?.Cartridge?.Data,
                CartridgeManufacturer = tInfo?.Cartridge?.Manufacturer?.Data,
                Amplifier = tInfo?.Amplifier?.Data,
                AmplifierManufacturer = tInfo?.Amplifier?.Manufacturer?.Data,
                Adc = tInfo?.Adc?.Data,
                AdcManufacturer = tInfo?.Adc?.Manufacturer?.Data,
                Wire = tInfo?.Wire?.Data,
                WireManufacturer = tInfo?.Wire?.Manufacturer?.Data
            };
            return View("CreateOrUpdate", albumModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AlbumCreateUpdateViewModel request)
        {
            if (ModelState.IsValid 
                && !string.IsNullOrEmpty(request.Artist) 
                && !string.IsNullOrEmpty(request.Album))
            {                
                var album = await _albumRepository.CreateOrUpdateAlbumAsync(request);
                var tinfo = await _techInfoRepository.CreateOrUpdateTechnicalInfoAsync(album, request);

                if (request.AlbumCover != null)
                {
                    _imageService.SaveCover(album.Id, request.AlbumCover, EntityType.AlbumCover);
                }

                return new RedirectResult($"{album.Id}");
            } 
            else
            {
                return View("CreateOrUpdate", request);
            }
        }

        [HttpPost("album/update")]
        public async Task<IActionResult> Update(AlbumCreateUpdateViewModel request)
        {
            if (request.AlbumId <= 0)
                return BadRequest();

            if (ModelState.IsValid
                && !string.IsNullOrEmpty(request.Artist)
                && !string.IsNullOrEmpty(request.Album))
            {

                var album = await _albumRepository.CreateOrUpdateAlbumAsync(request);
                await _techInfoRepository.CreateOrUpdateTechnicalInfoAsync(album, request);

                if (request.AlbumCover == null)
                {
                    _imageService.RemoveCover(album.Id, EntityType.AlbumCover);
                } 
                else
                {
                    _imageService.SaveCover(album.Id, request.AlbumCover, EntityType.AlbumCover);
                }

                return new RedirectResult($"/album/{request.AlbumId}");
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
            var album = await _albumRepository.Albums.Where(a => a.Id == id).AsNoTracking().FirstOrDefaultAsync();
            if (album == null)
                return NotFound();
            try
            {
                _imageService.RemoveCover(album.Id, EntityType.AlbumCover);
                await _techInfoRepository.TechInfos.Where(t => t.AlbumId == id).ExecuteDeleteAsync();
                await _albumRepository.Albums.Where(a => a.Id == id).ExecuteDeleteAsync();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpGet("search/{category?}")]
        public async Task<IActionResult> Search(string category, string value)
        {
            switch (category)
            {
                case "artist":  return Ok(await _albumRepository.Artists.AsNoTracking().AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                case "genre": return Ok(await _albumRepository.Genres.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                case "year": return Ok(await _albumRepository.Years.AsNoTracking().Where(x => x.Data.ToString().Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data.ToString() }).ToArrayAsync());
                case "reissue": return Ok(await _albumRepository.Reissues.AsNoTracking().Where(x => x.Data.ToString()!.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data.ToString()! }).ToArrayAsync());
                // technical info
                case "vinylstate": return Ok(await _techInfoRepository.VinylStates.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                case "digitalformat": return Ok(await _techInfoRepository.DigitalFormats.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                case "bitness": return Ok(await _techInfoRepository.Bitnesses.AsNoTracking().Where(x => x.Data.ToString().Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data.ToString() }).ToArrayAsync());
                case "sampling": return Ok(await _techInfoRepository.Samplings.AsNoTracking().Where(x => x.Data.ToString().Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data.ToString() }).ToArrayAsync());
                case "sourceformat": return Ok(await _techInfoRepository.SourceFormats.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                case "player": return Ok(await _techInfoRepository.Players.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                case "cartridge": return Ok(await _techInfoRepository.Cartridges.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                case "amp": return Ok(await _techInfoRepository.Amplifiers.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                case "adc": return Ok(await _techInfoRepository.Adcs.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                case "wire": return Ok(await _techInfoRepository.Wires.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                // manufacturers
                case "player_manufacturer": return Ok(await _techInfoRepository.PlayerManufacturers.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                case "cartridge_manufacturer": return Ok(await _techInfoRepository.CartridgeManufacturers.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                case "amp_manufacturer": return Ok(await _techInfoRepository.AmplifierManufacturers.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                case "adc_manufacturer": return Ok(await _techInfoRepository.AdcManufacturers.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                case "wire_manufacturer": return Ok(await _techInfoRepository.WireManufacturers.AsNoTracking().Where(x => x.Data.Contains(value)).Select(x => new AutocompleteResponse { Label = x.Data }).ToArrayAsync());
                default: return Ok(new AutocompleteResponse());
            }
        }
    }
}
