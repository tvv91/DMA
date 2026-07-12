using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Controllers;
using Web.Enums;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class AlbumControllerDeleteTests
{
    private readonly AlbumControllerTestFactory _factory = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Delete_InvalidId_ReturnsBadRequest(int id)
    {
        var controller = _factory.CreateController();
        var result = await controller.Delete(id);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid album ID", badRequest.Value);
        _factory.AlbumServiceMock.Verify(s => s.DeleteAlbumAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Delete_AlbumNotFound_ReturnsNotFound()
    {
        _factory.AlbumServiceMock
            .Setup(s => s.DeleteAlbumAsync(10))
            .ReturnsAsync(false);

        var controller = _factory.CreateController();
        var result = await controller.Delete(10);

        Assert.IsType<NotFoundResult>(result);
        _factory.ImageServiceMock.Verify(
            s => s.RemoveAsync(It.IsAny<int>(), It.IsAny<EntityType>()),
            Times.Never);
    }

    [Fact]
    public async Task Delete_Success_RemovesCoverAndReturnsOk()
    {
        _factory.AlbumServiceMock
            .Setup(s => s.DeleteAlbumAsync(10))
            .ReturnsAsync(true);

        var controller = _factory.CreateController();
        var result = await controller.Delete(10);

        Assert.IsType<OkResult>(result);
        _factory.ImageServiceMock.Verify(
            s => s.RemoveAsync(10, EntityType.AlbumCover),
            Times.Once);
    }

    [Fact]
    public async Task Delete_CoverRemovalFails_ReturnsBadRequest()
    {
        _factory.AlbumServiceMock
            .Setup(s => s.DeleteAlbumAsync(10))
            .ReturnsAsync(true);
        _factory.ImageServiceMock
            .Setup(s => s.RemoveAsync(10, EntityType.AlbumCover))
            .ThrowsAsync(new IOException("File locked"));

        var controller = _factory.CreateController();
        var result = await controller.Delete(10);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to delete album", badRequest.Value);
    }
}
