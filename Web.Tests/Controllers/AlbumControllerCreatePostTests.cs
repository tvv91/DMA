using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Controllers;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class AlbumControllerCreatePostTests
{
    private readonly AlbumControllerTestFactory _factory = new();

    private static AlbumCreateUpdateViewModel ValidRequest() => new()
    {
        Title = "New Album",
        Artist = "New Artist",
        Genre = "Rock",
    };

    [Fact]
    public async Task Create_InvalidModelState_ReturnsViewWithRequest()
    {
        var request = ValidRequest();
        var controller = _factory.CreateController();
        controller.ModelState.AddModelError("Title", "Required");

        var result = await controller.Create(request);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        Assert.Same(request, viewResult.Model);
        _factory.AlbumServiceMock.Verify(
            s => s.CreateOrFindAlbumAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_ValidRequestWithoutCover_RedirectsToGetById()
    {
        var request = ValidRequest();
        var album = new Album { Id = 42, Title = request.Title };
        _factory.AlbumServiceMock
            .Setup(s => s.CreateOrFindAlbumAsync(request.Title, request.Artist, request.Genre))
            .ReturnsAsync(album);

        var controller = _factory.CreateController();
        var result = await controller.Create(request);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("GetById", redirect.ActionName);
        Assert.Equal("Album", redirect.ControllerName);
        Assert.Equal(42, redirect.RouteValues!["id"]);
        _factory.ImageServiceMock.Verify(
            s => s.SaveAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_ValidRequestWithCover_SavesCoverAndRedirects()
    {
        var request = ValidRequest();
        request.AlbumCover = "cover.jpg";
        var album = new Album { Id = 42, Title = request.Title };
        _factory.AlbumServiceMock
            .Setup(s => s.CreateOrFindAlbumAsync(request.Title, request.Artist, request.Genre))
            .ReturnsAsync(album);

        var controller = _factory.CreateController();
        var result = await controller.Create(request);

        Assert.IsType<RedirectToActionResult>(result);
        _factory.ImageServiceMock.Verify(
            s => s.SaveAsync(42, "cover.jpg", EntityType.AlbumCover),
            Times.Once);
    }

    [Fact]
    public async Task Create_ServiceThrows_ReturnsViewWithModelError()
    {
        var request = ValidRequest();
        _factory.AlbumServiceMock
            .Setup(s => s.CreateOrFindAlbumAsync(request.Title, request.Artist, request.Genre))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        var controller = _factory.CreateController();
        var result = await controller.Create(request);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        Assert.False(controller.ModelState.IsValid);
        Assert.Contains(controller.ModelState[string.Empty]!.Errors, e => e.ErrorMessage.Contains("Database error"));
    }
}
