namespace Web.Services
{
    internal class CoverImageService : ICoverImageService
    {
        private const string NO_COVER = "resources/nocover.png";
        
        /// <summary>
        /// TODO: Just for test
        /// </summary>
        private Dictionary<int, string> storage = new Dictionary<int, string>
        {
            { 1, "https://content.discogs.com/media/green-day-american-idiot-300x298.jpg" },
            { 2, "https://content.discogs.com/media/MF-Doom-MM-Food-300x298.jpeg" },
            { 3, "https://content.discogs.com/media/Funeral-Arcade-Fire-300x300.jpeg" },
            { 4, "https://content.discogs.com/media/Modest-Mouse-%E2%80%93-Good-News-For-People-Who-Love-Bad-News-300x300.jpeg" },
            { 5, "https://content.discogs.com/media/Erlend-Oye-%E2%80%93-DJ-Kicks-300x298.jpeg" },
            { 6, "https://content.discogs.com/media/Album-Cover-Kanye-West-The-College-Dropout-300x300.jpg" },
            { 7, "https://content.discogs.com/media/TV-On-The-Radio-%E2%80%93-Desperate-Youth-Blood-Thirsty-Babes-300x300.jpeg" },
            { 8, "https://content.discogs.com/media/Yard-Act-%E2%80%8E%E2%80%93-Wheres-My-Utopia-300x300.jpeg" },
            { 9, "https://content.discogs.com/media/Amaro-Freitas-%E2%80%8E%E2%80%93-YY-300x300.jpeg" },
            { 10, "https://content.discogs.com/media/Pissed-Jeans-%E2%80%8E%E2%80%93-Half-Divorced-300x300.jpeg" },
            { 11, "https://content.discogs.com/media/Mary-Timony-%E2%80%8E%E2%80%93-Untame-The-Tiger-300x300.jpeg" },
            { 12, "https://content.discogs.com/media/Chromeo-%E2%80%8E%E2%80%93-Adult-Contemporary-300x285.jpeg" },
            { 13, "https://content.discogs.com/media/glass-beams-mirage-300x296.jpg" },
            { 14, "https://content.discogs.com/media/Album-Cover-Massive-Attack-%E2%80%8E%E2%80%93-Blue-Lines-300x300.jpg" },
            { 15, "https://content.discogs.com/media/Album-Cover-Madvillain-Madvillainy-300x300.jpeg" },

        };

        public string GetImageUrl(int albumId)
        {
            return storage.ContainsKey(albumId) ? storage[albumId] : NO_COVER;
        }
    }
}
