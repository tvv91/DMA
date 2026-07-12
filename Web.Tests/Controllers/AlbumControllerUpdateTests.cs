using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Controllers;
using Web.Enums;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class AlbumControllerUpdateTests
{
    private readonly AlbumControllerTestFactory _factory = new();

    private static AlbumCreateUpdateViewModel ValidUpdateRequest(int albumId = 5) => new()
    {
        AlbumId = albumId,
        Title = "Updated Title",
        Artist = "Updated Artist",
        Genre = "Updated Genre",
    };

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Update_InvalidAlbumId_ReturnsBadRequest(int albumId)
    {
        var controller = _factory.CreateController();
        var result = await controller.Update(ValidUpdateRequest(albumId));

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid album ID", badRequest.Value);
    }

    [Fact]
    public async Task Update_InvalidModelState_ReturnsViewWithRequest()
    {
        var request = ValidUpdateRequest();
        var controller = _factory.CreateController();
        controller.ModelState.AddModelError("Title", "Required");

        var result = await controller.Update(request);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        Assert.Same(request, viewResult.Model);
        _factory.AlbumServiceMock.Verify(s => s.UpdateAlbumAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Update_EmptyAlbumCover_RemovesCover(string? albumCover)
    {
        var request = ValidUpdateRequest();
        request.AlbumCover = albumCover;
        var album = new Album { Id = 5, Title = request.Title };
        _factory.AlbumServiceMock
            .Setup(s => s.UpdateAlbumAsync(request.AlbumId, request.Title, request.Artist, request.Genre))
            .ReturnsAsync(album);

        var controller = _factory.CreateController();
        var result = await controller.Update(request);

        Assert.IsType<RedirectToActionResult>(result);
        _factory.ImageServiceMock.Verify(
            s => s.RemoveAsync(5, EntityType.AlbumCover),
            Times.Once);
        _factory.ImageServiceMock.Verify(
            s => s.SaveAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_ExistingCoverReference_DoesNotSaveOrRemove()
    {
        var request = ValidUpdateRequest();
        request.AlbumCover = "5";
        var album = new Album { Id = 5, Title = request.Title };
        _factory.AlbumServiceMock
            .Setup(s => s.UpdateAlbumAsync(request.AlbumId, request.Title, request.Artist, request.Genre))
            .ReturnsAsync(album);

        var controller = _factory.CreateController();
        var result = await controller.Update(request);

        Assert.IsType<RedirectToActionResult>(result);
        _factory.ImageServiceMock.Verify(
            s => s.RemoveAsync(It.IsAny<int>(), It.IsAny<EntityType>()),
            Times.Never);
        _factory.ImageServiceMock.Verify(
            s => s.SaveAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_NewAlbumCover_SavesCover()
    {
        var request = ValidUpdateRequest();
        request.AlbumCover = "new-cover.jpg";
        var album = new Album { Id = 5, Title = request.Title };
        _factory.AlbumServiceMock
            .Setup(s => s.UpdateAlbumAsync(request.AlbumId, request.Title, request.Artist, request.Genre))
            .ReturnsAsync(album);

        var controller = _factory.CreateController();
        var result = await controller.Update(request);

        Assert.IsType<RedirectToActionResult>(result);
        _factory.ImageServiceMock.Verify(
            s => s.SaveAsync(5, "new-cover.jpg", EntityType.AlbumCover),
            Times.Once);
    }

    [Fact]
    public async Task Update_Success_RedirectsToGetById()
    {
        var request = ValidUpdateRequest();
        request.AlbumCover = "5";
        var album = new Album { Id = 5, Title = request.Title };
        _factory.AlbumServiceMock
            .Setup(s => s.UpdateAlbumAsync(request.AlbumId, request.Title, request.Artist, request.Genre))
            .ReturnsAsync(album);

        var controller = _factory.CreateController();
        var result = await controller.Update(request);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("GetById", redirect.ActionName);
        Assert.Equal("Album", redirect.ControllerName);
        Assert.Equal(5, redirect.RouteValues!["id"]);
    }

    [Fact]
    public async Task Update_ServiceThrows_ReturnsViewWithModelError()
    {
        var request = ValidUpdateRequest();
        _factory.AlbumServiceMock
            .Setup(s => s.UpdateAlbumAsync(request.AlbumId, request.Title, request.Artist, request.Genre))
            .ThrowsAsync(new KeyNotFoundException("Album not found"));

        var controller = _factory.CreateController();
        var result = await controller.Update(request);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        Assert.False(controller.ModelState.IsValid);
        Assert.Contains(controller.ModelState[string.Empty]!.Errors, e => e.ErrorMessage.Contains("Album not found"));
    }
}
