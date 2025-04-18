using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Request;

namespace Web.Db
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly DMADbContext _context;

        public AlbumRepository(DMADbContext ctx, ITechInfoRepository repository)
        {
            _context = ctx;
        }

        public IQueryable<Album> Albums => _context.Albums;

        public IQueryable<Artist> Artists => _context.Artists;

        public IQueryable<Country> Countries => _context.Countries;

        public IQueryable<Genre> Genres => _context.Genres;

        public IQueryable<Reissue> Reissues => _context.Reissues;

        public IQueryable<Year> Years => _context.Years;

        public IQueryable<Label> Labels => _context.Labels;

        public IQueryable<Storage> Storages => _context.Storages;

        public async Task<Album> CreateOrUpdateAlbumAsync(AlbumDataRequest request)
        {
            var _album = await _context.Albums.FirstOrDefaultAsync(x => x.Id == request.AlbumId);

            if (_album == null)
            {
                _album = new();
                await _context.Albums.AddAsync(_album);
            }

            _album.Data = request.Album;
            _album.Source = request.Source;
            _album.Size = request.Size;
            _album.Discogs = request.Discogs;
            _album.AddedDate = DateTime.Now;

            await CreateOrUpdateArtistAsync(_album, request);
            await CreateOrUpdateGenreAsync(_album, request);
            await CreateOrUpdateYearAsync(_album, request);
            await CreateOrUpdateReissueAsync(_album, request);
            await CreateOrUpdateCountryAsync(_album, request);
            await CreateOrUpdateLabelAsync(_album, request);
            await CreateOrUpdateStorageAsync(_album, request);

            await _context.SaveChangesAsync();

            return _album;
        }

        private async Task CreateOrUpdateArtistAsync(Album album, AlbumDataRequest request)
        {
            var _artist = await _context.Artists.FirstOrDefaultAsync(x => x.Data == request.Artist);

            if (_artist == null)
            {
                _artist = new() { Data = request.Artist };
                await _context.Artists.AddAsync(_artist);
            }

            album.Artist = _artist;
        }

        private async Task CreateOrUpdateGenreAsync(Album album, AlbumDataRequest request)
        {
            if (request.Genre == null)
            {
                album.GenreId = null;
            }
            else
            {
                var _genre = await _context.Genres.FirstOrDefaultAsync(x => x.Data == request.Genre);

                if (_genre == null)
                {
                    _genre = new() { Data = request.Genre };
                    await _context.Genres.AddAsync(_genre);
                }

                album.Genre = _genre;
            } 
        }

        private async Task CreateOrUpdateYearAsync(Album album, AlbumDataRequest request)
        {
            if (request.Year == null)
            {
                album.YearId = null;
            }
            else
            {
                var _year = await _context.Years.FirstOrDefaultAsync(x => x.Data == request.Year);

                if (_year == null)
                {
                    _year = new() { Data = request.Year.Value };
                    await _context.Years.AddAsync(_year);
                }
                
                album.Year = _year;
            }            
        }

        private async Task CreateOrUpdateReissueAsync(Album album, AlbumDataRequest request)
        {
            if (request.Reissue == null)
            {
                album.ReissueId = null;
            }
            else
            {
                var _reissue = await _context.Reissues.FirstOrDefaultAsync(x => x.Data == request.Reissue);

                if (_reissue == null)
                {
                    _reissue = new() { Data = request.Reissue };
                    await _context.Reissues.AddAsync(_reissue);
                }

                album.Reissue = _reissue;
            }
        }

        private async Task CreateOrUpdateCountryAsync(Album album, AlbumDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Country))
            {
                album.CountryId = null;
            }
            else
            {
                var _country = await _context.Countries.FirstOrDefaultAsync(x => x.Data == request.Country);

                if (_country == null)
                {
                    _country = new() { Data = request.Country };
                    await _context.Countries.AddAsync(_country);
                }

                album.Country = _country;
            }
        }

        private async Task CreateOrUpdateLabelAsync(Album album, AlbumDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Label))
            {
                album.LabelId = null;
            }
            else
            {
                var _label = await _context.Labels.FirstOrDefaultAsync(x => x.Data == request.Label);

                if (_label == null)
                {
                    _label = new() { Data = request.Label };
                    await _context.Labels.AddAsync(_label);
                }

                album.Label = _label;
            }
        }

        private async Task CreateOrUpdateStorageAsync(Album album, AlbumDataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Storage))
            {
                album.StorageId = null;
            }
            else
            {
                var _storage = await _context.Storages.FirstOrDefaultAsync(x => x.Data == request.Storage);

                if (_storage == null)
                {
                    _storage = new() { Data = request.Storage };
                    await _context.Storages.AddAsync(_storage);
                }

                album.Storage = _storage;
            }
        }

        public async Task<Album> GetByIdAsync(int albumId)
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Country)
                .Include(a => a.Genre)
                .Include(a => a.Label)
                .Include(a => a.Reissue)
                .Include(a => a.Year)
                .Include(a => a.Storage)
                .FirstOrDefaultAsync(a => a.Id == albumId);
        }
    }
}
