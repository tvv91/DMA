using DMA.Application.Statistics;

namespace Web.Tests.Helpers;

internal static class StatisticServiceTestState
{
    public static void ResetLastRefreshAttempt() => StatisticRefreshGate.ResetLastRefreshAttempt();

    public static void SetLastRefreshAttempt(DateTime value) => StatisticRefreshGate.SetLastRefreshAttempt(value);
}
