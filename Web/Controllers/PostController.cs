using Microsoft.AspNetCore.Mvc;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;

namespace Web.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        public PostController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
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
            var post = await _postRepository.GetByIdAsync(id);
            
            if (post is null)
                return NotFound();

            var vm = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                CreatedDate = post.CreatedDate,
                UpdatedTime = post.UpdatedDate,
                Category = post.PostCategories.FirstOrDefault()?.Category.Title
            };

            return View("Details", vm);
        }

        [HttpPost("post/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Category) || model.Category == "Category")
                ModelState.AddModelError("Category", "Please select a valid category");

            if (!ModelState.IsValid)
                return View("CreateUpdate", model);

            var post = new Post
            {
                Title = model.Title,
                Description = model.Description,
                Content = model.Content,
                CreatedDate = DateTime.UtcNow,
                IsDraft = false
            };

            post.PostCategories.Add(new PostCategory
            {
                Category = new Category { Title = model.Category }
            });

            await _postRepository.AddAsync(post);
            
            return RedirectToAction(nameof(GetById), new { id = post.Id });
        }

        [HttpDelete("post/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest();

            var success = await _postRepository.DeleteAsync(id);
            
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("post/edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            
            if (post is null)
                return NotFound();

            var vm = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                CreatedDate = post.CreatedDate,
                UpdatedTime = post.UpdatedDate,
                Category = post.PostCategories.FirstOrDefault()?.Category?.Title
            };

            return View("CreateUpdate", vm);
        }

        [HttpPost("post/update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(PostViewModel model)
        {
            if (model.Id is null)
                return BadRequest();

            var existing = await _postRepository.GetByIdAsync(model.Id.Value);
            
            if (existing is null)
                return NotFound();

            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.Content = model.Content;
            existing.UpdatedDate = DateTime.UtcNow;

            var currentCategory = existing.PostCategories.FirstOrDefault()?.Category?.Title;

            if (model.Category != currentCategory)
            {
                existing.PostCategories.Clear();
                existing.PostCategories.Add(new PostCategory
                {
                    Category = new Category { Title = model.Category }
                });
            }

            await _postRepository.UpdateAsync(existing);
            
            return RedirectToAction(nameof(GetById), new { id = existing.Id });
        }
    }
}
