using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Web.Authorization;
using Web.Features.Posts;
using Web.ViewModels;

namespace Web.Controllers
{
    public class PostController(ISender sender) : Controller
    {
        private readonly ISender _sender = sender;

        public async Task<IActionResult> Index()
        {
            await _sender.Send(new PostPageQuery());
            return View();
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("post/new")]
        public async Task<IActionResult> New() => View("CreateUpdate", await _sender.Send(new NewPostQuery()));

        [HttpGet("post/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var vm = await _sender.Send(new GetPostQuery(id));
                if (vm.IsDraft && !User.IsInRole(RoleNames.Admin))
                    return NotFound();
                return View("Details", vm);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpPost("post/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Category) || model.Category == "Category")
                ModelState.AddModelError("Category", "Please select a valid category");
            if (!ModelState.IsValid)
                return View("CreateUpdate", model);

            var id = await _sender.Send(new CreatePostCommand(model));
            TempData["PostCreated"] = true;
            return RedirectToAction(nameof(GetById), new { id });
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpDelete("post/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest();
            return await _sender.Send(new DeletePostCommand(id))
                ? RedirectToAction(nameof(Index))
                : NotFound();
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpGet("post/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                return View("CreateUpdate", await _sender.Send(new GetPostQuery(id)));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = RoleNames.Admin)]
        [HttpPost("post/update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PostViewModel model)
        {
            if (model.Id is null)
                return BadRequest();
            try
            {
                var id = await _sender.Send(new UpdatePostCommand(model));
                TempData["PostUpdated"] = true;
                return RedirectToAction(nameof(GetById), new { id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
