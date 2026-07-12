using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class PostControllerEditTests
{
    private readonly PostControllerTestFactory _factory = new();

    [Fact]
    public async Task Edit_ExistingPost_ReturnsCreateUpdateView()
    {
        var vm = new PostViewModel
        {
            Id = 3,
            Title = "Edit Post",
            Description = "Desc",
            Content = "Content",
            Category = "News",
        };
        _factory.PostServiceMock
            .Setup(s => s.GetPostViewModelAsync(3))
            .ReturnsAsync(vm);

        var controller = _factory.CreateController();
        var result = await controller.Edit(3);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        Assert.Same(vm, viewResult.Model);
    }

    [Fact]
    public async Task Edit_NotFound_ReturnsNotFound()
    {
        _factory.PostServiceMock
            .Setup(s => s.GetPostViewModelAsync(8))
            .ThrowsAsync(new KeyNotFoundException());

        var controller = _factory.CreateController();
        var result = await controller.Edit(8);

        Assert.IsType<NotFoundResult>(result);
    }
}
