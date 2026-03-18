using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Enums;
using Web.Extentions;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;

namespace Web.Services
{
    public class AlbumService(
        IDigitizationService digitizationService,
        ICoverService imageService,
        DMADbContext context,
        TimeProvider timeProvider) : IAlbumService
    {
        private readonly IDigitizationService _digitizationService = digitizationService;
        private readonly ICoverService _imageService = imageService;
        private readonly DMADbContext _context = context;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<PagedResult<Album>> GetIndexListAsync(int page, int pageSize, string? artistName = null, string? genreName = null, string? yearValue = null, string? albumTitle = null)
        {
            var query = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(artistName))
            {
                query = query.Where(a => a.Artist != null && a.Artist.Name.Contains(artistName));
            }

            if (!string.IsNullOrWhiteSpace(genreName))
            {
                query = query.Where(a => a.Genre != null && a.Genre.Name.Contains(genreName));
            }

            if (!string.IsNullOrWhiteSpace(albumTitle))
            {
                query = query.Where(a => a.Title.Contains(albumTitle));
            }

            if (!string.IsNullOrWhiteSpace(yearValue))
            {
                if (int.TryParse(yearValue, out int yearInt))
                {
                    query = query.Where(a => _context.Digitizations.Any(d => d.AlbumId == a.Id && d.Year != null && d.Year.Value == yearInt));
                }
                else
                {
                    query = query.Where(a => _context.Digitizations.Any(d => d.AlbumId == a.Id && d.Year != null && d.Year.Value.ToString().Contains(yearValue)));
                }
            }

            return await query.ToPagedResultAsync(page, pageSize, a => a.Id);
        }

