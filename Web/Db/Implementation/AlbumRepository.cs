using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Request;

namespace Web.Db
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly DMADbContext _context;
        private readonly ITechInfoRepository _techInfoRepository;

        public AlbumRepository(DMADbContext ctx, ITechInfoRepository repository)
        {
            _context = ctx;
            _techInfoRepository = repository;
        }

        public IQueryable<Album> Albums => _context.Albums;

        public IQueryable<Artist> Artists => _context.Artists;

        public IQueryable<Country> Countries => _context.Countries;

        public IQueryable<Genre> Genres => _context.Genres;

        public IQueryable<Reissue> Reissues => _context.Reissues;

        public IQueryable<Year> Years => _context.Years;

        public IQueryable<Label> Labels => _context.Labels;

        public IQueryable<Storage> Storages => _context.Storages;

        public async Task<Album> CreateNewAlbum(AlbumDataRequest request)
        {
            #region Required fields
            var artist = await FindArtistAsync(request.Artist);
            if (artist == null)
            {
                artist = await CreateArtistAsync(request.Artist);
            }

            var genre = await FindGenreAsync(request.Genre);
            if (genre == null)
            {
                genre = await CreateGenreAsync(request.Genre);
            }

            var year = await FindYearAsync(request.Year);
            if (year == null)
            {
                year = await CreateYearAsync(request.Year);
            }
            #endregion

            var album = new Album
            {
                Data = request.Album,
                Artist = artist,
                Genre = genre,
                Year = year,
                Size = request.Size,
                Source = request.Source,
                Discogs = request.Discogs,
                AddedDate = DateTime.Now
            };

            #region Not required fields
            if (request.Reissue != null)
            {
                var reissue = await FindReissueYearAsync(request.Reissue.Value);
                if (reissue == null)
                {
                    reissue = await CreateReissueYearAsync(request.Reissue.Value);
                }
                album.Reissue = reissue;
            }

            if (request.Country != null)
            {
                var country = await FindCountryAsync(request.Country);
                if (country == null)
                {
                    country = await CreateCountryAsync(request.Country);
                }
                album.Country = country;
            }

            if (request.Label != null)
            {
                var label = await FindLabelAsync(request.Label);
                if (label == null)
                {
                    label = await CreateLabelAsync(request.Label);
                }
                album.Label = label;
            }

            if (request.Storage != null)
            {
                var storage = await FindStorageAsync(request.Storage);
                if (storage == null)
                {
                    storage = await CreateStorageAsync(request.Storage);
                }
                album.Storage = storage;
            }
            #endregion

            var tInfo = await _techInfoRepository.CreateTechnicalInfoAsync(request);
            if (tInfo != null)
            {
                album.TechnicalInfo = tInfo;
            }

            await _context.Albums.AddAsync(album);
            await _context.SaveChangesAsync();
            return album;
        }

        private async Task<Label> FindLabelAsync(string label)
        {
            return await Labels.FirstOrDefaultAsync(x => x.Data == label);
        }

        private async Task<Label> CreateLabelAsync(string label)
        {
            var _label = new Label { Data = label };
            await _context.Labels.AddAsync(_label);
            await _context.SaveChangesAsync();
            return _label;
        }

        private async Task<Country> FindCountryAsync(string country)
        {
            return await Countries.FirstOrDefaultAsync(x => x.Data == country);
        }

        private async Task<Country> CreateCountryAsync(string country)
        {
            var _country = new Country { Data = country };
            await _context.Countries.AddAsync(_country);
            await _context.SaveChangesAsync();
            return _country;
        }

        private async Task<Reissue> FindReissueYearAsync(int? year)
        {
            return await Reissues.FirstOrDefaultAsync(x => x.Data == year);
        }

        private async Task<Reissue> CreateReissueYearAsync(int? year)
        {
            var _year = new Reissue { Data = year };
            await _context.Reissues.AddAsync(_year);
            await _context.SaveChangesAsync();
            return _year;
        }

        private async Task<Year> FindYearAsync(int year)
        {
            return await Years.FirstOrDefaultAsync(x => x.Data == year);
        }

        private async Task<Year> CreateYearAsync(int year)
        {
            var _year = new Year { Data = year };
            await _context.Years.AddAsync(_year);
            await _context.SaveChangesAsync();
            return _year;
        }

        private async Task<Artist> FindArtistAsync(string artistName)
        {
            return await Artists.FirstOrDefaultAsync(x => x.Data == artistName);
        }

        private async Task<Artist> CreateArtistAsync(string artistName)
        {
            var artist = new Artist { Data = artistName };
            await _context.Artists.AddAsync(artist);
            await _context.SaveChangesAsync();
            return artist;
        }

        private async Task<Genre> FindGenreAsync(string genreName)
        {
            return await Genres.FirstOrDefaultAsync(x => x.Data == genreName);
        }

        private async Task<Genre> CreateGenreAsync(string genreName)
        {
            var genre = new Genre { Data = genreName };
            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
            return genre;
        }

        private async Task<Storage> FindStorageAsync(string storageName)
        {
            return await Storages.FirstOrDefaultAsync(x => x.Data == storageName);
        }

        private async Task<Storage> CreateStorageAsync(string storageName)
        {
            var storage = new Storage { Data = storageName };
            await _context.Storages.AddAsync(storage);
            await _context.SaveChangesAsync();
            return storage;
        }

        public async Task<Album> UpdateAlbum(Album album, AlbumDataRequest request)
        {
            album.Data = request.Album;
            album.Size = request.Size;
            album.Source = request.Source;
            album.Discogs = request.Discogs;

            if (album.Artist.Data != request.Artist)
            {
                var artist = await FindArtistAsync(request.Artist);
                if (artist == null)
                {
                    artist = await CreateArtistAsync(request.Artist);
                }
                album.Artist = artist;
            }

            if (album.Genre.Data != request.Genre)
            {
                var genre = await FindGenreAsync(request.Genre);
                if(genre == null)
                {
                    genre = await CreateGenreAsync(request.Genre);
                }
                album.Genre = genre;
            }

            if(album.Year.Data != request.Year)
            {
                var year = await FindYearAsync(request.Year);
                if (year == null)
                {
                    year = await CreateYearAsync(request.Year);
                }
                album.Year = year;
            }

            if(request.Reissue != null && album?.Reissue?.Data != request.Reissue)
            {
                var reissue = await FindReissueYearAsync(request.Reissue);
                if (reissue == null)
                {
                    reissue = await CreateReissueYearAsync(request.Reissue);
                }
                album.Reissue = reissue;
            }

            if (request.Country != null && album?.Country?.Data != request.Country)
            {
                var country = await FindCountryAsync(request.Country);
                if (country == null)
                {
                    country = await CreateCountryAsync(request.Country);
                }
                album.Country = country;
            }

            if (request.Label != null && album?.Label?.Data != request.Label)
            {
                var label = await FindLabelAsync(request.Label);
                if (label == null)
                {
                    label = await CreateLabelAsync(request.Label);
                }
                album.Label = label;
            }

            if(request.Storage != null && album?.Storage?.Data != request.Storage)
            {
                var storage = await FindStorageAsync(request.Storage);
                if (storage == null)
                {
                    storage = await CreateStorageAsync(request.Storage);
                }
                album.Storage = storage;
            }

            await _context.SaveChangesAsync();
            return album;
        }
    }
}
