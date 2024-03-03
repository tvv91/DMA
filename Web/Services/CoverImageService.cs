namespace Web.Services
{
    internal class CoverImageService : ICoverImageService
    {
        private const string NO_COVER = "resources/nocover.png";
        private const string BASE_STORAGE = "wwwroot/covers";

        public string GetImageUrl(int albumId)
        {
            return File.Exists($"{BASE_STORAGE}/{albumId}.jpg") ? $"covers/{albumId}.jpg" : NO_COVER;
        }
    }
}
