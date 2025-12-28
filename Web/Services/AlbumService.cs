using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;

namespace Web.Services
{
    public class AlbumService : IAlbumService
    {
        private readonly IAlbumRepository _albumRepository;
        private readonly IDigitizationService _digitizationService;
        private readonly DMADbContext _context;

        public AlbumService(IAlbumRepository albumRepository, IDigitizationService digitizationService, DMADbContext context)
        {
            _albumRepository = albumRepository;
            _digitizationService = digitizationService;
            _context = context;
        }

        public async Task<PagedResult<Album>> GetIndexListAsync(int page, int pageSize)
        {
            return await _albumRepository.GetIndexListAsync(page, pageSize);
        }

        public async Task<Album?> GetByIdAsync(int id)
        {
            return await _albumRepository.GetByIdAsync(id);
        }

        public async Task<AlbumDetailsViewModel> GetAlbumDetailsAsync(int id)
        {
            var album = await _albumRepository.GetByIdAsync(id);
            if (album == null)
                throw new KeyNotFoundException($"Album with id {id} not found");

            var digitizations = await _digitizationService.GetByAlbumIdAsync(album.Id);
            return MapAlbumToAlbumDetailsVM(album, digitizations);
        }

        public async Task<Album> CreateOrFindAlbumAsync(string title, string artist, string genre)
        {
            var album = await _albumRepository.FindByTitleAndArtistAsync(title, artist);

            if (album is null)
            {
                album = new Album
                {
                    AddedDate = DateTime.Now,
                    Title = title,
                    Artist = await FindOrCreateArtistAsync(artist),
                    Genre = await FindOrCreateGenreAsync(genre)
                };

                album = await _albumRepository.AddAsync(album);
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
            existing.UpdateDate = DateTime.UtcNow;

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

            // Repository only saves changes
            return await _albumRepository.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAlbumAsync(int id)
        {
            return await _albumRepository.DeleteAsync(id);
        }

        public Digitization MapViewModelToDigitization(int albumId, AlbumCreateUpdateViewModel request)
        {
            return new Digitization
            {
                AlbumId = albumId,
                AddedDate = DateTime.Now,
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
                    Size = request.Size,
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
            
            return new AlbumCreateUpdateViewModel
            {
                AlbumId = album.Id,
                Title = album.Title,
                Artist = album.Artist?.Name ?? string.Empty,
                Genre = album.Genre?.Name ?? string.Empty,
                Action = ActionType.Update,
                Digitizations = digitizations.ToList()
            };
        }

        private async Task<Artist> FindOrCreateArtistAsync(string artistName)
        {
            var artist = await _context.Artists
                .FirstOrDefaultAsync(a => a.Name.ToLower() == artistName.ToLower());

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
            var genre = await _context.Genres
                .FirstOrDefaultAsync(g => g.Name.ToLower() == genreName.ToLower());

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

