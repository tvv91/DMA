using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("post/new")]
        public IActionResult New()
        {
            return View("CreateUpdate", new PostViewModel());
        }

        [HttpGet("post/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var vm = await _postService.GetPostViewModelAsync(id);
                return View("Details", vm);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("post/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Category) || model.Category == "Category")
                ModelState.AddModelError("Category", "Please select a valid category");

            if (!ModelState.IsValid)
                return View("CreateUpdate", model);

            var post = await _postService.CreatePostAsync(model);
            
            // Set flag to reset pagination to page 1 when returning to list
            TempData["PostCreated"] = true;
            
            return RedirectToAction(nameof(GetById), new { id = post.Id });
        }

        [HttpDelete("post/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            var success = await _postService.DeletePostAsync(id);
            
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("post/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var vm = await _postService.GetPostViewModelAsync(id);
                return View("CreateUpdate", vm);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("post/update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PostViewModel model)
        {
            if (model.Id is null)
                return BadRequest();

            try
            {
                var post = await _postService.UpdatePostAsync(model.Id.Value, model);
                
                // Set flag to reset pagination to page 1 when returning to list
                TempData["PostUpdated"] = true;
                
                return RedirectToAction(nameof(GetById), new { id = post.Id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
