using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Interfaces;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class EquipmentControllerGetByIdTests
{
    private readonly EquipmentControllerTestFactory _factory = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task GetById_InvalidId_ReturnsBadRequest(int id)
    {
        var controller = _factory.CreateController();

        var result = await controller.GetById(EntityType.Player, id);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task GetById_EquipmentNotFound_ReturnsNotFound()
    {
        _factory.EquipmentServiceMock
            .Setup(s => s.GetByIdAsync(5, EntityType.Adc))
            .ReturnsAsync((IManufacturer?)null);

        var controller = _factory.CreateController();
        var result = await controller.GetById(EntityType.Adc, 5);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetById_ExistingEquipment_ReturnsDetailsView()
    {
        var equipment = new Player { Id = 5, Name = "SL-1200" };
        var vm = new EquipmentViewModel { Id = 5, ModelName = "SL-1200", EquipmentType = EntityType.Player };
        _factory.EquipmentServiceMock
            .Setup(s => s.GetByIdAsync(5, EntityType.Player))
            .ReturnsAsync(equipment);
        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(5, EntityType.Player))
            .ReturnsAsync("/img/player.png");
        _factory.EquipmentServiceMock
            .Setup(s => s.MapEquipmentToViewModel(equipment, EntityType.Player, "/img/player.png"))
            .Returns(vm);

        var controller = _factory.CreateController();
        var result = await controller.GetById(EntityType.Player, 5);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Details", viewResult.ViewName);
        Assert.Same(vm, viewResult.Model);
    }

    [Fact]
    public async Task GetById_AlbumsTab_LoadsReleasedAlbumsPage()
    {
        var equipment = new Player { Id = 5, Name = "SL-1200" };
        var vm = new EquipmentViewModel { Id = 5, ModelName = "SL-1200", EquipmentType = EntityType.Player };
        var albums = new PagedResult<Album>([], 0, 1, 18);
        _factory.EquipmentServiceMock
            .Setup(s => s.GetByIdAsync(5, EntityType.Player))
            .ReturnsAsync(equipment);
        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(5, EntityType.Player))
            .ReturnsAsync("/img/player.png");
        _factory.EquipmentServiceMock
            .Setup(s => s.MapEquipmentToViewModel(equipment, EntityType.Player, "/img/player.png"))
            .Returns(vm);
        _factory.ReleaseServiceMock
            .Setup(s => s.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, 5, 1, 18))
            .ReturnsAsync(albums);

        var controller = _factory.CreateController();
        var result = await controller.GetById(EntityType.Player, 5, tab: "albums");

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<EquipmentViewModel>(viewResult.Model);
        Assert.Equal("albums", model.ActiveTab);
        Assert.NotNull(model.ReleasedAlbumsPage);
    }

    [Fact]
    public async Task GetById_AlbumsTab_ClampedPageSize()
    {
        var equipment = new Player { Id = 5, Name = "SL-1200" };
        var vm = new EquipmentViewModel { Id = 5, EquipmentType = EntityType.Player };
        _factory.EquipmentServiceMock.Setup(s => s.GetByIdAsync(5, EntityType.Player)).ReturnsAsync(equipment);
        _factory.ImageServiceMock.Setup(s => s.GetUrlAsync(5, EntityType.Player)).ReturnsAsync("/img/x.png");
        _factory.EquipmentServiceMock.Setup(s => s.MapEquipmentToViewModel(It.IsAny<IManufacturer>(), It.IsAny<EntityType>(), It.IsAny<string?>())).Returns(vm);
        _factory.ReleaseServiceMock
            .Setup(s => s.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, 5, 1, 100))
            .ReturnsAsync(new PagedResult<Album>([], 0, 1, 100));

        var controller = _factory.CreateController();
        await controller.GetById(EntityType.Player, 5, tab: "albums", pageSize: 500);

        _factory.ReleaseServiceMock.Verify(
            s => s.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, 5, 1, 100),
            Times.Once);
    }
}
