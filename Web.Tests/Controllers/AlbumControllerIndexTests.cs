using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Common;
using Web.Controllers;
using Web.ViewModels;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class AlbumControllerIndexTests
{
    private readonly AlbumControllerTestFactory _factory = new();

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Index_PageLessThanOne_ReturnsBadRequest(int page)
    {
        var controller = _factory.CreateController();

        var result = await controller.Index(page: page);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Page number should be positive", badRequest.Value);
        _factory.AlbumServiceMock.Verify(
            s => s.GetIndexListAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()),
            Times.Never);
    }

    [Theory]
    [InlineData(0, 15)]
    [InlineData(-5, 15)]
    public async Task Index_PageSizeZeroOrNegative_UsesDefaultPageSize(int pageSize, int expectedPageSize)
    {
        var pagedResult = new PagedResult<Album>([], 0, 1, expectedPageSize);
        _factory.AlbumServiceMock
            .Setup(s => s.GetIndexListAsync(1, expectedPageSize, null, null, null, null))
            .ReturnsAsync(pagedResult);
        _factory.AlbumServiceMock
            .Setup(s => s.HasAnyAlbumsAsync())
            .ReturnsAsync(false);

        var controller = _factory.CreateController();
        var result = await controller.Index(pageSize: pageSize);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName);
        var vm = Assert.IsType<AlbumIndexViewModel>(viewResult.Model);
        Assert.Equal(expectedPageSize, vm.PageSize);
    }

    [Fact]
    public async Task Index_PageSizeAboveMax_ClampedToThirty()
    {
        _factory.AlbumServiceMock
            .Setup(s => s.GetIndexListAsync(1, 30, null, null, null, null))
            .ReturnsAsync(new PagedResult<Album>([], 0, 1, 30));
        _factory.AlbumServiceMock
            .Setup(s => s.HasAnyAlbumsAsync())
            .ReturnsAsync(false);

        var controller = _factory.CreateController();
        var result = await controller.Index(pageSize: 100);

        var vm = Assert.IsType<AlbumIndexViewModel>(Assert.IsType<ViewResult>(result).Model);
        Assert.Equal(30, vm.PageSize);
    }

    [Fact]
    public async Task Index_ValidPageSize_PassedToService()
    {
        _factory.AlbumServiceMock
            .Setup(s => s.GetIndexListAsync(2, 20, null, null, null, null))
            .ReturnsAsync(new PagedResult<Album>([], 0, 2, 20));
        _factory.AlbumServiceMock
            .Setup(s => s.HasAnyAlbumsAsync())
            .ReturnsAsync(true);

        var controller = _factory.CreateController();
        await controller.Index(page: 2, pageSize: 20);

        _factory.AlbumServiceMock.Verify(
            s => s.GetIndexListAsync(2, 20, null, null, null, null),
            Times.Once);
    }

    [Fact]
    public async Task Index_ReturnsViewWithMappedViewModel()
    {
        var albums = new List<Album> { new() { Id = 1, Title = "Album One" } };
        _factory.AlbumServiceMock
            .Setup(s => s.GetIndexListAsync(1, 15, "Artist", "Genre", "1973", "Title"))
            .ReturnsAsync(new PagedResult<Album>(albums, 1, 1, 15));
        _factory.AlbumServiceMock
            .Setup(s => s.HasAnyAlbumsAsync())
            .ReturnsAsync(true);

        var controller = _factory.CreateController();
        var result = await controller.Index(
            artistName: "Artist",
            genreName: "Genre",
            yearValue: "1973",
            albumTitle: "Title");

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName);
        var vm = Assert.IsType<AlbumIndexViewModel>(viewResult.Model);
        Assert.Equal(1, vm.CurrentPage);
        Assert.Equal(15, vm.PageSize);
        Assert.True(vm.HasAnyAlbumsInDb);
        Assert.Equal("Artist", vm.ArtistName);
        Assert.Equal("Genre", vm.GenreName);
        Assert.Equal("1973", vm.YearValue);
        Assert.Equal("Title", vm.AlbumTitle);
        Assert.Same(albums, vm.Albums);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Index_NullOrEmptyFilters_PassedToService(string? filter)
    {
        _factory.AlbumServiceMock
            .Setup(s => s.GetIndexListAsync(1, 15, filter, filter, filter, filter))
            .ReturnsAsync(new PagedResult<Album>([], 0, 1, 15));
        _factory.AlbumServiceMock
            .Setup(s => s.HasAnyAlbumsAsync())
            .ReturnsAsync(false);

        var controller = _factory.CreateController();
        await controller.Index(artistName: filter, genreName: filter, yearValue: filter, albumTitle: filter);

        _factory.AlbumServiceMock.Verify(
            s => s.GetIndexListAsync(1, 15, filter, filter, filter, filter),
            Times.Once);
    }
}
