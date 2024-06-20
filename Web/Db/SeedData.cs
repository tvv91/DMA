using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Db
{
    public class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            DMADbContext ctx = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<DMADbContext>();
            if (ctx.Database.GetPendingMigrations().Any())
            {
                ctx.Database.Migrate();
            }
            if (!ctx.Albums.Any())
            {
                #region Artists
                Artist artist1 = new Artist { Data = "Artist1" };
                Artist artist2 = new Artist { Data = "Artist2" };
                Artist artist3 = new Artist { Data = "Artist3" };
                #endregion

                #region Genres
                Genre genre1 = new Genre { Data = "Industrial" };
                Genre genre2 = new Genre { Data = "Heavy Metal" };
                Genre genre3 = new Genre { Data = "Hard Rock" };
                Genre genre4 = new Genre { Data = "Jazz" };
                Genre genre5 = new Genre { Data = "Alternative" };
                #endregion

                #region Years
                Year year1 = new Year { Data = 1970 };
                Year year2 = new Year { Data = 1999 };
                Year year3 = new Year { Data = 2004 };
                Year year4 = new Year { Data = 2010 };
                Year year5 = new Year { Data = 2020 };
                #endregion

                #region Reissues
                Reissue reissue1 = new Reissue { Data = 2010 };
                Reissue reissue2 = new Reissue { Data = 2015 };
                Reissue reissue3 = new Reissue { Data = 2020 };
                #endregion

                #region Country
                Country country1 = new Country { Data = "France" };
                Country country2 = new Country { Data = "USA" };
                Country country3 = new Country { Data = "Germany" };
                #endregion

                #region Labels
                Label label1 = new Label { Data = "Roadrunner Records" };
                Label label2 = new Label { Data = "Loma Vista" };
                Label label3 = new Label { Data = "Interscope Records" };
                #endregion

                #region Technical Infos
                #region Adcs
                Adc adc1 = new Adc { Data = "Adc1" };
                Adc adc2 = new Adc { Data = "Adc2" };
                #endregion

                #region Amplifiers
                Amplifier amp1 = new Amplifier { Data = "Amplifier1" };
                Amplifier amp2 = new Amplifier { Data = "Amplifier2" };
                #endregion

                #region Bitnesses
                Bitness bitness1 = new Bitness { Data = 24 };
                Bitness bitness2 = new Bitness { Data = 32 };
                Bitness bitness3 = new Bitness { Data = 1 };
                #endregion

                #region Cartriges
                Cartrige cartrige1 = new Cartrige { Data = "Cartrige1" };
                Cartrige cartrige2 = new Cartrige { Data = "Cartrige2" };
                Cartrige cartrige3 = new Cartrige { Data = "Cartrige3" };
                #endregion

                #region Codecs
                Codec codec1 = new Codec { Data = "FLAC" };
                Codec codec2 = new Codec { Data = "DSD" };
                #endregion

                #region Devices
                Device device1 = new Device { Data = "Device1" };
                Device device2 = new Device { Data = "Device2" };
                Device device3 = new Device { Data = "Device3" };
                #endregion

                #region Formats
                Format format1 = new Format { Data = "Vinyl" };
                #endregion

                #region Processings
                Processing processing1 = new Processing { Data = "Declicking" };
                #endregion

                #region Samplings
                Sampling sampling1 = new Sampling { Data = 192 };
                Sampling sampling2 = new Sampling { Data = 384 };
                Sampling sampling3 = new Sampling { Data = 128 };
                #endregion

                #region States
                State state1 = new State { Data = "Mint" };
                State state2 = new State { Data = "Nearmint" };
                State state3 = new State { Data = "Verygood+" };
                State state4 = new State { Data = "Verygood" };
                State state5 = new State { Data = "Good" };
                #endregion

                TechnicalInfo ti1 = new TechnicalInfo
                {
                    Adc = adc1,
                    Amplifier = amp1,
                    Bitness = bitness1,
                    Cartrige = cartrige1,
                    Codec = codec1,
                    Device = device1,
                    Format = format1,
                    Processing = processing1,
                    Sampling = sampling1,
                    State = state3
                };
                TechnicalInfo ti2 = new TechnicalInfo
                {
                    Adc = adc1,
                    Amplifier = amp1,
                    Bitness = bitness1,
                    Cartrige = cartrige1,
                    Codec = codec1,
                    Device = device1,
                    Format = format1,
                    Processing = processing1,
                    Sampling = sampling1,
                    State = state2
                };
                TechnicalInfo ti3 = new TechnicalInfo
                {
                    Adc = adc1,
                    Amplifier = amp1,
                    Bitness = bitness2,
                    Cartrige = cartrige2,
                    Codec = codec1,
                    Device = device2,
                    Format = format1,
                    Processing = processing1,
                    Sampling = sampling2,
                    State = state2
                };
                TechnicalInfo ti4 = new TechnicalInfo
                {
                    Adc = adc2,
                    Amplifier = amp2,
                    Bitness = bitness3,
                    Cartrige = cartrige2,
                    Codec = codec2,
                    Device = device3,
                    Format = format1,
                    Sampling = sampling3,
                    State = state1
                };
                TechnicalInfo ti5 = new TechnicalInfo
                {
                    Adc = adc2,
                    Amplifier = amp2,
                    Bitness = bitness3,
                    Cartrige = cartrige3,
                    Codec = codec2,
                    Device = device3,
                    Format = format1,
                    Sampling = sampling3,
                    State = state1
                };
                #endregion

                ctx.Albums.Add(new Album
                {
                    Data = "Album1",
                    Artist = artist1,
                    Genre = genre1,
                    Year = year1,
                    Reissue = reissue1,
                    Country = country1,
                    Label = label1,
                    TechnicalInfo = ti1,
                });
                ctx.Albums.Add(new Album { Data = "Album2", Artist = artist1, Genre = genre1, Year = year2 });
                ctx.Albums.Add(new Album { Data = "Album3", Artist = artist1, Genre = genre1, Year = year3 });
                ctx.Albums.Add(new Album { Data = "Album4", Artist = artist1, Genre = genre1, Year = year4 });
                ctx.Albums.Add(new Album { Data = "Album5", Artist = artist1, Genre = genre1, Year = year5 });

                ctx.Albums.Add(new Album
                {
                    Data = "Album6",
                    Artist = artist2,
                    Genre = genre2,
                    Year = year1,
                    Reissue = reissue1,
                    Country = country1,
                    Label = label2
                });
                ctx.Albums.Add(new Album { Data = "Album7", Artist = artist2, Genre = genre2, Year = year2 });
                ctx.Albums.Add(new Album { Data = "Album8", Artist = artist2, Genre = genre2, Year = year3 });
                ctx.Albums.Add(new Album { Data = "Album9", Artist = artist2, Genre = genre2, Year = year4 });
                ctx.Albums.Add(new Album { Data = "Album10", Artist = artist2, Genre = genre2, Year = year5 });

                ctx.Albums.Add(new Album
                {
                    Data = "Album11",
                    Artist = artist3,
                    Genre = genre3,
                    Year = year1,
                    Reissue = reissue2,
                    Country = country2,
                    Label = label2
                });
                ctx.Albums.Add(new Album { Data = "Album12", Artist = artist3, Genre = genre3, Year = year2 });
                ctx.Albums.Add(new Album { Data = "Album13", Artist = artist3, Genre = genre3, Year = year3 });
                ctx.Albums.Add(new Album { Data = "Album14", Artist = artist3, Genre = genre3, Year = year4 });
                ctx.Albums.Add(new Album { Data = "Album15", Artist = artist3, Genre = genre3, Year = year5 });

                ctx.Albums.Add(new Album
                {
                    Data = "Album16",
                    Artist = artist1,
                    Genre = genre4,
                    Year = year1,
                    Reissue = reissue3,
                    Country = country2,
                    Label = label2
                });
                ctx.Albums.Add(new Album { Data = "Album17", Artist = artist1, Genre = genre4, Year = year2 });
                ctx.Albums.Add(new Album { Data = "Album18", Artist = artist2, Genre = genre4, Year = year3 });
                ctx.Albums.Add(new Album { Data = "Album19", Artist = artist2, Genre = genre4, Year = year4 });
                ctx.Albums.Add(new Album { Data = "Album20", Artist = artist3, Genre = genre4, Year = year5 });

                ctx.Albums.Add(new Album
                {
                    Data = "Album21",
                    Artist = artist1,
                    Genre = genre5,
                    Year = year1,
                    Reissue = reissue3,
                    Country = country3,
                    Label = label3
                });
                ctx.Albums.Add(new Album { Data = "Album22", Artist = artist2, Genre = genre5, Year = year2 });
                ctx.Albums.Add(new Album { Data = "Album23", Artist = artist3, Genre = genre5, Year = year3 });
                ctx.Albums.Add(new Album { Data = "Album24", Artist = artist1, Genre = genre5, Year = year4 });
                ctx.Albums.Add(new Album { Data = "Album25", Artist = artist1, Genre = genre5, Year = year5 });
            }
            ctx.SaveChanges();
        }
    }
}
