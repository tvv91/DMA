using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost("[controller]/update")]
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


        [HttpGet("[controller]/{category}/{id}/edit")]
        public async Task<IActionResult> Edit(EntityType category, int id)
        {
            if (id > 0)
            {
                var equipment = await _repository.GetEquipmentByIdAsync(category, id);
                if (equipment == null)
                    return NotFound();

                equipment.EquipmentCover = _imageService.GetImageUrl(id, category);
                return View("CreateUpdate", equipment);
            }
            return BadRequest();
        }
        
        [HttpGet("[controller]/{category}/{id}")]
        public async Task<IActionResult> GetById(EntityType? category, int id)
        {
            if (id > 0 && category != null)
            {
                switch (category)
                {
                    case EntityType.Adc:
                        var adc = await _repository.Adcs.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (adc != null)
                        {
                            var cover = _imageService.GetImageUrl(adc.Id, EntityType.Adc);
                            return View("Details", new EquipmentViewModel
                            {
                                Id = adc.Id,
                                EquipmentType = EntityType.Adc,
                                ModelName = adc.Data,
                                Description = adc.Description,
                                Manufacturer = adc?.Manufacturer?.Data,

                            });
                        }
                        else return NotFound();
                    case EntityType.Amplifier:
                        var amp = await _repository.Amplifiers.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (amp != null)
                        {
                            return View("Details", new EquipmentViewModel
                            {
                                Id = amp.Id,
                                EquipmentType = EntityType.Amplifier,
                                ModelName = amp.Data,
                                Description = amp.Description,
                                Manufacturer = amp?.Manufacturer?.Data,
                            });
                        }
                        else return NotFound();
                    case EntityType.Cartridge:
                        var cartridge = await _repository.Cartridges.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (cartridge != null)
                        {
                            return View("Details", new EquipmentViewModel
                            {
                                Id = cartridge.Id,
                                EquipmentType = EntityType.Cartridge,
                                ModelName = cartridge.Data,
                                Description = cartridge.Description,
                                Manufacturer = cartridge?.Manufacturer?.Data,
                            });
                        }
                        else return NotFound();
                    case EntityType.Player:
                        var player = await _repository.Players.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (player != null)
                        {
                            return View("Details", new EquipmentViewModel
                            {
                                Id = player.Id,
                                EquipmentType = EntityType.Player,
                                ModelName = player.Data,
                                Description = player.Description,
                                Manufacturer = player?.Manufacturer?.Data,
                            });
                        }
                        else return NotFound();
                    case EntityType.Wire:
                        var wire = await _repository.Wires.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (wire != null)
                        {
                            return View("Details", new EquipmentViewModel
                            {
                                Id = wire.Id,
                                EquipmentType = EntityType.Wire,
                                ModelName = wire.Data,
                                Description = wire.Description,
                                Manufacturer = wire?.Manufacturer?.Data,
                            });
                        }
                        else return NotFound();
                }
            }
            return BadRequest();
        }        

        [HttpGet("equipment/create")]
        public IActionResult Create()
        {
            return View("CreateUpdate", new EquipmentViewModel { Action = ActionType.Create, EquipmentType = EntityType.Adc });
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(EquipmentViewModel request)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(request.ModelName) && request?.EquipmentType != null)
            {
                var equipmentId = await _repository.CreateOrUpdateEquipmentAsync(request);

                if (request.EquipmentCover != null)
                {
                    _imageService.SaveCover(equipmentId, request.EquipmentCover, request.EquipmentType);
                }

                return new RedirectResult($"{request.EquipmentType.ToString().ToLower()}/{equipmentId}");
            }
            else
            {
                return View("CreateUpdate", request);
            }
        }
    }
}
