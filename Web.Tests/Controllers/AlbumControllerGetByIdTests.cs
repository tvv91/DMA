using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Controllers;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class AlbumControllerGetByIdTests
{
    private readonly AlbumControllerTestFactory _factory = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetById_InvalidId_ReturnsBadRequest(int id)
    {
        var controller = _factory.CreateController();

        var result = await controller.GetById(id);

        Assert.IsType<BadRequestResult>(result);
        _factory.AlbumServiceMock.Verify(s => s.GetAlbumDetailsAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task GetById_ExistingAlbum_ReturnsDetailsView()
    {
        var details = new AlbumDetailsViewModel { AlbumId = 5, Title = "Details Album" };
        _factory.AlbumServiceMock
            .Setup(s => s.GetAlbumDetailsAsync(5))
            .ReturnsAsync(details);

        var controller = _factory.CreateController();
        var result = await controller.GetById(5);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Details", viewResult.ViewName);
        Assert.Same(details, viewResult.Model);
    }

    [Fact]
    public async Task GetById_AlbumNotFound_ReturnsNotFound()
    {
        _factory.AlbumServiceMock
            .Setup(s => s.GetAlbumDetailsAsync(999))
            .ThrowsAsync(new KeyNotFoundException());

        var controller = _factory.CreateController();
        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
