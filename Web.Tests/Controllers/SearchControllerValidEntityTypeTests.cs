using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Enums;
using Web.Response;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class SearchControllerValidEntityTypeTests
{
    private readonly SearchControllerTestFactory _factory = new();

    [Theory]
    [InlineData("Artist", EntityType.Artist)]
    [InlineData("Genre", EntityType.Genre)]
    [InlineData("Player", EntityType.Player)]
    [InlineData("VinylState", EntityType.VinylState)]
    [InlineData("PlayerManufacturer", EntityType.PlayerManufacturer)]
    public async Task Search_ValidEntityType_ReturnsOkWithResults(
        string routeEntityType,
        EntityType expectedEntityType)
    {
        var expected = new List<AutocompleteResponse>
        {
            new() { Label = "Pink Floyd", Value = "Pink Floyd" },
        };
        _factory.SearchServiceMock
            .Setup(s => s.SearchAsync(expectedEntityType, "pink"))
            .ReturnsAsync(expected);

        var controller = _factory.CreateController();
        var result = await controller.Search(routeEntityType, "pink");

        var ok = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<List<AutocompleteResponse>>(ok.Value);
        Assert.Single(actual);
        Assert.Equal("Pink Floyd", actual[0].Label);
    }

    [Theory]
    [InlineData("artist", EntityType.Artist)]
    [InlineData("GENRE", EntityType.Genre)]
    [InlineData("vinylstate", EntityType.VinylState)]
    public async Task Search_EntityTypeCaseInsensitive_ParsesAndCallsService(
        string routeEntityType,
        EntityType expectedEntityType)
    {
        _factory.SearchServiceMock
            .Setup(s => s.SearchAsync(expectedEntityType, It.IsAny<string>()))
            .ReturnsAsync([]);

        var controller = _factory.CreateController();
        var result = await controller.Search(routeEntityType, null);

        Assert.IsType<OkObjectResult>(result);
        _factory.SearchServiceMock.Verify(
            s => s.SearchAsync(expectedEntityType, ""),
            Times.Once);
    }

    [Fact]
    public async Task Search_ServiceReturnsEmpty_ReturnsOkWithEmptyList()
    {
        _factory.SearchServiceMock
            .Setup(s => s.SearchAsync(EntityType.Country, "zzz"))
            .ReturnsAsync([]);

        var controller = _factory.CreateController();
        var result = await controller.Search("Country", "zzz");

        var ok = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<List<AutocompleteResponse>>(ok.Value);
        Assert.Empty(actual);
    }
}
