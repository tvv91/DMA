using System.Reflection;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("TempImageCleanupService")]
public class TempImageCleanupServiceExecuteTests : IDisposable
{
    private readonly TempImageCleanupTestEnvironment _env = new(settings: new Dictionary<string, string?>
    {
        ["TempImageCleanup:IntervalHours"] = "0.001",
        ["TempImageCleanup:MaxAgeHours"] = "48",
    });

    public void Dispose() => _env.Dispose();

    [Fact]
    public async Task ExecuteAsync_RunsCleanupThenStopsOnCancellation()
    {
        var stalePath = _env.CreateTempFile("stale.png", _env.UtcNow.AddDays(-5));

        using var cts = new CancellationTokenSource();
        await _env.Service.StartAsync(cts.Token);
        await Task.Delay(200);
        cts.Cancel();
        await _env.Service.StopAsync(CancellationToken.None);

        Assert.False(File.Exists(stalePath));
    }

    [Fact]
    public async Task ExecuteAsync_WhenCleanupCancelledDuringDelay_ExitsWithoutThrowing()
    {
        using var cts = new CancellationTokenSource();
        await _env.Service.StartAsync(cts.Token);
        cts.Cancel();
        var exception = await Record.ExceptionAsync(() => _env.Service.StopAsync(CancellationToken.None));

        Assert.Null(exception);
    }
}
