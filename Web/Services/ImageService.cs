using Microsoft.EntityFrameworkCore.Storage;
using Web.Enums;

namespace Web.Services
{
    internal class ImageService : IImageService
    {
        private const string STORAGE = "wwwroot/";
        private const string NO_COVER = "resources/nocover.png";
        private const string TEMP = $"{STORAGE}/temp";

        private Dictionary<Entity, string> path = new Dictionary<Entity, string> {
            { Entity.AlbumCover, "/covers/"},
            { Entity.VinylState, "/resources/vinylstate/"},
            { Entity.DigitalFormat, "/resources/codec/"},
            { Entity.Bitness, "/resources/bitness/"},
            { Entity.Sampling, "/resources/sampling/"},
            { Entity.SourceFormat, "/resources/sourceformat/"},
            { Entity.Player, "/resources/device/"},
            { Entity.Cartridge, "/resources/cartridge/"},
            { Entity.Amp, "/resources/amp/"},
            { Entity.Adc, "/resources/adc/"},
            { Entity.Wire, "/resources/wire/"},
            { Entity.AlbumDetailCover, "/covers/"},
        };        

        public string GetImageUrl(int id, Entity entity)
        {
            var ext = entity == Entity.AlbumCover || entity == Entity.AlbumDetailCover ? ".jpg" : ".png";
            return File.Exists($"{STORAGE}{path[entity]}{id}{ext}") 
                ? $"{path[entity]}{id}{ext}" 
                : entity == Entity.AlbumCover 
                ? NO_COVER 
                : string.Empty;
        }

        public void RemoveCover(int albumId)
        {
            var file = $"{STORAGE}/covers/{albumId}.jpg";
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch(Exception e)
                {
                    throw;
                }
            }
        }

        // TODO: Need to check image type and convert to jpg (or set filter)
        public void SaveCover(int albumId, string filename)
        {
            try
            {
                var file = $"{TEMP}/{filename}";
                if (File.Exists(file))
                {
                    if (!Directory.Exists($"{STORAGE}/covers"))
                    {
                        Directory.CreateDirectory($"{STORAGE}/covers");
                    }
                    File.Move(file, $"{STORAGE}/covers/{albumId}.jpg", true);
                    // clean temp directory
                    foreach (var f in Directory.GetFiles(TEMP))
                    {
                        File.Delete(f);
                    }
                }
            }
            catch (Exception e)
            {

            }
           
        }
    }
}
