using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Services;

namespace Web.Tests.Helpers;

internal sealed class TempImageCleanupTestEnvironment : IDisposable
{
    private static readonly DateTimeOffset DefaultUtcNow = new(2024, 9, 10, 12, 0, 0, TimeSpan.Zero);

    public string Root { get; }
    public string WebRoot { get; }
    public string TempDirectory { get; }
    public FakeTimeProvider TimeProvider { get; }
    public Mock<ILogger<TempImageCleanupService>> LoggerMock { get; } = new();
    public TempImageCleanupService Service { get; }

    public DateTime UtcNow => TimeProvider.GetUtcNow().UtcDateTime;

    public TempImageCleanupTestEnvironment(
        DateTimeOffset? utcNow = null,
        string? webRootPath = null,
        IReadOnlyDictionary<string, string?>? settings = null,
        bool createTempDirectory = true)
    {
        Root = Path.Combine(Path.GetTempPath(), "dma-temp-cleanup-tests", Guid.NewGuid().ToString("N"));
        WebRoot = webRootPath ?? Path.Combine(Root, "wwwroot");
        TempDirectory = Path.Combine(WebRoot, "temp");

        if (createTempDirectory)
            Directory.CreateDirectory(TempDirectory);

        TimeProvider = new FakeTimeProvider(utcNow ?? DefaultUtcNow);

        var environmentMock = new Mock<IWebHostEnvironment>();
        environmentMock.Setup(e => e.WebRootPath).Returns(webRootPath!);
        environmentMock.Setup(e => e.ContentRootPath).Returns(Root);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings ?? new Dictionary<string, string?>())
            .Build();

        Service = new TempImageCleanupService(
            environmentMock.Object,
            configuration,
            TimeProvider,
            LoggerMock.Object);
    }

    public string CreateTempFile(string name, DateTime lastWriteUtc)
    {
        var path = Path.Combine(TempDirectory, name);
        File.WriteAllBytes(path, [0x89, 0x50, 0x4E, 0x47]);
        File.SetLastWriteTimeUtc(path, lastWriteUtc);
        return path;
    }

    public async Task RunCleanupAsync(CancellationToken cancellationToken = default)
    {
        var method = typeof(TempImageCleanupService).GetMethod(
            "CleanupAsync",
            BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("CleanupAsync not found.");

        try
        {
            await (Task)method.Invoke(Service, [cancellationToken])!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            throw ex.InnerException;
        }
    }

    public string GetSafeTempDirectory()
    {
        var method = typeof(TempImageCleanupService).GetMethod(
            "GetSafeTempDirectory",
            BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("GetSafeTempDirectory not found.");

        return (string)method.Invoke(Service, null)!;
    }

    public static void InvokeEnsurePathInsideDirectory(string directory, string path)
    {
        var method = typeof(TempImageCleanupService).GetMethod(
            "EnsurePathInsideDirectory",
            BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("EnsurePathInsideDirectory not found.");

        method.Invoke(null, [directory, path]);
    }

    public void Dispose()
    {
        if (Directory.Exists(Root))
            Directory.Delete(Root, recursive: true);
    }
}
