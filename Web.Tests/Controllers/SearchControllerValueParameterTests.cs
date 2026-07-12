using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Enums;
using Web.Response;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class SearchControllerValueParameterTests
{
    private readonly SearchControllerTestFactory _factory = new();

    [Fact]
    public async Task Search_NullValue_PassesEmptyStringToService()
    {
        _factory.SearchServiceMock
            .Setup(s => s.SearchAsync(EntityType.Label, ""))
            .ReturnsAsync([]);

        var controller = _factory.CreateController();
        await controller.Search("Label", null);

        _factory.SearchServiceMock.Verify(
            s => s.SearchAsync(EntityType.Label, ""),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Search_BlankValue_PassesValueToService(string value)
    {
        _factory.SearchServiceMock
            .Setup(s => s.SearchAsync(EntityType.Storage, value))
            .ReturnsAsync([]);

        var controller = _factory.CreateController();
        await controller.Search("Storage", value);

        _factory.SearchServiceMock.Verify(
            s => s.SearchAsync(EntityType.Storage, value),
            Times.Once);
    }

    [Fact]
    public async Task Search_ExplicitValue_PassesValueToService()
    {
        _factory.SearchServiceMock
            .Setup(s => s.SearchAsync(EntityType.Adc, "RME"))
            .ReturnsAsync([new AutocompleteResponse { Label = "RME ADI-2", Value = "RME ADI-2" }]);

        var controller = _factory.CreateController();
        var result = await controller.Search("Adc", "RME");

        Assert.IsType<OkObjectResult>(result);
        _factory.SearchServiceMock.Verify(
            s => s.SearchAsync(EntityType.Adc, "RME"),
            Times.Once);
    }
}
