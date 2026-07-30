using System.Reflection;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Tests.Helpers;

namespace Web.Tests.Services;

[Collection("TempImageCleanupService")]
public class TempImageCleanupServicePathTests : IDisposable
{
    private readonly TempImageCleanupTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Fact]
    public void GetSafeTempDirectory_UsesWebRootPath()
    {
        var directory = _env.GetSafeTempDirectory();

        Assert.Equal(Path.GetFullPath(_env.TempDirectory), directory);
    }

    [Fact]
    public void GetSafeTempDirectory_NullWebRoot_UsesContentRootWwwroot()
    {
        using var env = new TempImageCleanupTestEnvironment(webRootPath: null, createTempDirectory: true);
        Directory.CreateDirectory(env.TempDirectory);

        var directory = env.GetSafeTempDirectory();

        Assert.Equal(Path.GetFullPath(env.TempDirectory), directory);
    }

    [Fact]
    public void EnsurePathInsideDirectory_PathOutsideDirectory_Throws()
    {
        var tempDirectory = Path.GetFullPath(_env.TempDirectory);
        var outsidePath = Path.GetFullPath(Path.Combine(_env.Root, "outside.png"));

        var exception = Assert.Throws<TargetInvocationException>(() =>
            TempImageCleanupTestEnvironment.InvokeEnsurePathInsideDirectory(tempDirectory, outsidePath));

        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public void EnsurePathInsideDirectory_PathInsideDirectory_DoesNotThrow()
    {
        var tempDirectory = Path.GetFullPath(_env.TempDirectory);
        var insidePath = Path.GetFullPath(Path.Combine(tempDirectory, "file.png"));

        TempImageCleanupTestEnvironment.InvokeEnsurePathInsideDirectory(tempDirectory, insidePath);
    }
}
