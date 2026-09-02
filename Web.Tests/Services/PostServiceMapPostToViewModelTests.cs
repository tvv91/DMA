using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

public class PostServiceMapPostToViewModelTests
{
    [Fact]
    public void MapPostToViewModel_FullPost_MapsAllFields()
    {
        var factory = new PostServiceTestFactory();
        var post = factory.Context.Posts.Add(new Post
        {
            Id = 7,
            Title = "Mapped",
            Description = "Desc",
            Content = "Body",
            CreatedDate = new DateTime(2024, 1, 1),
            UpdatedDate = new DateTime(2024, 2, 2),
            IsDraft = true,
        }).Entity;
        var category = new Category { Title = "MappedCat" };
        post.PostCategories.Add(new PostCategory { Category = category, Post = post });

        var result = factory.Service.MapPostToViewModel(post);

        Assert.Equal(7, result.Id);
        Assert.Equal("Mapped", result.Title);
        Assert.Equal("Desc", result.Description);
        Assert.Equal("Body", result.Content);
        Assert.Equal(new DateTime(2024, 1, 1), result.CreatedDate);
        Assert.Equal(new DateTime(2024, 2, 2), result.UpdatedTime);
        Assert.Equal("MappedCat", result.Category);
        Assert.True(result.IsDraft);
        factory.Dispose();
    }

    [Fact]
    public void MapPostToViewModel_NoCategory_MapsNullCategory()
    {
        var factory = new PostServiceTestFactory();
        var post = new Post { Id = 1, Title = "No Cat" };

        var result = factory.Service.MapPostToViewModel(post);

        Assert.Null(result.Category);
        factory.Dispose();
    }
}
