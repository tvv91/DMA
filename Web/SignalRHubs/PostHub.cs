using Microsoft.AspNetCore.SignalR;
using Web.Interfaces;
using Web.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.SignalRHubs
{
    public class PostHub : Hub
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostService _postService;
        private const int POSTS_PER_PAGE = 5;

        public PostHub(IPostRepository postRepository, IPostService postService)
        {
            _postRepository = postRepository;
            _postService = postService;
        }

        public async Task GetPosts(string connectionId, int page, string searchText, string category, string year, bool onlyDrafts)
        {
            var result = await _postRepository.GetFilteredListAsync(page, POSTS_PER_PAGE, searchText, category, year, onlyDrafts);

            var response = result.Items
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Description,
                    p.IsDraft,
                    Created = p.CreatedDate.HasValue ? p.CreatedDate.Value.ToShortDateString() : null,
                    Categories = p.PostCategories.Select(pc => pc.Category.Title).ToList()
                }).ToList();

            await Clients.Client(connectionId)
                .SendAsync("ReceivedPosts", response, result.TotalPages);
        }

        public async Task GetBlogTree(string connectionId)
        {
            var posts = await _postRepository.GetListAsync(1, int.MaxValue);
            var allPosts = posts.Items.Where(p => !p.IsDraft && p.CreatedDate.HasValue).ToList();

            var tree = allPosts
                .SelectMany(p => p.PostCategories.Any() 
                    ? p.PostCategories.Select(pc => new { Post = p, Category = pc.Category })
                    : new[] { new { Post = p, Category = (Category?)null } })
                .GroupBy(x => x.Category?.Title ?? "Uncategorized")
                .Select(catGroup => new
                {
                    Category = catGroup.Key,
                    Posts = catGroup
                        .Select(x => x.Post)
                        .Distinct()
                        .GroupBy(p => p.CreatedDate!.Value.Year)
                        .OrderByDescending(g => g.Key)
                        .Select(yearGroup => new
                        {
                            Year = yearGroup.Key,
                            Posts = yearGroup
                                .OrderByDescending(p => p.CreatedDate)
                                .Select(p => new
                                {
                                    Id = p.Id,
                                    Title = p.Title,
                                    Created = p.CreatedDate!.Value.ToShortDateString()
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .Where(cat => cat.Posts.Any())
                .OrderBy(x => x.Category)
                .ToList();

            await Clients.Client(connectionId)
                .SendAsync("ReceivedBlogTree", tree);
        }

        public async Task AutoSavePost(string connectionId, int id, string title, string description, string content, string category)
        {
            var now = DateTime.UtcNow;
            bool isNew = id == 0;

            try
            {
                if (isNew)
                {
                    // Create new draft post
                    var model = new PostViewModel
                    {
                        Title = title,
                        Description = description,
                        Content = content,
                        Category = category
                    };

                    var post = await _postService.CreateDraftPostAsync(model);
                    await Clients.Client(connectionId).SendAsync("PostCreated", post.Id, now);
                }
                else
                {
                    // Update existing post
                    var model = new PostViewModel
                    {
                        Title = title,
                        Description = description,
                        Content = content,
                        Category = category
                    };

                    await _postService.UpdatePostAsync(id, model);
                    await Clients.Client(connectionId).SendAsync("PostUpdated", now);
                }
            }
            catch (KeyNotFoundException)
            {
                // Post not found, ignore silently for autosave
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid breaking autosave
                // TODO: Add proper logging
            }
        }
    }
}

