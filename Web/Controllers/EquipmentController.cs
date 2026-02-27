using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Web.Enums;
using Web.Interfaces;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class EquipmentController : Controller
    {
        private const int DefaultAlbumsPerPage = 15;
        private const int MaxAlbumsPerPage = 30;
        private readonly IEquipmentService _equipmentService;
        private readonly IImageService _imageService;
        private readonly IAlbumService _albumService;

        public EquipmentController(IEquipmentService equipmentService, IImageService imageService, IAlbumService albumService)
        {
            _equipmentService = equipmentService;
            _imageService = imageService;
            _albumService = albumService;
        }

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
                await _imageService.RemoveCoverAsync(id, category);
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
                    await _imageService.RemoveCoverAsync(updated.Id, request.EquipmentType);
                else
                    await _imageService.SaveCoverAsync(updated.Id, request.EquipmentCover, request.EquipmentType);

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
            
            if (equipment == null)
                return NotFound();

            var imageUrl = await _imageService.GetImageUrlAsync(id, category);
            var vm = _equipmentService.MapEquipmentToViewModel(equipment, category, imageUrl);
            vm.Action = ActionType.Update;

            return View("CreateUpdate", vm);
        }

        [HttpGet("equipment/{category}/{id}", Order = 2)]
        public async Task<IActionResult> GetById(EntityType category, int id, int page = 1, int pageSize = 0)
        {
            if (id <= 0)
                return BadRequest();

            var equipment = await _equipmentService.GetByIdAsync(id, category);

            if (equipment == null)
                return NotFound();

            var imageUrl = await _imageService.GetImageUrlAsync(id, category);
            var vm = _equipmentService.MapEquipmentToViewModel(equipment, category, imageUrl);

            if (page < 1) page = 1;
            if (pageSize <= 0)
                pageSize = DefaultAlbumsPerPage;
            else if (pageSize > MaxAlbumsPerPage)
                pageSize = MaxAlbumsPerPage;

            var albumsResult = await _albumService.GetAlbumsByEquipmentAsync(category, id, page, pageSize);
            vm.AlbumsUsedIn = albumsResult;

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
                await _imageService.SaveCoverAsync(equipment.Id, request.EquipmentCover, request.EquipmentType);

            return Redirect($"/equipment/{request.EquipmentType}/{equipment.Id}");
        }
    }
}
