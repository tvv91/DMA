using Web.Models;

namespace Web.Db
{
    public class TestData
    {
        private List<Album> albums = new List<Album>();
        private List<PostCategory> posts = new List<PostCategory>();

        #region Random generation
        private static readonly Category[] categories =
        {
            new Category { Title = "Developing" },
            new Category { Title = "Releases" },
            new Category { Title = "Other" }
        };

        static string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static DateTime GenerateRandomDate(DateTime start, DateTime end)
        {
            var random = new Random();
            var range = (end - start).Days;
            return start.AddDays(random.Next(range));
        }

        static Category GetRandomCategory()
        {
            var random = new Random();
            int index = random.Next(categories.Length);
            return categories[index];
        }
        static bool GetRandomBool()
        {
            var random = new Random();
            return random.Next(2) == 1;
        }
        #endregion

        public List<PostCategory> GetPosts()
        {
            for (int i = 1; i <= 100; i++)
            {
                var post = new Post()
                {
                    Content = GenerateRandomText(1024),
                    Description = GenerateRandomText(50),
                    Title = GenerateRandomText(32),
                    CreatedDate = GenerateRandomDate(new DateTime(2015, 1, 1), DateTime.Today),
                    UpdatedDate = DateTime.Now,
                    IsDraft = GetRandomBool(),
                };

                var postCategory = new PostCategory
                {
                    Post = post,
                    Category = GetRandomCategory()
                };

                posts.Add(postCategory);
            }

            return posts;
        }
        public List<Album> GetAlbums()
        {
            #region Artist
            var artist1 = new Artist { Name = "Artist 1" };
            var artist2 = new Artist { Name = "Artist 2" };
            #endregion

            #region Genre
            var genre1 = new Genre { Name = "Heavy Metal" };
            var genre2 = new Genre { Name = "Jazz" };
            var genre3 = new Genre { Name = "Pop" };
            #endregion

            #region Year
            var year = new Year { Value = 2005 };
            #endregion

            #region Reissue
            var reissue = new Reissue { Value = 2020 };
            #endregion

            #region Country
            var country1 = new Country { Name = "USA" };
            var country2 = new Country { Name = "France" };
            var country3 = new Country { Name = "Germany" };

            #endregion

            #region Label
            var label = new Label { Name = "Roadrunner Records" };
            #endregion            

            #region Storage
            var storage = new Storage { Data = "D1" };
            #endregion

            #region Adc
            #region Manufacturers
            var adcManufacturer1 = new Manufacturer { Name = "TASCAM" };
            var adcManufacturer2 = new Manufacturer { Name = "MOTU" };
            var adcManufacturer3 = new Manufacturer { Name = "RME" };
            #endregion

            #region Adc
            var adc1 = new Adc
            {
                Manufacturer = adcManufacturer1,
                Name = "HS-P82"
            };
            var adc2 = new Adc
            {
                Manufacturer = adcManufacturer2,
                Name = "MK3"
            };
            var adc3 = new Adc
            {
                Manufacturer = adcManufacturer3,
                Name = "Fireface UC"
            };
            #endregion
            #endregion

            #region Amplifier
            #region Manufacturers
            var ampManufacturer1 = new Manufacturer { Name = "Parasound" };
            var ampManufacturer2 = new Manufacturer { Name = "Rotel" };
            var ampManufacturer3 = new Manufacturer { Name = "Yamaha" };
            #endregion

            #region Models
            var amp1 = new Amplifier
            {
                Manufacturer = ampManufacturer1,
                Name = "JC 2 BP Black"
            };
            var amp2 = new Amplifier
            {
                Manufacturer = ampManufacturer1,
                Name = "RC-1590 MkII Black"
            };
            var amp3 = new Amplifier
            {
                Manufacturer = ampManufacturer3,
                Name = "C-5000 Black"
            };
            #endregion
            #endregion

            #region Cartridge
            #region Manufacturers
            var cartridgeManufacturer1 = new Manufacturer { Name = "Lyra" };
            var cartridgeManufacturer2 = new Manufacturer { Name = "Ortofon" };
            var cartridgeManufacturer3 = new Manufacturer { Name = "Audio-Technica" };
            #endregion

            #region Models
            var cartridge1 = new Cartridge
            {
                Manufacturer = cartridgeManufacturer1,
                Name = "Delos"
            };
            var cartridge2 = new Cartridge
            {
                Manufacturer = cartridgeManufacturer2,
                Name = "SPU Royal N"
            };
            var cartridge3 = new Cartridge
            {
                Manufacturer = cartridgeManufacturer3,
                Name = "AT-OC9XSL"
            };
            #endregion
            #endregion

            #region Player
            #region Manufacturers
            var playerManufacturer1 = new Manufacturer { Name = "Technics" };
            var playerManufacturer2 = new Manufacturer { Name = "Denon" };
            var playerManufacturer3 = new Manufacturer { Name = "Pro-Ject" };
            #endregion

            #region Models
            var player1 = new Player
            {
                Manufacturer = playerManufacturer1,
                Name = "SL-1200GR Silver"
            };
            var player2 = new Player
            {
                Manufacturer = playerManufacturer2,
                Name = "DP-3000NE"
            };
            var player3 = new Player
            {
                Manufacturer = playerManufacturer3,
                Name = "The Classic Evo 2M Silver Walnut"
            };
            #endregion
            #endregion

            #region Wires
            #region Manufacturers
            var wireManufacturer1 = new Manufacturer { Name = "Atlas" };
            var wireManufacturer2 = new Manufacturer { Name = "Nordost" };
            var wireManufacturer3 = new Manufacturer { Name = "CHORD" };
            #endregion

            #region Models
            var wire1 = new Wire
            {
                Manufacturer = wireManufacturer1,
                Name = "Asimi Grun Ultra"
            };
            var wire2 = new Wire
            {
                Manufacturer = wireManufacturer2,
                Name = "Odin 2"
            };
            var wire3 = new Wire
            {
                Manufacturer = wireManufacturer3,
                Name = "SignatureX Tuned ARAY"
            };
            #endregion
            #endregion                

            #region Digitization
            var digitization1 = new Digitization
            {
                AddedDate = DateTime.Now,
                Source = "https://somelink.com",
                Discogs = "https://somelink.com",
                Size = 2.4,
                Year = year,
                Reissue = reissue,
                Country = country1,
                Label = label,
                Storage = storage,
                Format = new FormatInfo
                {
                    BitnessId = 2,
                    DigitalFormatId = 1,
                    SourceFormatId = 1,
                    SamplingId = 2,
                    VinylStateId = 1,
                },
                Equipment = new EquipmentInfo
                {
                    Adc = adc1,
                    Amplifier = amp1,
                    Cartridge = cartridge1,
                    Player = player1,
                    Wire = wire1
                }
            };
            #endregion

            for (int i = 1; i <= 100; i++)
            {
                var album = new Album
                {
                    Title = $"Album {i}",                    
                    Artist = artist1,
                    Genre = genre1,
                    Digitizations = new List<Digitization> { digitization1 }
                };
                albums.Add(album);
            }
            return albums;
        }
    }
}
