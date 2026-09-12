using Microsoft.AspNetCore.SignalR;
using MediatR;
using Web.Common;
using Web.Features.Posts;
using Web.ViewModels;

namespace Web.SignalRHubs
{
    public class PostHub(ISender sender, TimeProvider timeProvider) : Hub
    {
        private readonly ISender _sender = sender;
        private readonly TimeProvider _timeProvider = timeProvider;
        private const int POSTS_PER_PAGE = 5;

        public async Task GetPosts(string connectionId, int page, string searchText, string category, string year, bool onlyDrafts)
        {
            var isAdmin = Context.User.IsInRole(RoleNames.Admin);
            if (onlyDrafts && !isAdmin)
                onlyDrafts = false;

            var excludeDrafts = !isAdmin;
            var result = await _sender.Send(new GetHubPostsQuery(page, searchText, category, year, onlyDrafts, excludeDrafts));

            await Clients.Client(connectionId)
                .SendAsync("ReceivedPosts", result.Items, result.TotalPages);
        }

        public async Task GetBlogTree(string connectionId)
        {
            var tree = await _sender.Send(new GetBlogTreeQuery());

            await Clients.Client(connectionId)
                .SendAsync("ReceivedBlogTree", tree);
        }

        public async Task AutoSavePost(string connectionId, int id, string title, string description, string content, string category)
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
                    await Clients.Client(connectionId).SendAsync("PostCreated", postId, now);
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

