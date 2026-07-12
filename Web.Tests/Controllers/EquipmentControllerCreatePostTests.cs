using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class EquipmentControllerCreatePostTests
{
    private readonly EquipmentControllerTestFactory _factory = new();

    private static EquipmentViewModel ValidRequest() => new()
    {
        ModelName = "SL-1200",
        Manufacturer = "Technics",
        EquipmentType = EntityType.Player,
        Action = ActionType.Create,
    };

    [Fact]
    public async Task Create_InvalidModelState_ReturnsViewWithRequest()
    {
        var request = ValidRequest();
        var controller = _factory.CreateController();
        controller.ModelState.AddModelError("ModelName", "Required");

        var result = await controller.Create(request);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        Assert.Same(request, viewResult.Model);
        _factory.EquipmentServiceMock.Verify(s => s.CreateEquipmentAsync(It.IsAny<EquipmentViewModel>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_EmptyModelName_ReturnsViewWithRequest(string? modelName)
    {
        var request = ValidRequest();
        request.ModelName = modelName!;
        var controller = _factory.CreateController();

        var result = await controller.Create(request);

        Assert.IsType<ViewResult>(result);
        _factory.EquipmentServiceMock.Verify(s => s.CreateEquipmentAsync(It.IsAny<EquipmentViewModel>()), Times.Never);
    }

    [Fact]
    public async Task Create_InvalidEquipmentType_ReturnsViewWithModelError()
    {
        var request = ValidRequest();
        request.EquipmentType = EntityType.Artist;
        var controller = _factory.CreateController();

        var result = await controller.Create(request);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.True(controller.ModelState.ContainsKey(nameof(request.EquipmentType)));
    }

    [Theory]
    [InlineData(EntityType.Adc)]
    [InlineData(EntityType.Amplifier)]
    [InlineData(EntityType.Cartridge)]
    [InlineData(EntityType.Player)]
    [InlineData(EntityType.Wire)]
    public async Task Create_ValidRequestWithoutCover_RedirectsToDetails(EntityType type)
    {
        var request = ValidRequest();
        request.EquipmentType = type;
        request.EquipmentCover = null;
        var equipment = new Player { Id = 42, Name = request.ModelName };
        _factory.EquipmentServiceMock
            .Setup(s => s.CreateEquipmentAsync(request))
            .ReturnsAsync(equipment);

        var controller = _factory.CreateController();
        var result = await controller.Create(request);

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal($"/equipment/{type}/42", redirect.Url);
        _factory.ImageServiceMock.Verify(
            s => s.SaveAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>()),
            Times.Never);
    }

    [Fact]
    public async Task Create_ValidRequestWithCover_SavesCoverAndRedirects()
    {
        var request = ValidRequest();
        request.EquipmentCover = "cover.jpg";
        var equipment = new Player { Id = 7, Name = request.ModelName };
        _factory.EquipmentServiceMock
            .Setup(s => s.CreateEquipmentAsync(request))
            .ReturnsAsync(equipment);

        var controller = _factory.CreateController();
        var result = await controller.Create(request);

        Assert.IsType<RedirectResult>(result);
        _factory.ImageServiceMock.Verify(
            s => s.SaveAsync(7, "cover.jpg", EntityType.Player),
            Times.Once);
    }
}
