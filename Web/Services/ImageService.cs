using Microsoft.EntityFrameworkCore.Storage;
using Web.Enum;

namespace Web.Services
{
    internal class ImageService : IImageService
    {
        private const string STORAGE = "wwwroot/";
        private const string NO_COVER = "resources/nocover.png";
        private const string TEMP = $"{STORAGE}/temp";

        private Dictionary<EntityType, string> path = new Dictionary<EntityType, string> {
            { EntityType.AlbumCover, $"/covers/"},
            { EntityType.VinylState, "/resources/vinylstate/"},
            { EntityType.DigitalFormat, "/resources/codec/"},
            { EntityType.Bitness, "/resources/bitness/"},
            { EntityType.Sampling, "/resources/sampling/"},
            { EntityType.SourceFormat, "/resources/sourceformat/"},
            { EntityType.Device, "/resources/device/"},
            { EntityType.Cartridge, "/resources/cartridge/"},
            { EntityType.Amp, "/resources/amp/"},
            { EntityType.Adc, "/resources/adc/"},
            { EntityType.Processing, "/resources/Processing/"},
            { EntityType.AlbumDetailCover, "/covers/"},
        };        

        public string GetImageUrl(int id, EntityType entity)
        {
            var ext = entity == EntityType.AlbumCover || entity == EntityType.AlbumDetailCover ? ".jpg" : ".png";
            return File.Exists($"{STORAGE}{path[entity]}{id}{ext}") 
                ? $"{path[entity]}{id}{ext}" 
                : entity == EntityType.AlbumCover 
                ? NO_COVER 
                : string.Empty;
        }

        // TODO: Need to check image type and convert to jpg (or set filter)
        public void SaveCover(int albumId, string filename)
        {
            var file = $"{TEMP}/{filename}";
            if (File.Exists(file))
            {
                File.Move(file, $"{STORAGE}/covers/{albumId}.jpg");
                // clean temp directory
                foreach (var f in Directory.GetFiles(TEMP))
                {
                    File.Delete(f);
                }
            }
        }
    }
}
