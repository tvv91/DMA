using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class EquipmentControllerIndexTests
{
    private readonly EquipmentControllerTestFactory _factory = new();

    [Fact]
    public void Index_ReturnsView()
    {
        var controller = _factory.CreateController();

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Category_ReturnsOk()
    {
        var controller = _factory.CreateController();

        var result = controller.Category("player");

        Assert.IsType<OkResult>(result);
    }
}
