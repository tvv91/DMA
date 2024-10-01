using Web.Models;

namespace Tests
{
    /// <summary>
    /// Data for tests with different variantions (like different vinyl states, digitization data, etc.)
    /// </summary>
    public class TestData
    {
        DateTime t = DateTime.Now;
        private List<Album> albums = new List<Album>();
        public List<Album> GetData()
        {
            /*
            #region Artists
            Artist artist1 = new Artist { Id = 1, Data = "Artist1" };
            Artist artist2 = new Artist { Id = 2, Data = "Artist2" };
            Artist artist3 = new Artist { Id = 3, Data = "Artist3" };
            #endregion

            #region Genres
            Genre genre1 = new Genre { Id = 1, Data = "Industrial" };
            Genre genre2 = new Genre { Id = 2, Data = "Heavy Metal" };
            Genre genre3 = new Genre { Id = 3, Data = "Hard Rock" };
            Genre genre4 = new Genre { Id = 4, Data = "Jazz" };
            Genre genre5 = new Genre { Id = 5, Data = "Alternative" };
            #endregion

            #region Years
            Year year1 = new Year { Id = 1, Data = 1970 };
            Year year2 = new Year { Id = 2, Data = 1999 };
            Year year3 = new Year { Id = 3, Data = 2004 };
            Year year4 = new Year { Id = 4, Data = 2010 };
            Year year5 = new Year { Id = 5, Data = 2020 };
            #endregion

            #region Reissues
            Reissue reissue1 = new Reissue { Id = 1, Data = 2010 };
            Reissue reissue2 = new Reissue { Id = 2, Data = 2015 };
            Reissue reissue3 = new Reissue { Id = 3, Data = 2020 };
            #endregion

            #region Country
            Country country1 = new Country { Id = 1, Data = "France" };
            Country country2 = new Country { Id = 2, Data = "USA" };
            Country country3 = new Country { Id = 3, Data = "Germany" };
            #endregion

            #region Labels
            Label label1 = new Label { Id = 1, Data = "Roadrunner Records" };
            Label label2 = new Label { Id = 2, Data = "Loma Vista" };
            Label label3 = new Label { Id = 3, Data = "Interscope Records" };
            #endregion

            #region Technical Infos
            #region Adcs
            Adc adc1 = new Adc { Id = 1, Data = "Adc1" };
            Adc adc2 = new Adc { Id = 2, Data = "Adc2" };
            #endregion

            #region Amplifiers
            Amplifier amp1 = new Amplifier { Id = 1, Data = "Amplifier1" };
            Amplifier amp2 = new Amplifier { Id = 2, Data = "Amplifier2" };
            #endregion

            #region Bitnesses
            Bitness bitness1 = new Bitness { Id = 1, Data = 24 };
            Bitness bitness2 = new Bitness { Id = 2, Data = 32 };
            Bitness bitness3 = new Bitness { Id = 3, Data = 1 };
            #endregion

            #region Cartriges
            Cartrige cartrige1 = new Cartrige { Id = 1, Data = "Cartrige1" };
            Cartrige cartrige2 = new Cartrige { Id = 2, Data = "Cartrige2" };
            Cartrige cartrige3 = new Cartrige { Id = 3, Data = "Cartrige3" };
            #endregion

            #region Codecs
            DigitalFormat codec1 = new DigitalFormat { Id = 1, Data = "FLAC" };
            DigitalFormat codec2 = new DigitalFormat { Id = 2, Data = "DSD" };
            #endregion

            #region Devices
            Player device1 = new Player { Id = 1, Data = "Device1" };
            Player device2 = new Player { Id = 2, Data = "Device2" };
            Player device3 = new Player { Id = 3, Data = "Device3" };
            #endregion

            #region Formats
            SourceFormat format1 = new SourceFormat { Id = 1, Data = "Vinyl" };
            #endregion

            #region Processings
            Processing processing1 = new Processing { Id = 1, Data = "Declicking" };
            #endregion

            #region Samplings
            Sampling sampling1 = new Sampling { Id = 1, Data = 192 };
            Sampling sampling2 = new Sampling { Id = 2, Data = 384 };
            Sampling sampling3 = new Sampling { Id = 3, Data = 128 };
            #endregion

            #region States
            VinylState state1 = new VinylState { Id = 1, Data = "Mint" };
            VinylState state2 = new VinylState { Id = 2, Data = "Near Mint" };
            VinylState state3 = new VinylState { Id = 3, Data = "Very Good" };
            #endregion

            TechnicalInfo ti1 = new TechnicalInfo 
            {
                Id = 1,
                Adc = adc1,
                Amplifier = amp1,
                Bitness = bitness1,
                Cartrige = cartrige1,
                DigitalFormat = codec1,
                Player = device1,
                SourceFormat = format1,
                Processing = processing1,
                Sampling = sampling1,
                VinylState = state3
            };
            TechnicalInfo ti2 = new TechnicalInfo
            {
                Id = 2,
                Adc = adc1,
                Amplifier = amp1,
                Bitness = bitness1,
                Cartrige = cartrige1,
                DigitalFormat = codec1,
                Player = device1,
                SourceFormat = format1,
                Processing = processing1,
                Sampling = sampling1,
                VinylState = state2
            };
            TechnicalInfo ti3 = new TechnicalInfo
            {
                Id = 3,
                Adc = adc1,
                Amplifier = amp1,
                Bitness = bitness2,
                Cartrige = cartrige2,
                DigitalFormat = codec1,
                Player = device2,
                SourceFormat = format1,
                Processing = processing1,
                Sampling = sampling2,
                VinylState = state2
            };
            TechnicalInfo ti4 = new TechnicalInfo
            {
                Id = 4,
                Adc = adc2,
                Amplifier = amp2,
                Bitness = bitness3,
                Cartrige = cartrige2,
                DigitalFormat = codec2,
                Player = device3,
                SourceFormat = format1,
                Sampling = sampling3,
                VinylState = state1
            };
            TechnicalInfo ti5 = new TechnicalInfo
            {
                Id = 5,
                Adc = adc2,
                Amplifier = amp2,
                Bitness = bitness3,
                Cartrige = cartrige3,
                DigitalFormat = codec2,
                Player = device3,
                SourceFormat = format1,
                Sampling = sampling3,
                VinylState = state1
            };
            #endregion

            albums.Add(new Album
            {
                Id = 1,
                Data = "Album1",
                Artist = artist1,
                Genre = genre1,
                Year = year1,
                Reissue = reissue1,
                Country = country1,
                Label = label1,
                TechnicalInfo = ti1
            });
            albums.Add(new Album { Id = 2, Data = "Album2", Artist = artist1, Genre = genre1, Year = year2 });
            albums.Add(new Album { Id = 3, Data = "Album3", Artist = artist1, Genre = genre1, Year = year3 });
            albums.Add(new Album { Id = 4, Data = "Album4", Artist = artist1, Genre = genre1, Year = year4 });
            albums.Add(new Album { Id = 5, Data = "Album5", Artist = artist1, Genre = genre1, Year = year5 });

            albums.Add(new Album
            {
                Id = 6,
                Data = "Album6",
                Artist = artist2,
                Genre = genre2,
                Year = year1,
                Reissue = reissue1,
                Country = country1,
                Label = label2,
                TechnicalInfo = ti2
            });
            albums.Add(new Album { Id = 7, Data = "Album7", Artist = artist2, Genre = genre2, Year = year2 });
            albums.Add(new Album { Id = 8, Data = "Album8", Artist = artist2, Genre = genre2, Year = year3 });
            albums.Add(new Album { Id = 9, Data = "Album9", Artist = artist2, Genre = genre2, Year = year4 });
            albums.Add(new Album { Id = 10, Data = "Album10", Artist = artist2, Genre = genre2, Year = year5 });

            albums.Add(new Album
            {
                Id = 11,
                Data = "Album11",
                Artist = artist3,
                Genre = genre3,
                Year = year1,
                Reissue = reissue2,
                Country = country2,
                Label = label2,
                TechnicalInfo = ti3
            });
            albums.Add(new Album { Id = 12, Data = "Album12", Artist = artist3, Genre = genre3, Year = year2 });
            albums.Add(new Album { Id = 13, Data = "Album13", Artist = artist3, Genre = genre3, Year = year3 });
            albums.Add(new Album { Id = 14, Data = "Album14", Artist = artist3, Genre = genre3, Year = year4 });
            albums.Add(new Album { Id = 15, Data = "Album15", Artist = artist3, Genre = genre3, Year = year5 });

            albums.Add(new Album
            {
                Id = 16,
                Data = "Album16",
                Artist = artist1,
                Genre = genre4,
                Year = year1,
                Reissue = reissue3,
                Country = country2,
                Label = label2,
                TechnicalInfo = ti4
            });
            albums.Add(new Album { Id = 17, Data = "Album17", Artist = artist1, Genre = genre4, Year = year2 });
            albums.Add(new Album { Id = 18, Data = "Album18", Artist = artist2, Genre = genre4, Year = year3 });
            albums.Add(new Album { Id = 19, Data = "Album19", Artist = artist2, Genre = genre4, Year = year4 });
            albums.Add(new Album { Id = 20, Data = "Album20", Artist = artist3, Genre = genre4, Year = year5 });

            albums.Add(new Album
            {
                Id = 21,
                Data = "Album21",
                Artist = artist1,
                Genre = genre5,
                Year = year1,
                Reissue = reissue3,
                Country = country3,
                Label = label3,
                TechnicalInfo = ti5
            });
            albums.Add(new Album { Id = 22, Data = "Album22", Artist = artist2, Genre = genre5, Year = year2 });
            albums.Add(new Album { Id = 23, Data = "Album23", Artist = artist3, Genre = genre5, Year = year3 });
            albums.Add(new Album { Id = 24, Data = "Album24", Artist = artist1, Genre = genre5, Year = year4 });
            albums.Add(new Album { Id = 25, Data = "Album25", Artist = artist1, Genre = genre5, Year = year5 });

            return albums;
            */
            return new List<Album>();
        }
    }
}
