using Microsoft.AspNetCore.Mvc;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class EquipmentController : Controller
    {
        //private readonly IDigitizationRepository _repository;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IImageService _imageService;

        public EquipmentController(IEquipmentRepository equipmentRepository, IImageService imageService)
        {
            _equipmentRepository = equipmentRepository;
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
                var deleted = await _equipmentRepository.DeleteAsync(id, category);
                _imageService.RemoveCover(id, category);
                return deleted ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                // TODO: Add logging
                throw new InvalidOperationException("Error during deleting equipment", ex);
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(EquipmentViewModel request)
        {
            if (request.Id <= 0 || request.Action != ActionType.Update)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("CreateUpdate", request);

            var updated = await _equipmentRepository.UpdateAsync(GetEquipmentFromVM(request), request.EquipmentType);

            if (request.EquipmentCover is null)
                _imageService.RemoveCover(updated.Id, request.EquipmentType);
            else
                _imageService.SaveCover(updated.Id, request.EquipmentCover, request.EquipmentType);

            return RedirectToAction("GetById", "Equipment", new { category = request.EquipmentType, id = updated.Id });
        }


        [HttpGet("{category}/{id}/edit")]
        public async Task<IActionResult> Edit(EntityType category, int id)
        {
            if (id <= 0)
                return BadRequest();
            
            var equipment = await _equipmentRepository.GetByIdAsync(id, category);
            
            if (equipment == null)
                return NotFound();

            var vm = new EquipmentViewModel
            {
                Id = equipment.Id,
                ModelName = equipment.Name,
                Description = equipment.Description,
                EquipmentType = category,
                Action = ActionType.Update,
                EquipmentCover = _imageService.GetImageUrl(id, category),
                Manufacturer = equipment.Manufacturer?.Name,
            };

            return View("CreateUpdate", vm);
        }

        [HttpGet("[controller]/{category}/{id}")]
        public async Task<IActionResult> GetById(EntityType category, int id)
        {
            if (id <= 0)
                return BadRequest();

            var equipment = await _equipmentRepository.GetByIdAsync(id, category);

            if (equipment == null)
                return NotFound();

            var vm = new EquipmentViewModel
            {
                Id = equipment.Id,
                ModelName = equipment.Name,
                Description = equipment.Description,
                EquipmentType = category,
                EquipmentCover = _imageService.GetImageUrl(id, category),
                Manufacturer = equipment.Manufacturer?.Name,
            };

            return View("Details", vm);
        }

        
        [HttpGet("equipment/create")]
        public IActionResult Create()
        {
            return View("CreateUpdate", new EquipmentViewModel { Action = ActionType.Create, /*EquipmentType = EntityType.Adc*/ });
        }

        [HttpPost]
        public async Task<IActionResult> Create(EquipmentViewModel request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.ModelName))
                return View("CreateUpdate", request);

            var equipment = await _equipmentRepository.AddAsync(GetEquipmentFromVM(request), request.EquipmentType);

            if (request.EquipmentCover is not null)
                _imageService.SaveCover(equipment.Id, request.EquipmentCover, request.EquipmentType);

            return RedirectToAction("GetById", new { category = request.EquipmentType, id = equipment.Id });
        }

        #region Private
        private IManufacturer GetEquipmentFromVM(EquipmentViewModel request)
        {
            Manufacturer? CreateManufacturer(string? name, EntityType type) => string.IsNullOrWhiteSpace(name) ? null : new Manufacturer { Name = name, Type = type };

            return request.EquipmentType switch
            {
                EntityType.Adc => new Adc
                {
                    Id = request.Id,
                    Name = request.ModelName,
                    Description = request.Description,
                    Manufacturer = CreateManufacturer(request.Manufacturer, EntityType.Adc)
                },
                EntityType.Amplifier => new Amplifier
                {
                    Id = request.Id,
                    Name = request.ModelName,
                    Description = request.Description,
                    Manufacturer = CreateManufacturer(request.Manufacturer, EntityType.Amplifier)
                },
                EntityType.Cartridge => new Cartridge
                {
                    Id = request.Id,
                    Name = request.ModelName,
                    Description = request.Description,
                    Manufacturer = CreateManufacturer(request.Manufacturer, EntityType.Cartridge)
                },
                EntityType.Player => new Player
                {
                    Id = request.Id,
                    Name = request.ModelName,
                    Description = request.Description,
                    Manufacturer = CreateManufacturer(request.Manufacturer, EntityType.Player)
                },
                EntityType.Wire => new Wire
                {
                    Id = request.Id,
                    Name = request.ModelName,
                    Description = request.Description,
                    Manufacturer = CreateManufacturer(request.Manufacturer, EntityType.Wire)
                },
                _ => throw new ArgumentOutOfRangeException(nameof(request.EquipmentType), request.EquipmentType, "Unknown equipment type")
            };
        }
        #endregion
    }
}
