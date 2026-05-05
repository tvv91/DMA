using Web.Enums;
using Web.Interfaces;

namespace Web.Services
{
    public class LocalStorageImageService(
        IWebHostEnvironment environment,
        ILogger<LocalStorageImageService> logger) : IImageService
    {
        private const string NO_COVER = "resources/nocover.png";
        private static readonly HashSet<string> AllowedTempExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".png"
        };

        private readonly IWebHostEnvironment _environment = environment;
        private readonly ILogger<LocalStorageImageService> _logger = logger;

        private readonly Dictionary<EntityType, (string Path, string Ext)> _map = new()
            {
                { EntityType.AlbumCover, ("covers/album", ".jpg") },
                { EntityType.Player, ("covers/player", ".jpg") },
                { EntityType.Cartridge, ("covers/cartridge", ".jpg") },
                { EntityType.Amplifier, ("covers/amp", ".jpg") },
                { EntityType.Adc, ("covers/adc", ".jpg") },
                { EntityType.Wire, ("covers/wire", ".jpg") },
            };

        public async Task<string> GetUrlAsync(int id, EntityType entity)
        {
            if (!_map.TryGetValue(entity, out var info))
                return NO_COVER;

            var relativePath = Path.Combine(info.Path, $"{id}{info.Ext}");
            var fullPath = Path.Combine(GetWebRootPath(), relativePath);
            // Yeah, this code is sync, but when we change to cloud blobk storage, it will be async, so let's keep the signature async for now
            return await Task.FromResult(File.Exists(fullPath) ? $"/{relativePath.Replace("\\", "/")}" : $"/{NO_COVER}");
        }

        public async Task RemoveAsync(int id, EntityType entity)
        {
            if (!_map.TryGetValue(entity, out var info))
                return;

            var fullPath = GetSafeStorageFilePath(info.Path, $"{id}{info.Ext}");

            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cover removing {Entity}:{Id}", entity, id);
            }
        }

        public async Task SaveAsync(int id, string filename, EntityType entity)
        {
            if (!_map.TryGetValue(entity, out var info))
                return;

            var safeFilename = GetSafeTempFilename(filename);
            if (safeFilename is null)
            {
                _logger.LogWarning("Rejected invalid temp image filename {Filename} for {Entity}:{Id}", filename, entity, id);
                return;
            }

            try
            {
                var tempFile = GetSafeTempFilePath(safeFilename);
                if (File.Exists(tempFile))
                {
                    var targetDir = GetSafeStorageDirectory(info.Path);
                    if (!Directory.Exists(targetDir))
                        Directory.CreateDirectory(targetDir);

                    var destFile = GetSafeStorageFilePath(info.Path, $"{id}{info.Ext}");

                    await using (var source = new FileStream(
                        tempFile,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        bufferSize: 81920,
                        useAsync: true))
                    await using (var destination = new FileStream(
                        destFile,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None,
                        bufferSize: 81920,
                        useAsync: true))
                    {
                        await source.CopyToAsync(destination);
                        await destination.FlushAsync();
                    }

                    File.Delete(tempFile);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cover saving {Entity}:{Id}", entity, id);
            }
        }

        private string GetWebRootPath()
        {
            return Path.GetFullPath(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"));
        }

        private string GetSafeTempDirectory()
        {
            return Path.GetFullPath(Path.Combine(GetWebRootPath(), "temp"));
        }

        private string GetSafeTempFilePath(string filename)
        {
            var fullTempDirectory = GetSafeTempDirectory();
            var fullPath = Path.GetFullPath(Path.Combine(fullTempDirectory, filename));
            EnsurePathInsideDirectory(fullTempDirectory, fullPath);
            return fullPath;
        }

        private string GetSafeStorageDirectory(string relativePath)
        {
            var fullWebRoot = GetWebRootPath();
            var fullPath = Path.GetFullPath(Path.Combine(fullWebRoot, relativePath));
            EnsurePathInsideDirectory(fullWebRoot, fullPath);
            return fullPath;
        }

        private string GetSafeStorageFilePath(string relativePath, string filename)
        {
            var directory = GetSafeStorageDirectory(relativePath);
            var fullPath = Path.GetFullPath(Path.Combine(directory, Path.GetFileName(filename)));
            EnsurePathInsideDirectory(directory, fullPath);
            return fullPath;
        }

        private static string? GetSafeTempFilename(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
                return null;

            var safeFilename = Path.GetFileName(filename);
            if (!string.Equals(filename, safeFilename, StringComparison.Ordinal))
                return null;

            var extension = Path.GetExtension(safeFilename);
            if (!AllowedTempExtensions.Contains(extension))
                return null;

            var nameWithoutExtension = Path.GetFileNameWithoutExtension(safeFilename);
            return Guid.TryParseExact(nameWithoutExtension, "N", out _) ? safeFilename : null;
        }

        private static void EnsurePathInsideDirectory(string directory, string path)
        {
            var fullDirectory = Path.GetFullPath(directory);
            var fullPath = Path.GetFullPath(path);

            if (string.Equals(fullDirectory, fullPath, StringComparison.OrdinalIgnoreCase))
                return;

            if (!fullPath.StartsWith(fullDirectory + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Resolved image path is outside of the expected storage directory.");
        }
    }
}
