using System.Reflection;
using Web.Services;

namespace Web.Tests.Helpers;

internal static class StatisticServiceTestState
{
    public static void ResetLastRefreshAttempt()
    {
        var field = typeof(StatisticService).GetField(
            "_lastRefreshAttempt",
            BindingFlags.Static | BindingFlags.NonPublic);

        field?.SetValue(null, null);
    }

    public static void SetLastRefreshAttempt(DateTime value)
    {
        var field = typeof(StatisticService).GetField(
            "_lastRefreshAttempt",
            BindingFlags.Static | BindingFlags.NonPublic);

        field?.SetValue(null, value);
    }
}
