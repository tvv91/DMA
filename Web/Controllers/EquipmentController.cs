using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Enums;
using Web.Request;
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

        [HttpPost("[controller]/update")]
        public async Task<IActionResult> Update(EquipmentDataRequest request)
        {
            if (request.EquipmentId <= 0 || request.Action != ActionType.Update)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var entityId = await _repository.CreateOrUpdateEquipmentAsync(request);

                if (request.EquipmentCover == null)
                {
                    _imageService.RemoveCover(entityId, request.EntityType);
                }
                else
                {
                    _imageService.SaveCover(entityId, request.EquipmentCover, request.EntityType);
                }

                return new RedirectResult($"/equipment/{request.EntityType.ToString().ToLower()}/{entityId}");
            }
            return View("CreateOrUpdate", request);
        }

        [HttpGet("[controller]/{category}/{id}/edit")]
        public async Task<IActionResult> Edit(EntityType? category, int id)
        {
            if (id > 0 && category != null)
            {
                var img = _imageService.GetImageUrl(id, category.Value);
                switch (category)
                {
                    case EntityType.Adc:
                        var adc = await _repository.Adcs.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (adc != null)
                        {
                            var equipment = new EquipmentDataRequest
                            {
                                Action = ActionType.Update,
                                EntityType = EntityType.Adc,
                                EquipmentCover = img,
                                Description = adc?.Description,
                                Manufacturer = adc?.Manufacturer?.Data,
                                Model = adc?.Data,
                                EquipmentId = adc.Id
                            };
                            return View("CreateOrUpdate", equipment);
                        }
                        else return NotFound();
                    case EntityType.Amplifier:
                        var amp = await _repository.Amplifiers.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (amp != null)
                        {
                            var equipment = new EquipmentDataRequest
                            {
                                Action = ActionType.Update,
                                EntityType = EntityType.Amplifier,
                                EquipmentCover = img,
                                Description = amp?.Description,
                                Manufacturer = amp?.Manufacturer?.Data,
                                Model = amp?.Data,
                                EquipmentId = amp.Id
                            };
                            return View("CreateOrUpdate", equipment);
                        }
                        else return NotFound();
                    case EntityType.Player:
                        var player = await _repository.Players.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (player != null)
                        {
                            var equipment = new EquipmentDataRequest
                            {
                                Action = ActionType.Update,
                                EntityType = EntityType.Player,
                                EquipmentCover = img,
                                Description = player?.Description,
                                Manufacturer = player?.Manufacturer?.Data,
                                Model = player?.Data,
                                EquipmentId = player.Id
                            };
                            return View("CreateOrUpdate", equipment);
                        }
                        else return NotFound();
                    case EntityType.Cartridge:
                        var cartridge = await _repository.Cartridges.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (cartridge != null)
                        {
                            var equipment = new EquipmentDataRequest
                            {
                                Action = ActionType.Update,
                                EntityType = EntityType.Cartridge,
                                EquipmentCover = img,
                                Description = cartridge?.Description,
                                Manufacturer = cartridge?.Manufacturer?.Data,
                                Model = cartridge?.Data,
                                EquipmentId = cartridge.Id
                            };
                            return View("CreateOrUpdate", equipment);
                        }
                        else return NotFound();
                    case EntityType.Wire:
                        var wire = await _repository.Wires.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (wire != null)
                        {
                            var equipment = new EquipmentDataRequest
                            {
                                Action = ActionType.Update,
                                EntityType = EntityType.Wire,
                                EquipmentCover = img,
                                Description = wire?.Description,
                                Manufacturer = wire?.Manufacturer?.Data,
                                Model = wire?.Data,
                                EquipmentId = wire.Id
                            };
                            return View("CreateOrUpdate", equipment);
                        }
                        else return NotFound();
                    default: throw new ArgumentException("Unsupported Entity type");
                }
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
                            return View("EquipmentDetails", new EquipmentViewModel
                            {
                                Id = adc.Id,
                                EntityType = EntityType.Adc,
                                Model = adc.Data,
                                Description = adc.Description,
                                Manufacturer = adc?.Manufacturer?.Data,

                            });
                        }
                        else return NotFound();
                    case EntityType.Amplifier:
                        var amp = await _repository.Amplifiers.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (amp != null)
                        {
                            return View("EquipmentDetails", new EquipmentViewModel
                            {
                                Id = amp.Id,
                                EntityType = EntityType.Amplifier,
                                Model = amp.Data,
                                Description = amp.Description,
                                Manufacturer = amp?.Manufacturer?.Data,
                            });
                        }
                        else return NotFound();
                    case EntityType.Cartridge:
                        var cartridge = await _repository.Cartridges.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (cartridge != null)
                        {
                            return View("EquipmentDetails", new EquipmentViewModel
                            {
                                Id = cartridge.Id,
                                EntityType = EntityType.Cartridge,
                                Model = cartridge.Data,
                                Description = cartridge.Description,
                                Manufacturer = cartridge?.Manufacturer?.Data,
                            });
                        }
                        else return NotFound();
                    case EntityType.Player:
                        var player = await _repository.Cartridges.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (player != null)
                        {
                            return View("EquipmentDetails", new EquipmentViewModel
                            {
                                Id = player.Id,
                                EntityType = EntityType.Player,
                                Model = player.Data,
                                Description = player.Description,
                                Manufacturer = player?.Manufacturer?.Data,
                            });
                        }
                        else return NotFound();
                    case EntityType.Wire:
                        var wire = await _repository.Cartridges.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id);
                        if (wire != null)
                        {
                            return View("EquipmentDetails", new EquipmentViewModel
                            {
                                Id = wire.Id,
                                EntityType = EntityType.Player,
                                Model = wire.Data,
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
        public IActionResult Creaate()
        {
            return View("CreateOrUpdate", new EquipmentDataRequest());
        }
        
        [HttpPost]
        public async Task<IActionResult> NewEquipment(EquipmentDataRequest request)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(request.Model) && request?.EntityType != null)
            {
                var equipmentId = await _repository.CreateOrUpdateEquipmentAsync(request);

                if (request.EquipmentCover != null)
                {
                    _imageService.SaveCover(equipmentId, request.EquipmentCover, request.EntityType);
                }

                return new RedirectResult($"{request.EntityType.ToString().ToLower()}/{equipmentId}");
            }
            else
            {
                return View("CreateOrUpdate", request);
            }
        }
    }
}
