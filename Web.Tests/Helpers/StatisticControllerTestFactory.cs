using Moq;
using Web.Controllers;
using Web.Interfaces;

namespace Web.Tests.Helpers;

internal sealed class StatisticControllerTestFactory
{
    public Mock<IStatisticService> StatisticServiceMock { get; } = new();

    public StatisticController CreateController() =>
        new(StatisticServiceMock.Object);
}
