using Web.Enums;
using Web.Infrastructure.Icons;

namespace Web.Infrastructure.Icons
{
    public class LocalResourceIconService : IResourceIconService
    {
        private const string Storage = "wwwroot";
        private const string NoCover = "resources/nocover.png";

        private readonly Dictionary<EntityType, (string Path, string Ext)> _map = new()
            {
                { EntityType.VinylState, ("resources/vinylstate", ".png") },
                { EntityType.DigitalFormat, ("resources/codec", ".png") },
                { EntityType.Bitness, ("resources/bitness", ".png") },
                { EntityType.Sampling, ("resources/sampling", ".png") },
                { EntityType.SourceFormat, ("resources/sourceformat", ".png") },
            };

        public Task<string> GetIconUrlAsync(int id, EntityType entity)
        {
            if (!_map.TryGetValue(entity, out var info))
                return Task.FromResult($"/{NoCover}");

            var relativePath = Path.Combine(info.Path, $"{id}{info.Ext}");
            var fullPath = Path.Combine(Storage, relativePath);

            var url = File.Exists(fullPath) ? $"/{relativePath.Replace("\\", "/")}" : $"/{NoCover}";
            return Task.FromResult(url);
        }
    }
}
