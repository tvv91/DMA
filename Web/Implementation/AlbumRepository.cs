using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Extentions;
using Web.Interfaces;
using Web.Models;

namespace Web.Implementation
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly DMADbContext _context;

        public AlbumRepository(DMADbContext ctx) => _context = ctx;

        public async Task<PagedResult<Album>> GetListAsync(int page, int pageSize)
        {
            var query = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .Include(a => a.Digitizations)
                .AsQueryable();

            return await query.ToPagedResultAsync(page, pageSize, a => a.Id);
        }

        public async Task<Album?> GetByIdAsync(int id)
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .Include(a => a.Digitizations)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Album> AddAsync(Album album)
        {
            _context.Albums.Add(album);
            await _context.SaveChangesAsync();
            return album;
        }

        public async Task<Album> UpdateAsync(Album album)
        {
            if (album.Id <= 0)
                throw new InvalidDataException("AlbumId is invalid");

            var existing = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .FirstOrDefaultAsync(a => a.Id == album.Id);

            if (existing == null)
                throw new KeyNotFoundException($"Album {album.Id} not found");

            // return existed object if nothing to update
            if (existing.Title == album.Title && (string.IsNullOrWhiteSpace(album?.Genre?.Name) || existing.Genre?.Name == album.Genre.Name) && (string.IsNullOrWhiteSpace(album?.Artist?.Name) || existing.Artist?.Name == album.Artist.Name))
                return existing;

            existing.Title = album.Title;
            existing.UpdateDate = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(album?.Genre?.Name))
            {
                var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == album.Genre.Name);

                if (genre is null)
                {
                    genre = new Genre { Name = album.Genre.Name };
                    _context.Genres.Add(genre);
                }

                existing.GenreId = genre.Id;
            }

            if (!string.IsNullOrWhiteSpace(album?.Artist?.Name))
            {
                var artist = await _context.Artists
                    .FirstOrDefaultAsync(a => a.Name == album.Artist.Name);

                if (artist is null)
                {
                    artist = new Artist { Name = album.Artist.Name };
                    _context.Artists.Add(artist);
                }

                existing.ArtistId = artist.Id;
            }

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var affected = await _context.Albums
                .Where(a => a.Id == id)
                .ExecuteDeleteAsync();

            return affected > 0;
        }

        public async Task<PagedResult<Album>> SearchByTitleAsync(string title, int page, int pageSize)
        {
            var query = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .Include(a => a.Digitizations)
                .Where(a => a.Title.Contains(title));

            return await query.ToPagedResultAsync(page, pageSize, a => a.Title);
        }

        public async Task<PagedResult<Album>> GetIndexListAsync(int page, int pageSize)
        {
            var query = _context.Albums
                .Include(a => a.Artist)
                .AsQueryable();

            return await query.ToPagedResultAsync(page, pageSize, a => a.Id);
        }

        public async Task<Album?> FindByTitleAndArtistAsync(string title, string artist)
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .FirstOrDefaultAsync(a =>a.Title.ToLower() == title.ToLower() && a.Artist.Name.ToLower() == artist.ToLower());
        }
    }
}
