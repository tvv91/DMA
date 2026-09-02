namespace DMA.Application.Statistics;

internal static class StatisticRefreshGate
{
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);
    private static DateTime? _lastRefreshAttempt;
    private static readonly TimeSpan RefreshCooldown = TimeSpan.FromMinutes(5);

    public static DateTime? LastRefreshAttempt => _lastRefreshAttempt;

    public static bool CanRefresh(TimeProvider timeProvider) =>
        _lastRefreshAttempt is null ||
        timeProvider.GetUtcNow().UtcDateTime - _lastRefreshAttempt.Value > RefreshCooldown;

    public static void MarkRefreshAttempt(TimeProvider timeProvider) =>
        _lastRefreshAttempt = timeProvider.GetUtcNow().UtcDateTime;

    public static Task WaitAsync(CancellationToken cancellationToken = default) =>
        RefreshLock.WaitAsync(cancellationToken);

    public static void Release() => RefreshLock.Release();

    internal static void ResetLastRefreshAttempt() => _lastRefreshAttempt = null;

    internal static void SetLastRefreshAttempt(DateTime value) => _lastRefreshAttempt = value;
}
