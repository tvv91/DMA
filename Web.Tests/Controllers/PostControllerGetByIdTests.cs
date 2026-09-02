using Microsoft.AspNetCore.Mvc;
using Moq;
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

    [Fact]
    public async Task GetById_DraftPost_NonAdmin_ReturnsNotFound()
    {
        var vm = new PostViewModel
        {
            Id = 101,
            Title = "Draft",
            Description = "Draft",
            Content = "Draft",
            IsDraft = true,
        };
        _factory.PostServiceMock
            .Setup(s => s.GetPostViewModelAsync(101))
            .ReturnsAsync(vm);

        var controller = _factory.CreateController();
        var result = await controller.GetById(101);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetById_DraftPost_Admin_ReturnsDetailsView()
    {
        var vm = new PostViewModel
        {
            Id = 101,
            Title = "Draft",
            Description = "Draft",
            Content = "Draft",
            IsDraft = true,
        };
        _factory.PostServiceMock
            .Setup(s => s.GetPostViewModelAsync(101))
            .ReturnsAsync(vm);

        var controller = _factory.CreateController(asAdmin: true);
        var result = await controller.GetById(101);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Details", viewResult.ViewName);
        Assert.Same(vm, viewResult.Model);
    }
}
