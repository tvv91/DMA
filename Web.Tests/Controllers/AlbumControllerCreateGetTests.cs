using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Controllers;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class AlbumControllerCreateGetTests
{
    private readonly AlbumControllerTestFactory _factory = new();

    [Fact]
    public void Create_ReturnsCreateUpdateViewWithCreateAction()
    {
        var controller = _factory.CreateController();

        var result = controller.Create();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        var vm = Assert.IsType<AlbumCreateUpdateViewModel>(viewResult.Model);
        Assert.Equal(ActionType.Create, vm.Action);
        Assert.Equal(0, vm.AlbumId);
    }
}
