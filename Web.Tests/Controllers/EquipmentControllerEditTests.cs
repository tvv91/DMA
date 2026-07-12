using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class EquipmentControllerEditTests
{
    private readonly EquipmentControllerTestFactory _factory = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Edit_InvalidId_ReturnsBadRequest(int id)
    {
        var controller = _factory.CreateController();

        var result = await controller.Edit(EntityType.Player, id);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task Edit_EquipmentNotFound_ReturnsNotFound()
    {
        _factory.EquipmentServiceMock
            .Setup(s => s.GetByIdAsync(10, EntityType.Player))
            .ReturnsAsync((IManufacturer?)null);

        var controller = _factory.CreateController();
        var result = await controller.Edit(EntityType.Player, 10);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_ExistingEquipment_ReturnsCreateUpdateView()
    {
        var equipment = new Player { Id = 10, Name = "SL-1200" };
        var vm = new EquipmentViewModel { Id = 10, ModelName = "SL-1200", EquipmentType = EntityType.Player };
        _factory.EquipmentServiceMock
            .Setup(s => s.GetByIdAsync(10, EntityType.Player))
            .ReturnsAsync(equipment);
        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(10, EntityType.Player))
            .ReturnsAsync("/img/player.png");
        _factory.EquipmentServiceMock
            .Setup(s => s.MapEquipmentToViewModel(equipment, EntityType.Player, "/img/player.png"))
            .Returns(vm);

        var controller = _factory.CreateController();
        var result = await controller.Edit(EntityType.Player, 10);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        var model = Assert.IsType<EquipmentViewModel>(viewResult.Model);
        Assert.Equal(ActionType.Update, model.Action);
    }
}
