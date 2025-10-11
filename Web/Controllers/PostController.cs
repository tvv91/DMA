using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;

namespace Web.Controllers
{
    public class PostController : Controller
    {
        //private readonly IPostRepository _postRepository;
        //public PostController(IPostRepository postRepository)
        //{
        //    _postRepository = postRepository;
        //}

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //[HttpGet("post/new")]
        //public IActionResult New()
        //{
        //    return View("CreateUpdate");
        //}

        //[HttpGet("post/{id}")]
        //public async Task<IActionResult> GetPostById(int id)
        //{
        //    var post = await _postRepository.Posts.FirstOrDefaultAsync(x => x.Id == id);
            
        //    if (post is null)
        //    {
        //        return NotFound();
        //    }

        //    var postViewModel = new PostViewModel
        //    {
        //        Id = post.Id,
        //        Title = post.Title,
        //        Description = post.Description,
        //        Content = post.Content,
        //        CreatedDate = post.CreatedDate,
        //        UpdatedTime = post.UpdatedDate
        //    };

        //    return View("Details", postViewModel);
        //}

        //[HttpPost("post/create")]
        //public async Task<IActionResult> Create(PostViewModel model)
        //{
        //    if (model.Category == "Category")
        //    {
        //        ModelState.AddModelError("Category", "Please, select some category");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var post = await _postRepository.Posts.FirstOrDefaultAsync(x => x.Id == model.Id);

        //        if (post is null)
        //        {
        //            post = new Post
        //            {
        //                Title = model.Title,
        //                Description = model.Description,
        //                Content = model.Content,
        //                CreatedDate = DateTime.Now,
        //                IsDraft = false,
        //            };

        //            var category = new Category
        //            {
        //                Title = model.Category
        //            };

        //            var postCategory = new PostCategory
        //            {
        //                Post = post,
        //                Category = category
        //            };

        //            await _postRepository.AddPostAsync(postCategory);
        //        } 
        //        else
        //        {
        //            await _postRepository.Posts.Where(x => x.Id == model.Id).ExecuteUpdateAsync(
        //                x => x.SetProperty(x => x.Title, model.Title)
        //                .SetProperty(x => x.Description, model.Description)
        //                .SetProperty(x => x.Content, model.Content)
        //                .SetProperty(x => x.IsDraft, false));
        //        }
        //        return Redirect("/");
        //    }
        //    return View("CreateUpdate", model);
        //}

        //[HttpDelete("post/delete")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    if (id <= 0)
        //        return BadRequest();

        //    try
        //    {
        //        if (await _postRepository.Posts.Where(x => x.Id == id).ExecuteDeleteAsync() == 0)
        //            return NotFound();
        //    } 
        //    catch (Exception ex)
        //    {
        //        // TODO: Add logging
        //        return BadRequest("Some error during post removing");
        //    }
            
        //    return Ok();
        //}

        //[HttpGet("post/edit")]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    if (id <= 0)
        //        return BadRequest();

        //    var post = await _postRepository.Posts.Include(x => x.PostCategories).ThenInclude(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);

        //    if (post is null)
        //        return NotFound();

        //    var postdata = new PostViewModel
        //    {
        //        Id = post.Id,
        //        Title = post.Title,
        //        Description = post.Description,
        //        Content = post.Content,
        //        CreatedDate = post.CreatedDate,
        //        UpdatedTime = post.UpdatedDate,
        //        Category = post.PostCategories.First().Category.Title
        //    };

        //    return View("CreateUpdate", postdata);
        //}

        //[HttpPost("post/update")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Update(PostViewModel model)
        //{
        //    if (model.Id is null)
        //        return BadRequest();

        //    var post = await _postRepository.Posts.FirstOrDefaultAsync(x => x.Id == model.Id);
            
        //    if (post is null)
        //        return NotFound();

        //    var _postCategory = await _postRepository.PostCategories.Include(x => x.Category).FirstOrDefaultAsync(x => x.Post.Id == model.Id);
        //    var updatedDate = DateTime.Now;
            
        //    // if we change category, we need to remove old relation and add new
        //    if (_postCategory?.Category.Title != model.Category)
        //    {
        //        await _postRepository.PostCategories.Where(x => x.Post.Id == model.Id).ExecuteDeleteAsync();

        //        var _category = await _postRepository.Categories.FirstOrDefaultAsync(x => x.Title == model.Category) ?? new Category { Title = model.Category };

        //        var postCategory = new PostCategory
        //        {
        //            Post = new Post
        //            {
        //                Title = model.Title,
        //                Description = model.Description,
        //                Content = model.Content,
        //                UpdatedDate = updatedDate
        //            },
        //            Category = _category
        //        };

        //        var result = await _postRepository.AddPostAsync(postCategory);
        //    }

        //    await _postRepository.Posts.Where(x => x.Id == model.Id).ExecuteUpdateAsync(x => x
        //    .SetProperty(x => x.Title, model.Title)
        //    .SetProperty(x => x.Description, model.Description)
        //    .SetProperty(x => x.Content, model.Content)
        //    .SetProperty(x => x.UpdatedDate, updatedDate));

        //    return Redirect($"/post/{post.Id}");
        //}
    }
}
