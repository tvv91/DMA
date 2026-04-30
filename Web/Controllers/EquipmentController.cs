using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Common;
using Web.Enums;
using Web.Models;
using Web.Interfaces;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class EquipmentController(
        IEquipmentService equipmentService,
        IImageService imageService,
        IDigitizationService digitizationService) : Controller
    {
        private readonly IEquipmentService _equipmentService = equipmentService;
        private readonly IImageService _imageService = imageService;
        private readonly IDigitizationService _digitizationService = digitizationService;

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Category (string category)
        {
            return Ok();
        }

        [HttpDelete("[controller]/{category}/delete/")]
        public async Task<IActionResult> Delete(EntityType category, int id)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                var deleted = await _equipmentService.DeleteEquipmentAsync(id, category);
                await _imageService.RemoveAsync(id, category);
                return deleted ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                // TODO: Add logging
                throw new InvalidOperationException("Error during deleting equipment", ex);
            }
        }

        [HttpPost("[controller]/update")]
        public async Task<IActionResult> Update(EquipmentViewModel request)
        {
            if (request.Id <= 0)
                return BadRequest("Invalid equipment ID");

            if (request.Action != ActionType.Update)
                return BadRequest("Invalid action type");

            if (!ModelState.IsValid)
                return View("CreateUpdate", request);

            try
            {
                var updated = await _equipmentService.UpdateEquipmentAsync(request);

                if (request.EquipmentCover is null)
                    await _imageService.RemoveAsync(updated.Id, request.EquipmentType);
                else
                    await _imageService.SaveAsync(updated.Id, request.EquipmentCover, request.EquipmentType);

                return Redirect($"/equipment/{request.EquipmentType}/{updated.Id}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to update equipment: {ex.Message}");
                return View("CreateUpdate", request);
            }
        }


        [HttpGet("equipment/{category}/{id}/edit", Order = 1)]
        public async Task<IActionResult> Edit(EntityType category, int id)
        {
            if (id <= 0)
                return BadRequest();
            
            var equipment = await _equipmentService.GetByIdAsync(id, category);
            
            if (equipment is null)
                return NotFound();

            var imageUrl = await _imageService.GetUrlAsync(id, category);
            var vm = _equipmentService.MapEquipmentToViewModel(equipment, category, imageUrl);
            vm.Action = ActionType.Update;

            return View("CreateUpdate", vm);
        }

        private const int DefaultEquipmentAlbumsPageSize = 18;
        private const int MaxEquipmentAlbumsPageSize = 100;

        /// <summary>
        /// HTML partial for client-side paging on the equipment details "Albums" tab (fetch).
        /// </summary>
        [HttpGet("equipment/{category}/{id}/albums-data", Order = 0)]
        public async Task<IActionResult> EquipmentAlbumsData(EntityType category, int id, int page = 1, int pageSize = DefaultEquipmentAlbumsPageSize)
        {
            if (id <= 0)
                return BadRequest();

            var equipment = await _equipmentService.GetByIdAsync(id, category);
            if (equipment is null)
                return NotFound();

            if (pageSize <= 0)
                pageSize = DefaultEquipmentAlbumsPageSize;
            else if (pageSize > MaxEquipmentAlbumsPageSize)
                pageSize = MaxEquipmentAlbumsPageSize;

            if (page < 1)
                page = 1;

            var result = await _digitizationService.GetAlbumsDigitizedByEquipmentPagedAsync(category, id, page, pageSize);
            var vm = MapDigitizedAlbumsPage(category, id, result);
            return PartialView("_EquipmentDigitizedAlbumsInner", vm);
        }

        [HttpGet("equipment/{category}/{id}", Order = 2)]
        public async Task<IActionResult> GetById(EntityType category, int id, string? tab = null, int page = 1, int pageSize = 18)
        {
            if (id <= 0)
                return BadRequest();

            var equipment = await _equipmentService.GetByIdAsync(id, category);

            if (equipment is null)
                return NotFound();

            var imageUrl = await _imageService.GetUrlAsync(id, category);
            var vm = _equipmentService.MapEquipmentToViewModel(equipment, category, imageUrl);

            if (string.Equals(tab, "albums", StringComparison.OrdinalIgnoreCase))
            {
                vm.ActiveTab = "albums";
                if (pageSize <= 0) pageSize = DefaultEquipmentAlbumsPageSize;
                if (pageSize > MaxEquipmentAlbumsPageSize) pageSize = MaxEquipmentAlbumsPageSize;
                if (page < 1) page = 1;
                var albumsPage = await _digitizationService.GetAlbumsDigitizedByEquipmentPagedAsync(category, id, page, pageSize);
                vm.DigitizedAlbumsPage = MapDigitizedAlbumsPage(category, id, albumsPage);
            }

            return View("Details", vm);
        }

        
        [HttpGet("equipment/create")]
        public IActionResult Create()
        {
            return View("CreateUpdate", new EquipmentViewModel { Action = ActionType.Create, EquipmentType = EntityType.Adc });
        }

        [HttpPost]
        public async Task<IActionResult> Create(EquipmentViewModel request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.ModelName))
                return View("CreateUpdate", request);

            // Validate that EquipmentType is a valid equipment type
            var validEquipmentTypes = new[] { EntityType.Adc, EntityType.Amplifier, EntityType.Cartridge, EntityType.Player, EntityType.Wire };
            if (!validEquipmentTypes.Contains(request.EquipmentType))
            {
                ModelState.AddModelError(nameof(request.EquipmentType), "Invalid equipment type selected.");
                return View("CreateUpdate", request);
            }

            var equipment = await _equipmentService.CreateEquipmentAsync(request);

            if (request.EquipmentCover is not null)
                await _imageService.SaveAsync(equipment.Id, request.EquipmentCover, request.EquipmentType);

            return Redirect($"/equipment/{request.EquipmentType}/{equipment.Id}");
        }

        private EquipmentDigitizedAlbumsPageViewModel MapDigitizedAlbumsPage(EntityType category, int equipmentId, PagedResult<Album> result)
        {
            var catSeg = category.ToString().ToLowerInvariant();
            return new EquipmentDigitizedAlbumsPageViewModel
            {
                CurrentPage = result.CurrentPage,
                PageCount = result.TotalPages,
                PageSize = result.PageSize,
                HasResults = result.Items.Count > 0,
                CategorySegment = catSeg,
                EquipmentId = equipmentId,
                Albums = [.. result.Items.Select(a => new EquipmentAlbumRowViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    ArtistName = a.Artist?.Name ?? string.Empty,
                    DetailUrl = Url.Action("GetById", "Album", new { id = a.Id }) ?? string.Empty
                })]
            };
        }
    }
}
