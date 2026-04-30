using Web.Enums;
using Web.Interfaces;

namespace Web.Services
{
    public class LocalResourceIconService : IResourceIconService
    {
        private const string STORAGE = "wwwroot";
        private const string NO_COVER = "resources/nocover.png";

        private readonly Dictionary<EntityType, (string Path, string Ext)> _map = new()
            {
                { EntityType.VinylState, ("resources/vinylstate", ".png") },
                { EntityType.DigitalFormat, ("resources/codec", ".png") },
                { EntityType.Bitness, ("resources/bitness", ".png") },
                { EntityType.Sampling, ("resources/sampling", ".png") },
                { EntityType.SourceFormat, ("resources/sourceformat", ".png") },
            };

        public async Task<string> GetIconUrlAsync(int id, EntityType entity)
        {
            if (!_map.TryGetValue(entity, out var info))
                return $"/{NO_COVER}";

            var relativePath = Path.Combine(info.Path, $"{id}{info.Ext}");
            var fullPath = Path.Combine(STORAGE, relativePath);

            return await Task.FromResult(File.Exists(fullPath) ? $"/{relativePath.Replace("\\", "/")}" : $"/{NO_COVER}");
        }
    }
}