        public async Task<Album?> GetByIdAsync(int id)
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Album?> FindByAlbumAndArtistAsync(string title, string artist)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(artist))
                return null;

            var normalizedTitle = title.Trim();
            var normalizedArtist = artist.Trim();

            return await _context.Albums
                .Include(a => a.Artist)
                .AsNoTracking()
                .FirstOrDefaultAsync(a =>
                    a.Artist != null &&
                    a.Title == normalizedTitle &&
                    a.Artist.Name == normalizedArtist);
        }

        public async Task<AlbumDetailsViewModel> GetAlbumDetailsAsync(int id)
        {
            var album = await GetByIdAsync(id);
            if (album == null)
                throw new KeyNotFoundException($"Album with id {id} not found");

            var digitizations = await _digitizationService.GetByAlbumIdAsync(album.Id);
            return MapAlbumToAlbumDetailsVM(album, digitizations);
        }

        public async Task<Album> CreateOrFindAlbumAsync(string title, string artist, string genre)
        {
            var album = await FindByAlbumAndArtistAsync(title, artist);

            if (album is null)
            {
                album = new Album
                {
                    AddedDate = _timeProvider.GetLocalNow().LocalDateTime,
                    Title = title,
                    Artist = await FindOrCreateArtistAsync(artist),
                    Genre = await FindOrCreateGenreAsync(genre)
                };

                _context.Albums.Add(album);
                await _context.SaveChangesAsync();
            }

            return album;
        }

        public async Task<Album> UpdateAlbumAsync(int albumId, string title, string? artist, string? genre)
        {
            // Business logic: Validate album ID
            if (albumId <= 0)
                throw new InvalidDataException("AlbumId is invalid");

            // Business logic: Load existing album with tracking for updates
            var existing = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .FirstOrDefaultAsync(a => a.Id == albumId);
            
            if (existing == null)
                throw new KeyNotFoundException($"Album {albumId} not found");

            // Business logic: Check if nothing changed
            if (existing.Title == title &&
                (string.IsNullOrWhiteSpace(genre) || existing.Genre?.Name == genre) &&
                (string.IsNullOrWhiteSpace(artist) || existing.Artist?.Name == artist))
            {
                return existing;
            }

            // Business logic: Update properties
            existing.Title = title;
            existing.UpdateDate = _timeProvider.GetUtcNow().UtcDateTime;

            if (!string.IsNullOrWhiteSpace(genre))
            {
                var genreEntity = await FindOrCreateGenreAsync(genre);
                existing.GenreId = genreEntity.Id;
            }

            if (!string.IsNullOrWhiteSpace(artist))
            {
                var artistEntity = await FindOrCreateArtistAsync(artist);
                existing.ArtistId = artistEntity.Id;
            }

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAlbumAsync(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null)
                return false;

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();
            return true;
        }

        public Digitization MapViewModelToDigitization(int albumId, AlbumCreateUpdateViewModel request)
        {
            return new Digitization
            {
                AlbumId = albumId,
                AddedDate = _timeProvider.GetLocalNow().LocalDateTime,
                Source = request.Source,
                Discogs = request.Discogs,
                IsFirstPress = false,
                YearId = request.Year,
                ReissueId = request.Reissue,
                Country = !string.IsNullOrEmpty(request.Country) ? new Country { Name = request.Country } : null,
                Label = !string.IsNullOrEmpty(request.Label) ? new Label { Name = request.Label } : null,
                Storage = !string.IsNullOrEmpty(request.Storage) ? new Storage { Data = request.Storage } : null,

                FormatInfo = new FormatInfo
                {
                    BitnessId = request.Bitness,
                    Sampling = request.Sampling.HasValue ? new Sampling { Value = request.Sampling.Value } : null,
                    DigitalFormat = !string.IsNullOrEmpty(request.DigitalFormat) ? new DigitalFormat { Name = request.DigitalFormat } : null,
                    SourceFormat = !string.IsNullOrEmpty(request.SourceFormat) ? new SourceFormat { Name = request.SourceFormat } : null,
                    VinylState = !string.IsNullOrEmpty(request.VinylState) ? new VinylState { Name = request.VinylState } : null
                },

                EquipmentInfo = new EquipmentInfo
                {
                    Player = !string.IsNullOrEmpty(request.Player) ? new Player { Name = request.Player } : null,
                    Cartridge = !string.IsNullOrEmpty(request.Cartridge) ? new Cartridge { Name = request.Cartridge } : null,
                    Amplifier = !string.IsNullOrEmpty(request.Amplifier) ? new Amplifier { Name = request.Amplifier } : null,
                    Adc = !string.IsNullOrEmpty(request.Adc) ? new Adc { Name = request.Adc } : null,
                    Wire = !string.IsNullOrEmpty(request.Wire) ? new Wire { Name = request.Wire } : null
                }
            };
        }

        public AlbumDetailsViewModel MapAlbumToAlbumDetailsVM(Album album, IEnumerable<Digitization>? digitizations = null)
        {
            return new AlbumDetailsViewModel
            {
                AlbumId = album.Id,
                Title = album.Title,
                Artist = album.Artist?.Name ?? string.Empty,
                Genre = album.Genre?.Name ?? string.Empty,
                AddedDate = album.AddedDate,
                UpdateDate = album.UpdateDate,
                Digitizations = digitizations
            };
        }

        public async Task<AlbumCreateUpdateViewModel> MapAlbumToCreateUpdateVMAsync(Album album)
        {
            var digitizations = await _digitizationService.GetByAlbumIdAsync(album.Id);
            
            // Check if cover exists - only set AlbumCover if cover actually exists
            var coverUrl = await _imageService.GetCoverUrlAsync(album.Id, EntityType.AlbumCover);
            var albumCover = coverUrl.Contains("nocover") ? null : album.Id.ToString();
            
            return new AlbumCreateUpdateViewModel
            {
                AlbumId = album.Id,
                Title = album.Title,
                Artist = album.Artist?.Name ?? string.Empty,
                Genre = album.Genre?.Name ?? string.Empty,
                AlbumCover = albumCover, // Set album ID only if cover exists, otherwise null
                Action = ActionType.Update,
                Digitizations = digitizations.ToList()
            };
        }

        private async Task<Artist> FindOrCreateArtistAsync(string artistName)
        {
            var normalizedArtistName = artistName.Trim();

            var artist = await _context.Artists
                .FirstOrDefaultAsync(a => a.Name == normalizedArtistName);

            if (artist is null)
            {
                artist = new Artist { Name = artistName };
                _context.Artists.Add(artist);
                await _context.SaveChangesAsync();
            }

            return artist;
        }

        private async Task<Genre> FindOrCreateGenreAsync(string genreName)
        {
            var normalizedGenreName = genreName.Trim();

            var genre = await _context.Genres
                .FirstOrDefaultAsync(g => g.Name == normalizedGenreName);

            if (genre is null)
            {
                genre = new Genre { Name = genreName };
                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();
            }

            return genre;
        }
    }
}

