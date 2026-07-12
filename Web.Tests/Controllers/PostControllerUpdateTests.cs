using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class PostControllerUpdateTests
{
    private readonly PostControllerTestFactory _factory = new();

    private static PostViewModel ValidUpdateModel() => new()
    {
        Id = 10,
        Title = "Updated Post",
        Description = "Updated description",
        Content = "Updated content",
        Category = "News",
    };

    [Fact]
    public async Task Update_NullId_ReturnsBadRequest()
    {
        var model = ValidUpdateModel();
        model.Id = null;

        var controller = _factory.CreateController();
        var result = await controller.Update(model);

        Assert.IsType<BadRequestResult>(result);
        _factory.PostServiceMock.Verify(
            s => s.UpdatePostAsync(It.IsAny<int>(), It.IsAny<PostViewModel>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_ValidModel_RedirectsToGetByIdAndSetsTempData()
    {
        var model = ValidUpdateModel();
        var post = new Post { Id = 10, Title = model.Title };
        _factory.PostServiceMock
            .Setup(s => s.UpdatePostAsync(10, model))
            .ReturnsAsync(post);

        var controller = _factory.CreateController();
        var result = await controller.Update(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("GetById", redirect.ActionName);
        Assert.Equal(10, redirect.RouteValues!["id"]);
        Assert.True(controller.TempData.ContainsKey("PostUpdated"));
        Assert.True((bool)controller.TempData["PostUpdated"]!);
    }

    [Fact]
    public async Task Update_ValidModel_CallsUpdatePostAsync()
    {
        var model = ValidUpdateModel();
        _factory.PostServiceMock
            .Setup(s => s.UpdatePostAsync(10, model))
            .ReturnsAsync(new Post { Id = 10, Title = model.Title });

        var controller = _factory.CreateController();
        await controller.Update(model);

        _factory.PostServiceMock.Verify(s => s.UpdatePostAsync(10, model), Times.Once);
    }

    [Fact]
    public async Task Update_NotFound_ReturnsNotFound()
    {
        var model = ValidUpdateModel();
        _factory.PostServiceMock
            .Setup(s => s.UpdatePostAsync(10, model))
            .ThrowsAsync(new KeyNotFoundException());

        var controller = _factory.CreateController();
        var result = await controller.Update(model);

        Assert.IsType<NotFoundResult>(result);
    }
}
