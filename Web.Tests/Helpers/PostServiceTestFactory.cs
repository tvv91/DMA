using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.Tests.Helpers;

internal sealed class PostServiceTestFactory : IDisposable
{
    private static readonly DateTimeOffset FixedUtcNow = new(2024, 8, 20, 14, 30, 0, TimeSpan.Zero);

    public Context Context { get; }
    public FakeTimeProvider TimeProvider { get; } = new(FixedUtcNow);
    public PostService Service { get; }

    public DateTime FixedUtcDateTime => FixedUtcNow.UtcDateTime;

    public PostServiceTestFactory()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new Context(options);
        Context.Database.EnsureCreated();
        Service = new PostService(Context, TimeProvider);
    }

    public async Task<Post> SeedPostAsync(
        string title = "Test Post",
        string description = "Test description",
        string content = "Test content",
        string? categoryTitle = null,
        bool isDraft = false,
        DateTime? createdDate = null,
        bool clearCreatedDate = false)
    {
        var post = new Post
        {
            Title = title,
            Description = description,
            Content = content,
            IsDraft = isDraft,
            CreatedDate = clearCreatedDate ? null : createdDate ?? FixedUtcDateTime,
        };

        if (categoryTitle is not null)
        {
            var category = await FindOrAddCategoryAsync(categoryTitle);
            post.PostCategories.Add(new PostCategory
            {
                Category = category,
                Post = post,
            });
        }

        Context.Posts.Add(post);
        await Context.SaveChangesAsync();
        return post;
    }

    public async Task<Category> FindOrAddCategoryAsync(string title)
    {
        var existing = Context.Categories.FirstOrDefault(c => c.Title == title);
        if (existing is not null)
            return existing;

        var category = new Category { Title = title };
        Context.Categories.Add(category);
        await Context.SaveChangesAsync();
        return category;
    }

    public static PostViewModel CreateViewModel(
        string title = "New Post",
        string description = "New description",
        string content = "New content",
        string category = "News",
        int? id = null) =>
        new()
        {
            Id = id,
            Title = title,
            Description = description,
            Content = content,
            Category = category,
        };

    public void Dispose() => Context.Dispose();
}
