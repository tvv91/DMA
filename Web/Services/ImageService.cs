using Web.Enum;

namespace Web.Services
{
    internal class ImageService : IImageService
    {
        private const string STORAGE = "wwwroot/";
        private const string NO_COVER = "resources/nocover.png";        
        private const string ALBUM_COVERS = $"{STORAGE}/covers";
        private const string VINYL_STATE = $"{STORAGE}/resources/vinylstate";
        private const string DIGITAL_FORMAT = $"{STORAGE}/resources/digitalformat";
        private const string BITNESS = $"{STORAGE}/resources/bitness";
        private const string SAMPLING= $"{STORAGE}/resources/sampling";
        private const string SOURCE_FORMAT = $"{STORAGE}/resources/sourceformat";
        private const string DEVICE = $"{STORAGE}/resources/device";
        private const string CARTRIDGE= $"{STORAGE}/resources/cartridge";
        private const string AMP = $"{STORAGE}/resources/amp";
        private const string ADC = $"{STORAGE}/resources/adc";
        private const string PROCESSING = $"{STORAGE}/resources/processing";

        public string GetImageUrl(int id, EntityType entity)
        {
            switch(entity)
            {
                case EntityType.AlbumCover:
                    return File.Exists($"{ALBUM_COVERS}/{id}.jpg") ? $"/covers/{id}.jpg" : NO_COVER;
                case EntityType.VinylState:
                    return File.Exists($"{VINYL_STATE}/{id}.png") ? $"/resources/vinylstate/{id}.png" : string.Empty;
                case EntityType.DigitalFormat:
                    return File.Exists($"{DIGITAL_FORMAT}/{id}.png") ? $"/resources/digitalformat/{id}.png" : string.Empty;
                case EntityType.Bitness:
                    return File.Exists($"{BITNESS}/{id}.png") ? $"/resources/bitness/{id}.png" : string.Empty;
                case EntityType.Sampling:
                    return File.Exists($"{SAMPLING}/{id}.png") ? $"/resources/sampling/{id}.png" : string.Empty;
                case EntityType.SourceFormat:
                    return File.Exists($"{SOURCE_FORMAT}/{id}.png") ? $"/resources/sourceformat/{id}.png" : string.Empty;
                case EntityType.Device:
                    return File.Exists($"{DEVICE}/{id}.png") ? $"/resources/device/{id}.png" : string.Empty;
                case EntityType.Cartridge:
                    return File.Exists($"{CARTRIDGE}/{id}.png") ? $"/resources/cartridge/{id}.png" : string.Empty;
                case EntityType.Amp:
                    return File.Exists($"{AMP}/{id}.png") ? $"/resources/amp/{id}.png" : string.Empty;
                case EntityType.Adc:
                    return File.Exists($"{ADC}/{id}.png") ? $"/resources/adc/{id}.png" : string.Empty;
                case EntityType.Processing:
                    return File.Exists($"{PROCESSING}/{id}.png") ? $"/resources/processing/{id}.png" : string.Empty;
                case EntityType.AlbumDetailCover:
                    return File.Exists($"{ALBUM_COVERS}/{id}.jpg") ? $"/covers/{id}.jpg" : string.Empty;
                default: 
                    return string.Empty;
            }
            
        }
    }
}
