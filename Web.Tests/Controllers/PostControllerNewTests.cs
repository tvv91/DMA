using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Models;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class PostControllerNewTests
{
    private readonly PostControllerTestFactory _factory = new();

    [Fact]
    public void New_ReturnsCreateUpdateViewWithEmptyModel()
    {
        var controller = _factory.CreateController();

        var result = controller.New();

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        var model = Assert.IsType<PostViewModel>(viewResult.Model);
        Assert.Null(model.Id);
        Assert.Equal(string.Empty, model.Title);
    }
}
