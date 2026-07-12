using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Moq;
using Web.Controllers;
using Web.Interfaces;

namespace Web.Tests.Helpers;

internal sealed class EquipmentControllerTestFactory
{
    public Mock<IEquipmentService> EquipmentServiceMock { get; } = new();
    public Mock<IImageService> ImageServiceMock { get; } = new();
    public Mock<IReleaseService> ReleaseServiceMock { get; } = new();

    public EquipmentController CreateController()
    {
        var controller = new EquipmentController(
            EquipmentServiceMock.Object,
            ImageServiceMock.Object,
            ReleaseServiceMock.Object);

        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock
            .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
            .Returns("/Album/GetById/1");

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext(),
            RouteData = new RouteData()
        };
        controller.Url = urlHelperMock.Object;

        return controller;
    }
}
