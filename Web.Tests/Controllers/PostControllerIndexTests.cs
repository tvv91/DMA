using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class PostControllerIndexTests
{
    private readonly PostControllerTestFactory _factory = new();

    [Fact]
    public void Index_ReturnsView()
    {
        var controller = _factory.CreateController();

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }
}
