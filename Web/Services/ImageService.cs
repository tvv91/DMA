using Web.Enums;

namespace Web.Services
{
    internal class ImageService : IImageService
    {
        private const string STORAGE = "wwwroot/";
        private const string NO_COVER = "/resources/nocover.png";
        private const string TEMP = $"{STORAGE}/temp";

        private Dictionary<EntityType, string> path = new Dictionary<EntityType, string> {
            { EntityType.AlbumCover, "/covers/album/"},
            { EntityType.VinylState, "/resources/vinylstate/"},
            { EntityType.DigitalFormat, "/resources/codec/"},
            { EntityType.Bitness, "/resources/bitness/"},
            { EntityType.Sampling, "/resources/sampling/"},
            { EntityType.SourceFormat, "/resources/sourceformat/"},
            { EntityType.Player, "/covers//player/"},
            { EntityType.Cartridge, "/covers/cartridge/"},
            { EntityType.Amplifier, "/covers/amp/"},
            { EntityType.Adc, "/covers/adc/"},
            { EntityType.Wire, "/covers/wire/"},
        };

        public string GetIconUrl(int id, EntityType entity)
        {
            return File.Exists($"{STORAGE}{path[entity]}{id}.png") ? $"{path[entity]}{id}.png" : string.Empty;
        }

        public string GetImageUrl(int id, EntityType entity)
        {
            switch (entity)
            {
                case EntityType.Amplifier:
                case EntityType.Adc:
                case EntityType.Wire:
                case EntityType.Cartridge:
                case EntityType.Player:
                    return File.Exists($"{STORAGE}{path[entity]}{id}.jpg") ? $"{path[entity]}{id}.jpg" : string.Empty;
                default:
                    return File.Exists($"{STORAGE}{path[entity]}{id}.jpg") ? $"{path[entity]}{id}.jpg" : NO_COVER;
            }
        }

        public void RemoveCover(int id, EntityType entity)
        {
            var file = $"{STORAGE}/{path[entity]}{id}.jpg";
            if (File.Exists(file))
            {
                try
                {
                    File.Delete(file);
                }
                catch(Exception e)
                {
                    // TODO: Add logging
                }
            }
        }

        // TODO: Need to check image type and convert to jpg (or set filter)
        public void SaveCover(int id, string filename, EntityType entity)
        {
            try
            {
                var file = $"{TEMP}/{filename}";
                if (File.Exists(file))
                {
                    var _path = $"{STORAGE}/{path[entity]}";
                    if (!Directory.Exists(_path))
                    {
                        Directory.CreateDirectory(_path);
                    }
                    File.Move(file, $"{_path}{id}.jpg", true);
                    // clean temp directory
                    foreach (var f in Directory.GetFiles(TEMP))
                    {
                        File.Delete(f);
                    }
                }
            }
            catch (Exception e)
            {
                // TODO: Add logging
            }
        }
    }
}
