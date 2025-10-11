using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly IDigitizationRepository _repository;
        private readonly IRepository<Adc> _adcRepo;
        private readonly IRepository<Amplifier> _amplifierRepo;
        private readonly IRepository<Cartridge> _cartridgeRepo;
        private readonly IRepository<Player> _playerRepo;
        private readonly IRepository<Wire> _wireRepo;
        
        private readonly IImageService _imageService;

        public EquipmentController(
            IDigitizationRepository digitizationRepo, 
            IImageService imageService, 
            IRepository<Adc> adcRepo,
            IRepository<Amplifier > amplifierRepo,
            IRepository<Cartridge > cartridgeRepo,
            IRepository<Player> playerRepo, 
            IRepository<Wire> wireRepo)
        {
            _repository = digitizationRepo;
            _imageService = imageService;
            _adcRepo = adcRepo;
            _amplifierRepo = amplifierRepo;
            _cartridgeRepo = cartridgeRepo;
            _playerRepo = playerRepo;
            _wireRepo = wireRepo;
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
                var deleted = await _repository.DeleteAsync(id, category);
                _imageService.RemoveCover(id, category);
                return deleted ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error during deleting equipment", ex);
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(EquipmentViewModel request)
        {
            if (request.EquipmentId <= 0 || request.Action != ActionType.Update)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("CreateUpdate", request);

            var entityId = await _repository.CreateOrUpdateEquipmentAsync(request);

            if (request.EquipmentCover is null)
                _imageService.RemoveCover(entityId, request.EquipmentType);
            else
                 _imageService.SaveCover(entityId, request.EquipmentCover, request.EquipmentType);

            return RedirectToAction("GetById", "Equipment", new {category = request.EquipmentType, id = entityId });
        }


        [HttpGet("{category}/{id}/edit")]
        public async Task<IActionResult> Edit(EntityType category, int id)
        {
            if (id <= 0)
                return BadRequest();
            
            var equipment = await _repository.GetEquipmentByIdAsync(category, id);
            
            if (equipment == null)
                return NotFound();

            equipment.EquipmentCover = _imageService.GetImageUrl(id, category);
            return View("CreateUpdate", equipment);
        }

        [HttpGet("[controller]/{category}/{id}")]
        public async Task<IActionResult> GetById(EntityType category, int id)
        {
            if (id <= 0)
                return BadRequest();

            var equipment = await _repository.GetEquipmentByIdAsync(category, id);

            if (equipment == null)
                return NotFound();

            return View("Details", equipment);
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

            var equipmentId = await _repository.CreateOrUpdateEquipmentAsync(request);

            if (request.EquipmentCover is not null)
                _imageService.SaveCover(equipmentId, request.EquipmentCover, request.EquipmentType);

            return RedirectToAction("GetById", new { category = request.EquipmentType, id = equipmentId });
        }
    }
}
