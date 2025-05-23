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
                Data = "Some album",
                Size = 5.4,
                Source = "https://somesource.com",
                Discogs = "https://discogs.com",
                AddedDate = DateTime.Now,
                Artist = new Artist { Data = "Some artist" },
                Genre = new Genre { Data = "Heavy Metal" },
                Year = new Year { Data = 2010 },
                Reissue = new Reissue { Data = 2020 },
                Country = new Country { Data = "USA" },
                Label = new Label { Data = "Some label" },
                Storage = new Storage { Data = "Some storage" },
                TechnicalInfo = new TechnicalInfo
                {
                    Adc = new Adc
                    {
                        Data = "Some Adc Model",
                        Manufacturer = new AdcManufacturer { Data = "Some Adc Manufacturer" }
                    },
                    Amplifier = new Amplifier
                    {
                        Data = "Some Amplifier Model",
                        Manufacturer = new AmplifierManufacturer { Data = "Some Amplifier Manufacturer" }
                    },
                    Bitness = new Bitness { Data = 24 },
                    Cartridge = new Cartridge
                    {
                        Data = "Some Cartridge Model",
                        Manufacturer = new CartridgeManufacturer { Data = "Some Cartridge Manufacturer" }
                    },
                    DigitalFormat = new DigitalFormat { Data = "FLAC" },
                    Player = new Player
                    {
                        Data = "Some Player Model",
                        Manufacturer = new PlayerManufacturer { Data = "Some Player Manufacturer" }
                    },
                    SourceFormat = new SourceFormat { Data = "LP 12'' 33RPM" },
                    Sampling = new Sampling { Data = 192 },
                    VinylState = new VinylState { Data = "Mint" },
                    Wire = new Wire
                    {
                        Data = "Some Wire Model",
                        Manufacturer = new WireManufacturer { Data = "Some Wire Manufacturer" }
                    }
                }
            };
        }
    }
}
