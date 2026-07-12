using Moq;
using Web.Controllers;
using Web.Interfaces;

namespace Web.Tests.Helpers;

internal sealed class SearchControllerTestFactory
{
    public Mock<ISearchService> SearchServiceMock { get; } = new();

    public SearchController CreateController() =>
        new(SearchServiceMock.Object);
}
