using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Moq;
using Web.Common;
using Web.Controllers;
using Web.Interfaces;

namespace Web.Tests.Helpers;

internal sealed class PostControllerTestFactory
{
    public Mock<IPostService> PostServiceMock { get; } = new();

    public PostController CreateController(bool asAdmin = false)
    {
        var controller = new PostController(PostServiceMock.Object);
        var httpContext = new DefaultHttpContext();

        if (asAdmin)
        {
            var identity = new ClaimsIdentity(
                [new Claim(ClaimTypes.Role, RoleNames.Admin)],
                authenticationType: "Test");
            httpContext.User = new ClaimsPrincipal(identity);
        }

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext,
            RouteData = new RouteData(),
        };
        controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

        return controller;
    }
}
