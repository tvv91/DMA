using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Web.Db;
using Web.Enums;
using Web.Extentions;
using Web.Models;
using Web.Response;
using Web.Services;
using Web.SignalRHubs;
using Web.ViewModels;

namespace Web.Controllers
{
    public class AlbumController : Controller
    {
        private const int ALBUMS_PER_PAGE = 15;
        private readonly IAlbumRepository _albumRepository;
        private readonly IImageService _imageService;
        private readonly ITechInfoRepository _techInfoRepository;
        private readonly Dictionary<string, Func<string, Task<IActionResult>>> _searchMap;

        public AlbumController(IAlbumRepository albumRepository, IImageService imageService, ITechInfoRepository tinfoRepository)
        {
            _albumRepository = albumRepository;
            _imageService = imageService;
            _techInfoRepository = tinfoRepository;
            _searchMap = new()
            {
                // album
                ["artist"] = v => SearchStringAsync(albumRepository.Artists, x => x.Data, v),
                ["genre"] = v => SearchStringAsync(albumRepository.Genres, x => x.Data, v),
                ["year"] = v => SearchNumberAsync(albumRepository.Years, x => x.Data, v),
                ["reissue"] = v => SearchNumberAsync(albumRepository.Reissues, x => x.Data, v),
                // tech info
                ["vinylstate"] = v => SearchStringAsync(_techInfoRepository.VinylStates, x => x.Data, v),
                ["digitalformat"] = v => SearchStringAsync(_techInfoRepository.DigitalFormats, x => x.Data, v),
                ["bitness"] = v => SearchNumberAsync(_techInfoRepository.Bitnesses, x => x.Data, v),
                ["sampling"] = v => SearchNumberAsync(_techInfoRepository.Samplings, x => x.Data, v),
                ["sourceformat"] = v => SearchStringAsync(_techInfoRepository.SourceFormats, x => x.Data, v),
                ["player"] = v => SearchStringAsync(_techInfoRepository.Players, x => x.Data, v),
                ["cartridge"] = v => SearchStringAsync(_techInfoRepository.Cartridges, x => x.Data, v),
                ["amp"] = v => SearchStringAsync(_techInfoRepository.Amplifiers, x => x.Data, v),
                ["adc"] = v => SearchStringAsync(_techInfoRepository.Adcs, x => x.Data, v),
                ["wire"] = v => SearchStringAsync(_techInfoRepository.Wires, x => x.Data, v),
                ["player_manufacturer"] = v => SearchStringAsync(_techInfoRepository.PlayerManufacturers, x => x.Data, v),
                ["cartridge_manufacturer"] = v => SearchStringAsync(_techInfoRepository.CartridgeManufacturers, x => x.Data, v),
                ["amp_manufacturer"] = v => SearchStringAsync(_techInfoRepository.AmplifierManufacturers, x => x.Data, v),
                ["adc_manufacturer"] = v => SearchStringAsync(_techInfoRepository.AdcManufacturers, x => x.Data, v),
                ["wire_manufacturer"] = v => SearchStringAsync(_techInfoRepository.WireManufacturers, x => x.Data, v),
            };
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1) 
                return BadRequest("Page number should be positive");

            var pagedAlbums = await _albumRepository.Albums
                .Include(x => x.Artist)
                .AsNoTracking()
                .ToPagedResultAsync(page, ALBUMS_PER_PAGE, x => x.Id);
            
            var albumViewModel = new AlbumViewModel
            {
                CurrentPage = page,
                PageCount = pagedAlbums.TotalPages,
                Albums = pagedAlbums.Items
            };

            return View("Index", albumViewModel);
        }
         
