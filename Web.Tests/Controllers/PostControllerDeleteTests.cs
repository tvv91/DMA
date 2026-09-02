using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class PostControllerDeleteTests
{
    private readonly PostControllerTestFactory _factory = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Delete_InvalidId_ReturnsBadRequest(int id)
    {
        var controller = _factory.CreateController();

        var result = await controller.Delete(id);

        Assert.IsType<BadRequestResult>(result);
        _factory.PostServiceMock.Verify(s => s.DeletePostAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Delete_NotFound_ReturnsNotFound()
    {
        _factory.PostServiceMock
            .Setup(s => s.DeletePostAsync(5))
            .ReturnsAsync(false);

        var controller = _factory.CreateController();
        var result = await controller.Delete(5);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_Success_RedirectsToIndex()
    {
        _factory.PostServiceMock
            .Setup(s => s.DeletePostAsync(5))
            .ReturnsAsync(true);

        var controller = _factory.CreateController();
        var result = await controller.Delete(5);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
    }
}
