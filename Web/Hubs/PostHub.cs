using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Web.Authorization;
using Web.Features.Posts;
using Web.ViewModels;

namespace Web.Hubs
{
    public class PostHub(
        ISender sender,
        TimeProvider timeProvider,
        ILogger<PostHub> logger) : Hub
    {
        private readonly ISender _sender = sender;
        private readonly TimeProvider _timeProvider = timeProvider;
        private readonly ILogger<PostHub> _logger = logger;

        public async Task GetPosts(int page, string searchText, string category, string year, bool onlyDrafts)
        {
            var isAdmin = Context.User.IsInRole(RoleNames.Admin);
            if (onlyDrafts && !isAdmin)
                onlyDrafts = false;

            var excludeDrafts = !isAdmin;
            var result = await _sender.Send(new GetHubPostsQuery(page, searchText, category, year, onlyDrafts, excludeDrafts));

            await Clients.Caller
                .SendAsync("ReceivedPosts", result.Items, result.TotalPages);
        }

        public async Task GetBlogTree()
        {
            var tree = await _sender.Send(new GetBlogTreeQuery());

            await Clients.Caller
                .SendAsync("ReceivedBlogTree", tree);
        }

        [Authorize(Roles = RoleNames.Admin)]
        public async Task AutoSavePost(int id, string title, string description, string content, string category)
        {
            var now = _timeProvider.GetUtcNow().UtcDateTime;
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

                    var postId = await _sender.Send(new CreateDraftPostCommand(model));
                    await Clients.Caller.SendAsync("PostCreated", postId, now);
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

                    model.Id = id;
                    await _sender.Send(new UpdatePostCommand(model));
                    await Clients.Caller.SendAsync("PostUpdated", now);
                }
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Post {PostId} was not found during autosave", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to autosave post {PostId}", id);
            }
        }
    }
}

