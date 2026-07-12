using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Enums;
using Web.Response;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

public class SearchControllerInvalidEntityTypeTests
{
    private readonly SearchControllerTestFactory _factory = new();

    [Theory]
    [InlineData("invalid")]
    [InlineData("NotAnEntity")]
    [InlineData("AlbumCoverExtra")]
    [InlineData("UnknownType")]
    public async Task Search_InvalidEntityType_ReturnsBadRequest(string entityType)
    {
        var controller = _factory.CreateController();

        var result = await controller.Search(entityType, "query");

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal($"Invalid entity type: {entityType}", badRequest.Value);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("")]
    public async Task Search_InvalidEntityType_DoesNotCallService(string entityType)
    {
        var controller = _factory.CreateController();

        await controller.Search(entityType, "query");

        _factory.SearchServiceMock.Verify(
            s => s.SearchAsync(It.IsAny<EntityType>(), It.IsAny<string>()),
            Times.Never);
    }
}
