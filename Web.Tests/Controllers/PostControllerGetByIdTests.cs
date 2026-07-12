using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class PostControllerGetByIdTests
{
    private readonly PostControllerTestFactory _factory = new();

    [Fact]
    public async Task GetById_ExistingPost_ReturnsDetailsView()
    {
        var vm = new PostViewModel
        {
            Id = 7,
            Title = "Post Title",
            Description = "Description",
            Content = "Content",
            Category = "News",
        };
        _factory.PostServiceMock
            .Setup(s => s.GetPostViewModelAsync(7))
            .ReturnsAsync(vm);

        var controller = _factory.CreateController();
        var result = await controller.GetById(7);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Details", viewResult.ViewName);
        Assert.Same(vm, viewResult.Model);
    }

    [Fact]
    public async Task GetById_NotFound_ReturnsNotFound()
    {
        _factory.PostServiceMock
            .Setup(s => s.GetPostViewModelAsync(99))
            .ThrowsAsync(new KeyNotFoundException());

        var controller = _factory.CreateController();
        var result = await controller.GetById(99);

        Assert.IsType<NotFoundResult>(result);
    }
}
