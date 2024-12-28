using Web.Models;

namespace Web.Db
{
    public static class TestData
    { 
        private static List<Album> albums = new List<Album>();

        public static List<Album> GetAlbums()
        {
            #region Artist
            var artist1 = new Artist { Data = "Artist 1" };
            var artist2 = new Artist { Data = "Artist 2" };
            #endregion

            #region Genre
            var genre1 = new Genre  { Data = "Heavy Metal" };
            var genre2 = new Genre { Data = "Jazz" };
            var genre3 = new Genre { Data = "Pop" };
            #endregion

            #region Year
            var year = new Year { Data = 2005 };
            #endregion

            #region Reissue
            var reissue = new Reissue { Data = 2020 };
            #endregion

            #region Country
            var country1 = new Country { Data = "USA" };
            var country2 = new Country { Data = "France" };
            var country3 = new Country { Data = "Germany" };

            #endregion

            #region Label
            var label = new Label { Data = "Roadrunner Records" };
            #endregion

            #region Storage
            var storage = new Storage { Data = "D1" };
            #endregion

            #region Manufacturers

            #region Adc
            #region Manufacturers
            var adcManufacturer1 = new AdcManufacturer { Data = "TASCAM" };
            var adcManufacturer2 = new AdcManufacturer { Data = "MOTU" };
            var adcManufacturer3 = new AdcManufacturer { Data = "RME" };
            #endregion
            
            #region Adc
            var adc1 = new Adc
            {
                Manufacturer = adcManufacturer1,
                Data = "HS-P82"
            };
            var adc2 = new Adc
            {
                Manufacturer = adcManufacturer2,
                Data = "MK3"
            };
            var adc3 = new Adc
            {
                Manufacturer = adcManufacturer3,
                Data = "Fireface UC"
            };
            #endregion
            #endregion

            #region Amplifier
            #region Manufacturers
            var ampManufacturer1 = new AmplifierManufacturer { Data = "Parasound" };
            var ampManufacturer2 = new AmplifierManufacturer { Data = "Rotel" };
            var ampManufacturer3 = new AmplifierManufacturer { Data = "Yamaha" };
            #endregion

            #region Models
            var amp1 = new Amplifier
            {
                Manufacturer = ampManufacturer1,
                Data = "JC 2 BP Black"
            };
            var amp2 = new Amplifier
            {
                Manufacturer = ampManufacturer1,
                Data = "RC-1590 MkII Black"
            };
            var amp3 = new Amplifier
            {
                Manufacturer = ampManufacturer3,
                Data = "C-5000 Black"
            };
            #endregion
            #endregion

            #region Cartrige
            #region Manufacturers
            var cartrigeManufacturer1 = new CartrigeManufacturer { Data = "Lyra" };
            var cartrigeManufacturer2 = new CartrigeManufacturer { Data = "Ortofon" };
            var cartrigeManufacturer3 = new CartrigeManufacturer { Data = "Audio-Technica" };
            #endregion

            #region Models
            var cartrige1 = new Cartrige
            {
                Manufacturer = cartrigeManufacturer1,
                Data = "Delos"
            };
            var cartrige2 = new Cartrige
            {
                Manufacturer = cartrigeManufacturer2,
                Data = "SPU Royal N"
            };
            var cartrige3 = new Cartrige
            {
                Manufacturer = cartrigeManufacturer3,
                Data = "AT-OC9XSL"
            };
            #endregion
            #endregion

            #region Player
            #region Manufacturers
            var playerManufacturer1 = new PlayerManufacturer { Data = "Technics" };
            var playerManufacturer2 = new PlayerManufacturer { Data = "Denon" };
            var playerManufacturer3 = new PlayerManufacturer { Data = "Pro-Ject" };
            #endregion

            #region Models
            var player1 = new Player
            {
                Manufacturer = playerManufacturer1,
                Data = "SL-1200GR Silver"
            };
            var player2 = new Player
            {
                Manufacturer = playerManufacturer2,
                Data = "DP-3000NE"
            };
            var player3 = new Player
            {
                Manufacturer = playerManufacturer3,
                Data = "The Classic Evo 2M Silver Walnut"
            };
            #endregion
            #endregion

            #region Wires
            #region Manufacturers
            var wireManufacturer1 = new WireManufacturer { Data = "Atlas" };
            var wireManufacturer2 = new WireManufacturer { Data = "Nordost" };
            var wireManufacturer3 = new WireManufacturer { Data = "CHORD" };
            #endregion

            #region Models
            var wire1 = new Wire
            {
                Manufacturer = wireManufacturer1,
                Data = "Asimi Grun Ultra"
            };
            var wire2 = new Wire
            {
                Manufacturer = wireManufacturer2,
                Data = "Odin 2"
            };
            var wire3 = new Wire
            {
                Manufacturer = wireManufacturer3,
                Data = "SignatureX Tuned ARAY"
            };
            #endregion
            #endregion

            #region Technical Info
            var tInfo1 = new TechnicalInfo
            {
                BitnessId = 1,
                DigitalFormatId = 2,
                SourceFormatId = 1,
                SamplingId = 4,
                VinylStateId = 1,
                Adc = adc1,
                Amplifier = amp1,
                Cartrige = cartrige1,
                Player = player1,
                Wire = wire1
            };
            #endregion
            #endregion
            // Album with
            var album = new Album
            {
                Data = "Album 1",
                Size = 2.4,
                Source = "https://somelink.com",
                Discogs = "https://somelink.com",
                AddedDate = DateTime.Now,
                Artist = artist1,
                Genre = genre1,
                Year = year,
                Reissue = reissue,
                Country = country1,
                Label = label,
                Storage = storage,
                TechnicalInfo = tInfo1,
            };

            albums.Add(album);
            return albums;            
        }
    }
}
