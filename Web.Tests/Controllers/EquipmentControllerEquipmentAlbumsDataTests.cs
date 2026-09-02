using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Interfaces;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class EquipmentControllerEquipmentAlbumsDataTests
{
    private readonly EquipmentControllerTestFactory _factory = new();

    [Fact]
    public async Task EquipmentAlbumsData_InvalidId_ReturnsBadRequest()
    {
        var controller = _factory.CreateController();

        var result = await controller.EquipmentAlbumsData(EntityType.Player, 0);

        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task EquipmentAlbumsData_EquipmentNotFound_ReturnsNotFound()
    {
        _factory.EquipmentServiceMock
            .Setup(s => s.GetByIdAsync(10, EntityType.Wire))
            .ReturnsAsync((IManufacturer?)null);

        var controller = _factory.CreateController();
        var result = await controller.EquipmentAlbumsData(EntityType.Wire, 10);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task EquipmentAlbumsData_ValidRequest_ReturnsPartialView()
    {
        var equipment = new Wire { Id = 10, Name = "Cable" };
        var albums = new PagedResult<Album>(
            [new Album { Id = 1, Title = "Album One", Artist = new Artist { Name = "Artist" } }],
            1, 1, 18);
        _factory.EquipmentServiceMock
            .Setup(s => s.GetByIdAsync(10, EntityType.Wire))
            .ReturnsAsync(equipment);
        _factory.ReleaseServiceMock
            .Setup(s => s.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Wire, 10, 2, 18))
            .ReturnsAsync(albums);

        var controller = _factory.CreateController();
        var result = await controller.EquipmentAlbumsData(EntityType.Wire, 10, page: 2);

        var partial = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("_EquipmentReleasedAlbumsInner", partial.ViewName);
        var vm = Assert.IsType<EquipmentReleasedAlbumsPageViewModel>(partial.Model);
        Assert.Equal(10, vm.EquipmentId);
        Assert.Equal("wire", vm.CategorySegment);
        Assert.True(vm.HasResults);
        Assert.Single(vm.Albums);
    }

    [Theory]
    [InlineData(0, 18)]
    [InlineData(-5, 18)]
    public async Task EquipmentAlbumsData_InvalidPageSize_UsesDefault(int pageSize, int expectedPageSize)
    {
        var equipment = new Player { Id = 3, Name = "P" };
        _factory.EquipmentServiceMock.Setup(s => s.GetByIdAsync(3, EntityType.Player)).ReturnsAsync(equipment);
        _factory.ReleaseServiceMock
            .Setup(s => s.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, 3, 1, expectedPageSize))
            .ReturnsAsync(new PagedResult<Album>([], 0, 1, expectedPageSize));

        var controller = _factory.CreateController();
        await controller.EquipmentAlbumsData(EntityType.Player, 3, pageSize: pageSize);

        _factory.ReleaseServiceMock.Verify(
            s => s.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, 3, 1, expectedPageSize),
            Times.Once);
    }

    [Fact]
    public async Task EquipmentAlbumsData_PageSizeAboveMax_ClampedToHundred()
    {
        var equipment = new Player { Id = 3, Name = "P" };
        _factory.EquipmentServiceMock.Setup(s => s.GetByIdAsync(3, EntityType.Player)).ReturnsAsync(equipment);
        _factory.ReleaseServiceMock
            .Setup(s => s.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, 3, 1, 100))
            .ReturnsAsync(new PagedResult<Album>([], 0, 1, 100));

        var controller = _factory.CreateController();
        await controller.EquipmentAlbumsData(EntityType.Player, 3, pageSize: 250);

        _factory.ReleaseServiceMock.Verify(
            s => s.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, 3, 1, 100),
            Times.Once);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-2, 1)]
    public async Task EquipmentAlbumsData_InvalidPage_NormalizedToOne(int page, int expectedPage)
    {
        var equipment = new Player { Id = 3, Name = "P" };
        _factory.EquipmentServiceMock.Setup(s => s.GetByIdAsync(3, EntityType.Player)).ReturnsAsync(equipment);
        _factory.ReleaseServiceMock
            .Setup(s => s.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, 3, expectedPage, 18))
            .ReturnsAsync(new PagedResult<Album>([], 0, expectedPage, 18));

        var controller = _factory.CreateController();
        await controller.EquipmentAlbumsData(EntityType.Player, 3, page: page);

        _factory.ReleaseServiceMock.Verify(
            s => s.GetAlbumsReleasedByEquipmentPagedAsync(EntityType.Player, 3, expectedPage, 18),
            Times.Once);
    }
}
