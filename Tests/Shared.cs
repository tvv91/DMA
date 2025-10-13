using Web.Models;

namespace Tests
{
    public static class Shared
    {
        public static Album GetFullAlbum()
        {
            return new Album
            {
                Id = 123,
                Title = "Some album",
                Size = 5.4,
                Source = "https://somesource.com",
                Discogs = "https://discogs.com",
                AddedDate = DateTime.Now,
                Artist = new Artist { Data = "Some artist" },
                Genre = new Genre { Name = "Heavy Metal" },
                Year = new Year { Value = 2010 },
                Reissue = new Reissue { Data = 2020 },
                Country = new Country { Name = "USA" },
                Label = new Label { Data = "Some label" },
                Storage = new Storage { Data = "Some storage" },
                TechnicalInfo = new TechnicalInfo
                {
                    Bitness = new Bitness { Value = 24 },
                    DigitalFormat = new DigitalFormat { Name = "FLAC" },
                    SourceFormat = new SourceFormat { Name = "LP 12'' 33RPM" },
                    Sampling = new Sampling { Value = 192 },
                    VinylState = new VinylState { Name = "Mint" },
                    Adc = new Adc
                    {
                        Name = "Some Adc Model",
                        Manufacturer = new AdcManufacturer { Name = "Some Adc Manufacturer" }
                    },
                    Amplifier = new Amplifier
                    {
                        Name = "Some Amplifier Model",
                        Manufacturer = new AmplifierManufacturer { Name = "Some Amplifier Manufacturer" }
                    },
                    Cartridge = new Cartridge
                    {
                        Name = "Some Cartridge Model",
                        Manufacturer = new CartridgeManufacturer { Name = "Some Cartridge Manufacturer" }
                    },
                    Player = new Player
                    {
                        Name = "Some Player Model",
                        Manufacturer = new PlayerManufacturer { Name = "Some Player Manufacturer" }
                    },
                    Wire = new Wire
                    {
                        Name = "Some Wire Model",
                        Manufacturer = new WireManufacturer { Name = "Some Wire Manufacturer" }
                    }
                }
            };
        }
    }
}
