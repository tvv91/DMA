using Microsoft.AspNetCore.Mvc;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
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
        private readonly IDigitizationRepository _digitizationRepository;

        public AlbumController(IAlbumRepository albumRepository, IImageService imageService, IDigitizationRepository digitizationRepository)
        {
            _albumRepository = albumRepository;
            _imageService = imageService;
            _digitizationRepository = digitizationRepository;
        }

        [HttpGet("album")]
        public async Task<IActionResult> Index(int page = 1)
        {
            if (page < 1)
                return BadRequest("Page number should be positive");

            var result = await _albumRepository.GetIndexListAsync(page, ALBUMS_PER_PAGE);
            
            var vm = new AlbumIndexViewModel
            {
                CurrentPage = page,
                PageCount = result.TotalPages,
                Albums = result.Items
            };

            return View("Index", vm);
        }

        [HttpGet("album/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id < 1)
                return BadRequest();

            var album = await _albumRepository.GetByIdAsync(id);
            
            if (album == null)
                return NotFound();

            var digitizations = await _digitizationRepository.GetByAlbumIdAsync(album.Id);

            return View("Details", MapAlbumToAlbumDetailsVM(album, digitizations));
        }

        [HttpGet("album/create")]
        public IActionResult Create()
        {
            return View("CreateUpdate", new AlbumCreateUpdateViewModel());
        }

        [HttpGet("album/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id < 1)
                return BadRequest("Invalid album Id");

            var album = await _albumRepository.GetByIdAsync(id);

            if (album == null)
                return NotFound();

            return View("CreateUpdate", MapAlbumToAlbumDetailsVM(album));
        }

        [HttpPost]
        public async Task<IActionResult> Create(AlbumCreateUpdateViewModel request)
        {
            if (!ModelState.IsValid)
                return View("CreateUpdate", request);

            try
            {
                var album = await _albumRepository.FindByTitleAndArtistAsync(request.Title, request.Artist);

                if (album is null)
                {
                    album = new Album
                    {
                        AddedDate = DateTime.Now,
                        Title = request.Title,
                        Artist = new Artist { Name = request.Artist },
                        Genre = new Genre { Name = request.Genre }
                    };
                    
                    album = await _albumRepository.AddAsync(album);
                }

                await _digitizationRepository.AddAsync(MapVMToDigitization(album.Id, request));

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
                var album = await _albumRepository.UpdateAsync(new Album
                {
                    Id = request.AlbumId,
                    Title = request.Title,
                    Artist = new Artist { Name = request.Artist },
                    Genre = new Genre { Name = request.Genre }
                });

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

            if (!await _albumRepository.DeleteAsync(id))
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

        #region Private methods
        private AlbumDetailsViewModel MapAlbumToAlbumDetailsVM(Album album, IEnumerable<Digitization>? digitizations = null)
        {
            
            return new AlbumDetailsViewModel
            {
                AlbumId = album.Id,
                Title = album.Title,
                Artist = album.Artist?.Name ?? string.Empty,
                Genre = album.Genre?.Name ?? string.Empty,
                AddedDate = album.AddedDate,
                UpdateDate = album.UpdateDate,
                Digitizations = digitizations
            };
        }
        private Digitization MapVMToDigitization(int albumId, AlbumCreateUpdateViewModel request)
        {
            return new Digitization
            {
                AlbumId = albumId,
                AddedDate = DateTime.Now,
                Source = request.Source,
                Discogs = request.Discogs,
                IsFirstPress = false,
                YearId = request.Year,
                ReissueId = request.Reissue,
                Country = !string.IsNullOrEmpty(request.Country) ? new Country { Name = request.Country } : null,
                Label = !string.IsNullOrEmpty(request.Label) ? new Label { Name = request.Label } : null,
                Storage = !string.IsNullOrEmpty(request.Storage) ? new Storage { Data = request.Storage } : null,

                FormatInfo = new FormatInfo
                {
                    Size = request.Size,
                    BitnessId = request.Bitness,
                    Sampling = request.Sampling.HasValue ? new Sampling { Value = request.Sampling.Value } : null,
                    DigitalFormat = !string.IsNullOrEmpty(request.DigitalFormat) ? new DigitalFormat { Name = request.DigitalFormat } : null,
                    SourceFormat = !string.IsNullOrEmpty(request.SourceFormat) ? new SourceFormat { Name = request.SourceFormat } : null,
                    VinylState = !string.IsNullOrEmpty(request.VinylState) ? new VinylState { Name = request.VinylState } : null
                },

                EquipmentInfo = new EquipmentInfo
                {
                    Player = !string.IsNullOrEmpty(request.Player) ? new Player { Name = request.Player } : null,
                    Cartridge = !string.IsNullOrEmpty(request.Cartridge) ? new Cartridge { Name = request.Cartridge } : null,
                    Amplifier = !string.IsNullOrEmpty(request.Amplifier) ? new Amplifier { Name = request.Amplifier } : null,
                    Adc = !string.IsNullOrEmpty(request.Adc) ? new Adc { Name = request.Adc } : null,
                    Wire = !string.IsNullOrEmpty(request.Wire) ? new Wire { Name = request.Wire } : null
                }
            };
        }
        #endregion
    }
}
