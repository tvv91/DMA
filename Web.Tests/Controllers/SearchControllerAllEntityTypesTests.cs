using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Response;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class SearchControllerAllEntityTypesTests
{
    private readonly SearchControllerTestFactory _factory = new();

    [Theory]
    [MemberData(nameof(AllEntityTypes))]
    public async Task Search_AllEntityTypeNames_ReturnOk(EntityType entityType)
    {
        _factory.SearchServiceMock
            .Setup(s => s.SearchAsync(entityType, It.IsAny<string>()))
            .ReturnsAsync([]);

        var controller = _factory.CreateController();
        var result = await controller.Search(entityType.ToString(), "q");

        Assert.IsType<OkObjectResult>(result);
        _factory.SearchServiceMock.Verify(
            s => s.SearchAsync(entityType, "q"),
            Times.Once);
    }

    public static IEnumerable<object[]> AllEntityTypes() =>
        Enum.GetValues<EntityType>().Select(t => new object[] { t });
}
