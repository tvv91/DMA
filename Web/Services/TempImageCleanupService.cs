namespace Web.Services
{
    public class TempImageCleanupService(
        IWebHostEnvironment environment,
        IConfiguration configuration,
        TimeProvider timeProvider,
        ILogger<TempImageCleanupService> logger) : BackgroundService
    {
        private readonly IWebHostEnvironment _environment = environment;
        private readonly IConfiguration _configuration = configuration;
        private readonly TimeProvider _timeProvider = timeProvider;
        private readonly ILogger<TempImageCleanupService> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var interval = GetConfiguredTimeSpan("TempImageCleanup:IntervalHours", TimeSpan.FromHours(24));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during temp image cleanup");
                }

                await Task.Delay(interval, stoppingToken);
            }
        }

        private Task CleanupAsync(CancellationToken cancellationToken)
        {
            var tempDirectory = GetSafeTempDirectory();
            if (!Directory.Exists(tempDirectory))
                return Task.CompletedTask;

            var maxAge = GetConfiguredTimeSpan("TempImageCleanup:MaxAgeHours", TimeSpan.FromHours(48));
            var cutoffUtc = _timeProvider.GetUtcNow().UtcDateTime - maxAge;
            var deletedCount = 0;

            foreach (var file in Directory.EnumerateFiles(tempDirectory))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var fullPath = Path.GetFullPath(file);
                EnsurePathInsideDirectory(tempDirectory, fullPath);

                var lastWriteUtc = File.GetLastWriteTimeUtc(fullPath);
                if (lastWriteUtc > cutoffUtc)
                    continue;

                try
                {
                    File.Delete(fullPath);
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    var filename = Path.GetFileName(file);
                    _logger.LogWarning(ex, "Failed to delete stale temp image {Filename}", filename);
                }
            }

            if (deletedCount > 0)
                _logger.LogInformation("Deleted {DeletedCount} stale temp image files", deletedCount);

            return Task.CompletedTask;
        }

        private TimeSpan GetConfiguredTimeSpan(string key, TimeSpan fallback)
        {
            var hours = _configuration.GetValue<double?>(key);
            return hours is > 0 ? TimeSpan.FromHours(hours.Value) : fallback;
        }

        private string GetSafeTempDirectory()
        {
            var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            return Path.GetFullPath(Path.Combine(webRoot, "temp"));
        }

        private static void EnsurePathInsideDirectory(string directory, string path)
        {
            var fullDirectory = Path.GetFullPath(directory);
            var fullPath = Path.GetFullPath(path);

            if (!fullPath.StartsWith(fullDirectory + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Resolved temp image path is outside of the temp directory.");
        }
    }
}
