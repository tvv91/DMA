using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class EquipmentControllerCreateGetTests
{
    private readonly EquipmentControllerTestFactory _factory = new();

    [Fact]
    public void Create_ReturnsCreateUpdateViewWithDefaults()
    {
        var controller = _factory.CreateController();

        var result = controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        var vm = Assert.IsType<EquipmentViewModel>(viewResult.Model);
        Assert.Equal(ActionType.Create, vm.Action);
        Assert.Equal(EntityType.Adc, vm.EquipmentType);
    }
}
