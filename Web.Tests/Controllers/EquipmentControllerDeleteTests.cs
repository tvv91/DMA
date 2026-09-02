using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Interfaces;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class EquipmentControllerDeleteTests
{
    private readonly EquipmentControllerTestFactory _factory = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Delete_InvalidId_ReturnsBadRequest(int id)
    {
        var controller = _factory.CreateController();

        var result = await controller.Delete(EntityType.Player, id);

        Assert.IsType<BadRequestResult>(result);
        _factory.EquipmentServiceMock.Verify(s => s.DeleteEquipmentAsync(It.IsAny<int>(), It.IsAny<EntityType>()), Times.Never);
    }

    [Fact]
    public async Task Delete_EquipmentNotFound_ReturnsNotFound()
    {
        _factory.EquipmentServiceMock
            .Setup(s => s.DeleteEquipmentAsync(10, EntityType.Adc))
            .ReturnsAsync(false);

        var controller = _factory.CreateController();
        var result = await controller.Delete(EntityType.Adc, 10);

        Assert.IsType<NotFoundResult>(result);
        _factory.ImageServiceMock.Verify(s => s.RemoveAsync(10, EntityType.Adc), Times.Once);
    }

    [Fact]
    public async Task Delete_Success_ReturnsOkAndRemovesImage()
    {
        _factory.EquipmentServiceMock
            .Setup(s => s.DeleteEquipmentAsync(10, EntityType.Cartridge))
            .ReturnsAsync(true);

        var controller = _factory.CreateController();
        var result = await controller.Delete(EntityType.Cartridge, 10);

        Assert.IsType<OkResult>(result);
        _factory.ImageServiceMock.Verify(s => s.RemoveAsync(10, EntityType.Cartridge), Times.Once);
    }

    [Fact]
    public async Task Delete_ImageServiceThrows_WrapsInvalidOperationException()
    {
        _factory.EquipmentServiceMock
            .Setup(s => s.DeleteEquipmentAsync(10, EntityType.Wire))
            .ReturnsAsync(true);
        _factory.ImageServiceMock
            .Setup(s => s.RemoveAsync(10, EntityType.Wire))
            .ThrowsAsync(new IOException("File locked"));

        var controller = _factory.CreateController();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => controller.Delete(EntityType.Wire, 10));

        Assert.Equal("Error during deleting equipment", exception.Message);
        Assert.IsType<IOException>(exception.InnerException);
    }
}
