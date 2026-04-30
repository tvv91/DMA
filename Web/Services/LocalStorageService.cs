using Web.Enums;
using Web.Interfaces;

namespace Web.Services
{
    public class LocalStorageService(ILogger<LocalStorageService> logger) : IImageService
    {
        private const string STORAGE = "wwwroot";
        private const string NO_COVER = "resources/nocover.png";
        private const string TEMP = "wwwroot/temp";

        private readonly ILogger<LocalStorageService> _logger = logger;

        private readonly Dictionary<EntityType, (string Path, string Ext)> _map = new()
            {
                { EntityType.AlbumCover, ("covers/album", ".jpg") },
                { EntityType.VinylState, ("resources/vinylstate", ".png") },
                { EntityType.DigitalFormat, ("resources/codec", ".png") },
                { EntityType.Bitness, ("resources/bitness", ".png") },
                { EntityType.Sampling, ("resources/sampling", ".png") },
                { EntityType.SourceFormat, ("resources/sourceformat", ".png") },
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
            var fullPath = Path.Combine(STORAGE, relativePath);
            // Yeah, this code is sync, but when we change to cloud blobk storage, it will be async, so let's keep the signature async for now
            return await Task.FromResult(File.Exists(fullPath) ? $"/{relativePath.Replace("\\", "/")}" : $"/{NO_COVER}");
        }

        public async Task RemoveAsync(int id, EntityType entity)
        {
            if (!_map.TryGetValue(entity, out var info))
                return;

            var fullPath = Path.Combine(STORAGE, info.Path, $"{id}{info.Ext}");

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

            try
            {
                var tempFile = Path.Combine(TEMP, filename);
                if (File.Exists(tempFile))
                {
                    var targetDir = Path.Combine(STORAGE, info.Path);
                    if (!Directory.Exists(targetDir))
                        Directory.CreateDirectory(targetDir);

                    var destFile = Path.Combine(targetDir, $"{id}{info.Ext}");

                    await using var source = new FileStream(
                        tempFile,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        bufferSize: 81920,
                        useAsync: true);

                    await using var destination = new FileStream(
                        destFile,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None,
                        bufferSize: 81920,
                        useAsync: true);

                    await source.CopyToAsync(destination);
                    await destination.FlushAsync();

                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cover saving {Entity}:{Id}", entity, id);
            }
        }
    }
}
