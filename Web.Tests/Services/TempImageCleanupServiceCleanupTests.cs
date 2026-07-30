using System.Reflection;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("TempImageCleanupService")]
public class TempImageCleanupServiceCleanupTests : IDisposable
{
    private readonly TempImageCleanupTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Fact]
    public async Task CleanupAsync_NoTempDirectory_DoesNothing()
    {
        using var env = new TempImageCleanupTestEnvironment(createTempDirectory: false);

        await env.RunCleanupAsync();

        Assert.False(Directory.Exists(env.TempDirectory));
    }

    [Fact]
    public async Task CleanupAsync_EmptyDirectory_DoesNothing()
    {
        await _env.RunCleanupAsync();

        Assert.Empty(Directory.GetFiles(_env.TempDirectory));
    }

    [Fact]
    public async Task CleanupAsync_StaleFile_DeletesFile()
    {
        var stalePath = _env.CreateTempFile("stale.png", _env.UtcNow.AddHours(-49));

        await _env.RunCleanupAsync();

        Assert.False(File.Exists(stalePath));
    }

    [Fact]
    public async Task CleanupAsync_FreshFile_KeepsFile()
    {
        var freshPath = _env.CreateTempFile("fresh.png", _env.UtcNow.AddHours(-1));

        await _env.RunCleanupAsync();

        Assert.True(File.Exists(freshPath));
    }

    [Fact]
    public async Task CleanupAsync_MixedFiles_DeletesOnlyStale()
    {
        var stalePath = _env.CreateTempFile("old.png", _env.UtcNow.AddDays(-3));
        var freshPath = _env.CreateTempFile("new.png", _env.UtcNow.AddMinutes(-30));

        await _env.RunCleanupAsync();

        Assert.False(File.Exists(stalePath));
        Assert.True(File.Exists(freshPath));
    }

    [Fact]
    public async Task CleanupAsync_CustomMaxAge_UsesConfiguration()
    {
        using var env = new TempImageCleanupTestEnvironment(settings: new Dictionary<string, string?>
        {
            ["TempImageCleanup:MaxAgeHours"] = "1",
        });

        var stalePath = env.CreateTempFile("two-hours.png", env.UtcNow.AddHours(-2));
        var freshPath = env.CreateTempFile("thirty-min.png", env.UtcNow.AddMinutes(-30));

        await env.RunCleanupAsync();

        Assert.False(File.Exists(stalePath));
        Assert.True(File.Exists(freshPath));
    }

    [Fact]
    public async Task CleanupAsync_InvalidMaxAge_UsesDefault48Hours()
    {
        using var env = new TempImageCleanupTestEnvironment(settings: new Dictionary<string, string?>
        {
            ["TempImageCleanup:MaxAgeHours"] = "0",
        });

        var borderlineFreshPath = env.CreateTempFile("47-hours.png", env.UtcNow.AddHours(-47));

        await env.RunCleanupAsync();

        Assert.True(File.Exists(borderlineFreshPath));
    }

    [Fact]
    public async Task CleanupAsync_DeletedFiles_LogsInformation()
    {
        _env.CreateTempFile("stale.png", _env.UtcNow.AddDays(-5));

        await _env.RunCleanupAsync();

        _env.LoggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((value, _) => value.ToString()!.Contains("Deleted")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task CleanupAsync_NoDeletions_DoesNotLogInformation()
    {
        _env.CreateTempFile("fresh.png", _env.UtcNow);

        await _env.RunCleanupAsync();

        _env.LoggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }

    [Fact]
    public async Task CleanupAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        _env.CreateTempFile("stale-1.png", _env.UtcNow.AddDays(-5));
        _env.CreateTempFile("stale-2.png", _env.UtcNow.AddDays(-5));

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => _env.RunCleanupAsync(cts.Token));
    }

    [Fact]
    public async Task CleanupAsync_LockedFile_LogsWarningAndKeepsProcessing()
    {
        var lockedPath = _env.CreateTempFile("locked.png", _env.UtcNow.AddDays(-5));
        var deletablePath = _env.CreateTempFile("deletable.png", _env.UtcNow.AddDays(-5));

        using var stream = new FileStream(lockedPath, FileMode.Open, FileAccess.Read, FileShare.None);

        await _env.RunCleanupAsync();

        Assert.True(File.Exists(lockedPath));
        Assert.False(File.Exists(deletablePath));
        _env.LoggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((value, _) => value.ToString()!.Contains("locked.png")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
