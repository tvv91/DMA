using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class PostControllerCreateTests
{
    private readonly PostControllerTestFactory _factory = new();

    private static PostViewModel ValidModel() => new()
    {
        Title = "New Post",
        Description = "Short description",
        Content = "Full content",
        Category = "News",
    };

    [Fact]
    public async Task Create_InvalidModelState_ReturnsViewWithoutCallingService()
    {
        var model = ValidModel();
        var controller = _factory.CreateController();
        controller.ModelState.AddModelError("Title", "Required");

        var result = await controller.Create(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        Assert.Same(model, viewResult.Model);
        _factory.PostServiceMock.Verify(s => s.CreatePostAsync(It.IsAny<PostViewModel>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Category")]
    public async Task Create_InvalidCategory_ReturnsViewWithCategoryError(string? category)
    {
        var model = ValidModel();
        model.Category = category!;
        var controller = _factory.CreateController();

        var result = await controller.Create(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        Assert.False(controller.ModelState.IsValid);
        Assert.Contains(
            controller.ModelState["Category"]!.Errors,
            e => e.ErrorMessage == "Please select a valid category");
        _factory.PostServiceMock.Verify(s => s.CreatePostAsync(It.IsAny<PostViewModel>()), Times.Never);
    }

    [Fact]
    public async Task Create_ValidModel_RedirectsToGetByIdAndSetsTempData()
    {
        var model = ValidModel();
        var post = new Post { Id = 15, Title = model.Title };
        _factory.PostServiceMock
            .Setup(s => s.CreatePostAsync(model))
            .ReturnsAsync(post);

        var controller = _factory.CreateController();
        var result = await controller.Create(model);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("GetById", redirect.ActionName);
        Assert.Equal(15, redirect.RouteValues!["id"]);
        Assert.True(controller.TempData.ContainsKey("PostCreated"));
        Assert.True((bool)controller.TempData["PostCreated"]!);
    }

    [Fact]
    public async Task Create_ValidModel_CallsCreatePostAsync()
    {
        var model = ValidModel();
        _factory.PostServiceMock
            .Setup(s => s.CreatePostAsync(model))
            .ReturnsAsync(new Post { Id = 1, Title = model.Title });

        var controller = _factory.CreateController();
        await controller.Create(model);

        _factory.PostServiceMock.Verify(s => s.CreatePostAsync(model), Times.Once);
    }
}
