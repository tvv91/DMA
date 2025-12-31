using Web.Enums;

namespace Web.Services
{
    internal class ImageService : IImageService
    {
        private const string STORAGE = "wwwroot";
        private const string NO_COVER = "resources/nocover.png";
        private const string TEMP = "wwwroot/temp";

        private readonly ILogger<ImageService> _logger;

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

        public ImageService(ILogger<ImageService> logger)
        {
            _logger = logger;
        }

        public async Task<string> GetImageUrlAsync(int id, EntityType entity)
        {
            if (!_map.TryGetValue(entity, out var info))
                return NO_COVER;

            var relativePath = Path.Combine(info.Path, $"{id}{info.Ext}");
            var fullPath = Path.Combine(STORAGE, relativePath);

            return await Task.FromResult(File.Exists(fullPath) ? $"/{relativePath.Replace("\\", "/")}" : $"/{NO_COVER}");
        }

        public async Task RemoveCoverAsync(int id, EntityType entity)
        {
            if (!_map.TryGetValue(entity, out var info))
                return;

            var fullPath = Path.Combine(STORAGE, info.Path, $"{id}{info.Ext}");

            try
            {
                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cover removing {Entity}:{Id}", entity, id);
            }
        }

        public async Task SaveCoverAsync(int id, string filename, EntityType entity)
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
                    await Task.Run(() => File.Move(tempFile, destFile, true));

                    if (File.Exists(tempFile))
                        await Task.Run(() => File.Delete(tempFile));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cover saving {Entity}:{Id}", entity, id);
            }
        }
    }
}
