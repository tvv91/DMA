using Microsoft.AspNetCore.Mvc;
using Web.Controllers;

namespace Web.Tests.Controllers;

public class AboutControllerIndexTests
{
    [Fact]
    public void Index_ReturnsView()
    {
        var controller = new AboutController();

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }
}
