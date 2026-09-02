using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Controllers;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class AlbumControllerEditTests
{
    private readonly AlbumControllerTestFactory _factory = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public async Task Edit_InvalidId_ReturnsBadRequest(int id)
    {
        var controller = _factory.CreateController();

        var result = await controller.Edit(id);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid album Id", badRequest.Value);
    }

    [Fact]
    public async Task Edit_AlbumNotFound_ReturnsNotFound()
    {
        _factory.AlbumServiceMock
            .Setup(s => s.GetByIdAsync(10))
            .ReturnsAsync((Album?)null);

        var controller = _factory.CreateController();
        var result = await controller.Edit(10);

        Assert.IsType<NotFoundResult>(result);
        _factory.AlbumServiceMock.Verify(s => s.MapAlbumToCreateUpdateVMAsync(It.IsAny<Album>()), Times.Never);
    }

    [Fact]
    public async Task Edit_KeyNotFoundException_ReturnsNotFound()
    {
        _factory.AlbumServiceMock
            .Setup(s => s.GetByIdAsync(10))
            .ThrowsAsync(new KeyNotFoundException());

        var controller = _factory.CreateController();
        var result = await controller.Edit(10);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_ExistingAlbum_ReturnsCreateUpdateView()
    {
        var album = new Album { Id = 10, Title = "Edit Album" };
        var vm = new AlbumCreateUpdateViewModel { AlbumId = 10, Title = "Edit Album", Action = ActionType.Update };
        _factory.AlbumServiceMock
            .Setup(s => s.GetByIdAsync(10))
            .ReturnsAsync(album);
        _factory.AlbumServiceMock
            .Setup(s => s.MapAlbumToCreateUpdateVMAsync(album))
            .ReturnsAsync(vm);

        var controller = _factory.CreateController();
        var result = await controller.Edit(10);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        Assert.Same(vm, viewResult.Model);
    }
}
