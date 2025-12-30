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
        private readonly IEquipmentService _equipmentService;
        private readonly IImageService _imageService;

        public EquipmentController(IEquipmentService equipmentService, IImageService imageService)
        {
            _equipmentService = equipmentService;
            _imageService = imageService;
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
                _imageService.RemoveCover(id, category);
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
            if (request.Id <= 0 || request.Action != ActionType.Update)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("CreateUpdate", request);

            var updated = await _equipmentService.UpdateEquipmentAsync(request);

            if (request.EquipmentCover is null)
                _imageService.RemoveCover(updated.Id, request.EquipmentType);
            else
                _imageService.SaveCover(updated.Id, request.EquipmentCover, request.EquipmentType);

            return Redirect($"/equipment/{request.EquipmentType}/{updated.Id}");
        }


        [HttpGet("equipment/{category}/{id}/edit", Order = 1)]
        public async Task<IActionResult> Edit(EntityType category, int id)
        {
            if (id <= 0)
                return BadRequest();
            
            var equipment = await _equipmentService.GetByIdAsync(id, category);
            
            if (equipment == null)
                return NotFound();

            var imageUrl = _imageService.GetImageUrl(id, category);
            var vm = _equipmentService.MapEquipmentToViewModel(equipment, category, imageUrl);
            vm.Action = ActionType.Update;

            return View("CreateUpdate", vm);
        }

        [HttpGet("equipment/{category}/{id}", Order = 2)]
        public async Task<IActionResult> GetById(EntityType category, int id)
        {
            if (id <= 0)
                return BadRequest();

            var equipment = await _equipmentService.GetByIdAsync(id, category);

            if (equipment == null)
                return NotFound();

            var imageUrl = _imageService.GetImageUrl(id, category);
            var vm = _equipmentService.MapEquipmentToViewModel(equipment, category, imageUrl);

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
                _imageService.SaveCover(equipment.Id, request.EquipmentCover, request.EquipmentType);

            return Redirect($"/equipment/{request.EquipmentType}/{equipment.Id}");
        }
    }
}
