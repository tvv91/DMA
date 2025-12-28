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
                .AsQueryable();

            return await query.ToPagedResultAsync(page, pageSize, a => a.Id);
        }

        public async Task<Album?> GetByIdAsync(int id)
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
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

            existing.Title = album.Title;
            existing.UpdateDate = album.UpdateDate;
            existing.ArtistId = album.ArtistId;
            existing.GenreId = album.GenreId;

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