        [HttpGet("album/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id < 1)
                return BadRequest();
            
            var album = await _albumRepository.GetByIdAsync(id);
            
            if (album == null)
                return NotFound();

            var tinfo = await _techInfoRepository.GetByIdAsync(id);
            album.TechnicalInfo ??= tinfo;

            var model = new AlbumDetailsViewModel { Album = album };
            return View("Details", model);
        }

        [HttpGet("album/create")]
        public IActionResult Create()
        {
            return View("CreateOrUpdate", new AlbumCreateUpdateViewModel());
        }

        [HttpGet("album/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id < 1)
                return BadRequest("Invalid album Id");

            var album = await _albumRepository.GetByIdAsync(id);

            if (album == null)
                return NotFound();
            
            var tInfo = await _techInfoRepository.GetByIdAsync(id);
            var cover = _imageService.GetImageUrl(album.Id, EntityType.AlbumCover);
            var albumModel = MapToViewModel(album, tInfo);
            
            return View("CreateOrUpdate", albumModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AlbumCreateUpdateViewModel request)
        {
            if (!ModelState.IsValid)
                return View("CreateOrUpdate", request);

            try
            {
                var album = await _albumRepository.CreateOrUpdateAlbumAsync(request);
                await _techInfoRepository.CreateOrUpdateTechnicalInfoAsync(album, request);

                if (request.AlbumCover != null)
                    _imageService.SaveCover(album.Id, request.AlbumCover, EntityType.AlbumCover);

                return RedirectToAction("GetById", "Album", new { id = album.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to save album. " + ex.Message);
                return View("CreateOrUpdate", request);
            }
        }

        [HttpPost("album/update")]
        public async Task<IActionResult> Update(AlbumCreateUpdateViewModel request)
        {
            if (request.AlbumId < 1)
                return BadRequest("Invalid album ID");

            if (!ModelState.IsValid)
                return View("CreateOrUpdate", request);

            try
            {
                var album = await _albumRepository.CreateOrUpdateAlbumAsync(request);
                await _techInfoRepository.CreateOrUpdateTechnicalInfoAsync(album, request);

                if (request.AlbumCover is not null)
                {
                    _imageService.SaveCover(album.Id, request.AlbumCover, EntityType.AlbumCover);
                    DefaultHub.InvalidateAlbumCache(album.Id);
                }
                else
                {
                    _imageService.RemoveCover(album.Id, EntityType.AlbumCover);
                    DefaultHub.InvalidateAlbumCache(album.Id);
                }

                return RedirectToAction("GetById", "Album", new { id = request.AlbumId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to update album. " + ex.Message);
                return View("CreateOrUpdate", request);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (id < 1)
                return BadRequest("Invalid album ID");

            var album = await _albumRepository.Albums
                .Where(x => x.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

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
                // TODO: Add logging
                return BadRequest("Failed to delete album");
            }
            return Ok();
        }

        [HttpGet("search/{category?}")]
        public async Task<IActionResult> Search(string category, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Ok(Array.Empty<AutocompleteResponse>());

            return _searchMap.TryGetValue(category ?? "", out var searchFunc)
                ? await searchFunc(value)
                : Ok(Array.Empty<AutocompleteResponse>());
        }

        #region Private methods
        private AlbumCreateUpdateViewModel MapToViewModel(Album album, TechnicalInfo? tInfo)
        {
            return new AlbumCreateUpdateViewModel
            {
                Action = ActionType.Update,
                AlbumId = album.Id,
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
                AlbumCover = _imageService.GetImageUrl(album.Id, EntityType.AlbumCover),
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
        }

        private async Task<IActionResult> SearchStringAsync<TEntity>(IQueryable<TEntity> query, Expression<Func<TEntity, string>> selector,string value) 
            where TEntity : class
        {
            var likePattern = $"%{value}%";

            var result = await query
                .AsNoTracking()
                .Where(x => EF.Functions.Like(EF.Property<string>(x, ((MemberExpression)selector.Body).Member.Name), likePattern))
                .Select(x => new AutocompleteResponse { Label = EF.Property<string>(x, ((MemberExpression)selector.Body).Member.Name) })
                .ToListAsync();

            return Ok(result);
        }

        private async Task<IActionResult> SearchNumberAsync<TEntity, TProperty>(IQueryable<TEntity> query, Expression<Func<TEntity, TProperty>> selector, string value)
            where TEntity : class
        {
            var func = selector.Compile();

            var list = await query
                .AsNoTracking()
                .ToListAsync();

            var results = list
                .Where(x => func(x)?.ToString()?.Contains(value, StringComparison.OrdinalIgnoreCase) == true)
                .Select(x => new AutocompleteResponse { Label = func(x)?.ToString() ?? "" })
                .ToArray();

            return Ok(results);
        }
        #endregion
    }
}
