using Microsoft.AspNetCore.Mvc;
using Web.Db;
using Web.Enums;
using Web.Services;
using Web.ViewModels;

namespace Web.Controllers
{
    public class EquipmentController : Controller
    {
        private readonly ITechInfoRepository _repository;
        private readonly IImageService _imageService;

        public EquipmentController(ITechInfoRepository repository, IImageService imageService)
        {
            _repository = repository;
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
                var deleted = await _repository.DeleteEquipmentAsync(id, category);
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
