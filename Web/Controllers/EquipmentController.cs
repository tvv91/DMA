using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Web.Common;
using Web.Enums;
using Web.Authorization;
using Web.Features.Equipment;
using Web.ViewModels;

namespace Web.Controllers
{
    public class EquipmentController(ISender sender) : Controller
    {
        private const int DefaultEquipmentAlbumsPageSize = 18;
        private readonly ISender _sender = sender;

        public async Task<IActionResult> Index()
        {
            await _sender.Send(new EquipmentPageQuery());
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Category(string category)
        {
            await _sender.Send(new EquipmentPageQuery());
            return Ok();
        }

        [HttpDelete("[controller]/{category}/delete/")]
        public async Task<IActionResult> Delete(EntityType category, int id)
        {
            if (id <= 0)
                return BadRequest();

            return await _sender.Send(new DeleteEquipmentCommand(category, id)) ? Ok() : NotFound();
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
                var id = await _sender.Send(new UpdateEquipmentCommand(request));
                return Redirect($"/equipment/{request.EquipmentType}/{id}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Failed to update equipment: {ex.Message}");
                return View("CreateUpdate", request);
            }
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("equipment/{category}/{id}/edit", Order = 1)]
        public async Task<IActionResult> Edit(EntityType category, int id)
        {
            if (id <= 0)
                return BadRequest();

            var vm = await _sender.Send(new GetEquipmentQuery(category, id, null, 1, DefaultEquipmentAlbumsPageSize));
            if (vm is null)
                return NotFound();
            vm.Action = ActionType.Update;
            return View("CreateUpdate", vm);
        }

        [HttpGet("equipment/{category}/{id}/albums-data", Order = 0)]
        public async Task<IActionResult> EquipmentAlbumsData(EntityType category, int id, int page = 1, int pageSize = DefaultEquipmentAlbumsPageSize)
        {
            if (id <= 0)
                return BadRequest();
            var vm = await _sender.Send(new GetEquipmentAlbumsQuery(category, id, page, pageSize));
            return vm is null ? NotFound() : PartialView("_EquipmentReleasedAlbumsInner", vm);
        }

        [HttpGet("equipment/{category}/{id}", Order = 2)]
        public async Task<IActionResult> GetById(EntityType category, int id, string? tab = null, int page = 1, int pageSize = DefaultEquipmentAlbumsPageSize)
        {
            if (id <= 0)
                return BadRequest();
            var vm = await _sender.Send(new GetEquipmentQuery(category, id, tab, page, pageSize));
            return vm is null ? NotFound() : View("Details", vm);
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("equipment/create")]
        public async Task<IActionResult> Create() => View("CreateUpdate", await _sender.Send(new CreateEquipmentFormQuery()));

        [Authorize(Roles = RoleNames.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(EquipmentViewModel request)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.ModelName))
                return View("CreateUpdate", request);

            var validTypes = new[] { EntityType.Adc, EntityType.Amplifier, EntityType.Cartridge, EntityType.Player, EntityType.Wire };
            if (!validTypes.Contains(request.EquipmentType))
            {
                ModelState.AddModelError(nameof(request.EquipmentType), "Invalid equipment type selected.");
                return View("CreateUpdate", request);
            }

            var id = await _sender.Send(new CreateEquipmentCommand(request));
            return Redirect($"/equipment/{request.EquipmentType}/{id}");
        }
    }
}
