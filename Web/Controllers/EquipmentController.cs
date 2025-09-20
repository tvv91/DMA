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
        public async Task<IActionResult> Delete(EntityType? category, int id)
        {
            if (id > 0 && category != null)
            {
                switch (category)
                {
                    case EntityType.Adc:
                        if (await _repository.Adcs.AnyAsync(a => a.Id == id))
                        {
                            try
                            {
                                _imageService.RemoveCover(id, EntityType.Adc);
                                // TODO: Add loging what TInfo was updated
                                // TODO: Maybe need some precheck and notification that this equipment used in multiple albums to avoid random removing?
                                await _repository.TechInfos.Where(t => t.AdcId == id).ExecuteUpdateAsync(x => x.SetProperty(p => p.AdcId, p => null));
                                await _repository.Adcs.Where(x => x.Id == id).ExecuteDeleteAsync();
                                return Ok();
                            }
                            catch (Exception ex)
                            {
                                // TODO: Add logging
                                throw new InvalidOperationException("Error during deleting Adc");
                            }
                            
                        }
                        else return NotFound(); 
                    case EntityType.Amplifier:
                        if (await _repository.Amplifiers.AnyAsync(a => a.Id == id))
                        {
                            try
                            {
                                _imageService.RemoveCover(id, EntityType.Amplifier);
                                // TODO: Add loging what TInfo was updated
                                // TODO: Maybe need some precheck and notification that this equipment used in multiple albums to avoid random removing?
                                await _repository.TechInfos.Where(t => t.AmplifierId == id).ExecuteUpdateAsync(x => x.SetProperty(x => x.AmplifierId, x => null));
                                await _repository.Amplifiers.Where(x => x.Id == id).ExecuteDeleteAsync();
                                return Ok();
                            }
                            catch (Exception ex)
                            {
                                // TODO: Add logging
                                throw new InvalidOperationException("Error during deleting Amp");
                            }
                        }
                        else return NotFound();
                    case EntityType.Cartridge:
                        if (await _repository.Cartridges.AnyAsync(a => a.Id == id))
                        {
                            try
                            {
                                _imageService.RemoveCover(id, EntityType.Cartridge);
                                // TODO: Add loging what TInfo was updated
                                // TODO: Maybe need some precheck and notification that this equipment used in multiple albums to avoid random removing?
                                await _repository.TechInfos.Where(t => t.CartridgeId == id).ExecuteUpdateAsync(x => x.SetProperty(p => p.CartridgeId, p => null));
                                await _repository.Cartridges.Where(x => x.Id == id).ExecuteDeleteAsync();
                                return Ok();
                            }
                            catch (Exception ex)
                            {
                                // TODO: Add logging
                                throw new InvalidOperationException("Error during deleting Cartridge");
                            }
                        }
                        else return NotFound();
                    case EntityType.Player:
                        if (await _repository.Players.AnyAsync(a => a.Id == id))
                        {
                            try
                            {
                                _imageService.RemoveCover(id, EntityType.Player);
                                // TODO: Add loging what TInfo was updated
                                // TODO: Maybe need some precheck and notification that this equipment used in multiple albums to avoid random removing?
                                await _repository.TechInfos.Where(t => t.PlayerId == id).ExecuteUpdateAsync(x => x.SetProperty(p => p.PlayerId, p => null));
                                await _repository.Players.Where(adc => adc.Id == id).ExecuteDeleteAsync();
                                return Ok();
                            }
                            catch (Exception ex)
                            {
                                // TODO: Add logging
                                throw new InvalidOperationException("Error during deleting Player");
                            }
                        }
                        else return NotFound();
                    case EntityType.Wire:
                        if (await _repository.Wires.AnyAsync(a => a.Id == id))
                        {
                            try
                            {
                                _imageService.RemoveCover(id, EntityType.Wire);
                                // TODO: Maybe need some precheck and notification that this equipment used in multiple albums to avoid random removing?
                                await _repository.TechInfos.Where(t => t.PlayerId == id).ExecuteUpdateAsync(x => x.SetProperty(p => p.PlayerId, p => null));
                                await _repository.Wires.Where(adc => adc.Id == id).ExecuteDeleteAsync();
                                return Ok();
                            }
                            catch (Exception ex)
                            {
                                // TODO: Add logging
                                throw new InvalidOperationException("Error during deleting Wire");
                            }
                        }
                        else return NotFound();
                    default: throw new InvalidOperationException("Invalid EntityType");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("[controller]/update")]
        public async Task<IActionResult> Update(EquipmentViewModel request)
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
                    _imageService.RemoveCover(entityId, request.EquipmentType);
                }
                else
                {
                    _imageService.SaveCover(entityId, request.EquipmentCover, request.EquipmentType);
                }

                return new RedirectResult($"/equipment/{request.EquipmentType.ToString().ToLower()}/{entityId}");
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
                            var equipment = new EquipmentViewModel
                            {
                                Action = ActionType.Update,
                                EquipmentType = EntityType.Adc,
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
                            var equipment = new EquipmentViewModel
                            {
                                Action = ActionType.Update,
                                EquipmentType = EntityType.Amplifier,
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
                            var equipment = new EquipmentViewModel
                            {
                                Action = ActionType.Update,
                                EquipmentType = EntityType.Player,
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
                            var equipment = new EquipmentViewModel
                            {
                                Action = ActionType.Update,
                                EquipmentType = EntityType.Cartridge,
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
                            var equipment = new EquipmentViewModel
                            {
                                Action = ActionType.Update,
                                EquipmentType = EntityType.Wire,
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
                            return View("Details", new EquipmentViewModel
                            {
                                Id = adc.Id,
                                EquipmentType = EntityType.Adc,
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
                            return View("Details", new EquipmentViewModel
                            {
                                Id = amp.Id,
                                EquipmentType = EntityType.Amplifier,
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
                            return View("Details", new EquipmentViewModel
                            {
                                Id = cartridge.Id,
                                EquipmentType = EntityType.Cartridge,
                                Model = cartridge.Data,
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
                                Model = player.Data,
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
        public IActionResult Create()
        {
            return View("CreateOrUpdate", new EquipmentViewModel { Action = ActionType.Create, EquipmentType = EntityType.Adc });
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(EquipmentViewModel request)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(request.Model) && request?.EquipmentType != null)
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
                return View("CreateOrUpdate", request);
            }
        }
    }
}
