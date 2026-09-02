using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Interfaces;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class EquipmentControllerUpdateTests
{
    private readonly EquipmentControllerTestFactory _factory = new();

    private static EquipmentViewModel ValidUpdateRequest() => new()
    {
        Id = 5,
        ModelName = "Updated Model",
        EquipmentType = EntityType.Player,
        Action = ActionType.Update,
    };

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Update_InvalidId_ReturnsBadRequest(int id)
    {
        var request = ValidUpdateRequest();
        request.Id = id;
        var controller = _factory.CreateController();

        var result = await controller.Update(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid equipment ID", badRequest.Value);
    }

    [Fact]
    public async Task Update_InvalidActionType_ReturnsBadRequest()
    {
        var request = ValidUpdateRequest();
        request.Action = ActionType.Create;
        var controller = _factory.CreateController();

        var result = await controller.Update(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid action type", badRequest.Value);
    }

    [Fact]
    public async Task Update_InvalidModelState_ReturnsViewWithRequest()
    {
        var request = ValidUpdateRequest();
        var controller = _factory.CreateController();
        controller.ModelState.AddModelError("ModelName", "Required");

        var result = await controller.Update(request);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("CreateUpdate", viewResult.ViewName);
        _factory.EquipmentServiceMock.Verify(s => s.UpdateEquipmentAsync(It.IsAny<EquipmentViewModel>()), Times.Never);
    }

    [Fact]
    public async Task Update_NullEquipmentCover_RemovesImageAndRedirects()
    {
        var request = ValidUpdateRequest();
        request.EquipmentCover = null;
        var updated = new Player { Id = 5, Name = request.ModelName };
        _factory.EquipmentServiceMock
            .Setup(s => s.UpdateEquipmentAsync(request))
            .ReturnsAsync(updated);

        var controller = _factory.CreateController();
        var result = await controller.Update(request);

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/equipment/Player/5", redirect.Url);
        _factory.ImageServiceMock.Verify(s => s.RemoveAsync(5, EntityType.Player), Times.Once);
        _factory.ImageServiceMock.Verify(
            s => s.SaveAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>()),
            Times.Never);
    }

    [Fact]
    public async Task Update_WithEquipmentCover_SavesImageAndRedirects()
    {
        var request = ValidUpdateRequest();
        request.EquipmentCover = "new-cover.jpg";
        var updated = new Player { Id = 5, Name = request.ModelName };
        _factory.EquipmentServiceMock
            .Setup(s => s.UpdateEquipmentAsync(request))
            .ReturnsAsync(updated);

        var controller = _factory.CreateController();
        var result = await controller.Update(request);

        Assert.IsType<RedirectResult>(result);
        _factory.ImageServiceMock.Verify(
            s => s.SaveAsync(5, "new-cover.jpg", EntityType.Player),
            Times.Once);
    }

    [Fact]
    public async Task Update_ServiceThrows_ReturnsViewWithModelError()
    {
        var request = ValidUpdateRequest();
        _factory.EquipmentServiceMock
            .Setup(s => s.UpdateEquipmentAsync(request))
            .ThrowsAsync(new InvalidOperationException("Update failed"));

        var controller = _factory.CreateController();
        var result = await controller.Update(request);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(controller.ModelState.IsValid);
        Assert.Contains(controller.ModelState[string.Empty]!.Errors, e => e.ErrorMessage.Contains("Update failed"));
    }
}
