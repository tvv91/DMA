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
            await _context.SaveChangesAsync();
            return album;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return false;
            }

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<Album>> SearchByTitleAsync(string title, int page, int pageSize)
        {
            var query = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .Where(a => a.Title.Contains(title))
                .AsNoTracking();

            return await query.ToPagedResultAsync(page, pageSize, a => a.Title);
        }

        public async Task<PagedResult<Album>> GetIndexListAsync(int page, int pageSize, string? artistName = null, string? genreName = null, string? yearValue = null, string? albumTitle = null)
        {
            var query = _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .AsNoTracking()
                .AsQueryable();

            // Apply filters
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
                // Try to parse year value and filter albums that have digitizations with this year
                if (int.TryParse(yearValue, out int yearInt))
                {
                    query = query.Where(a => _context.Digitizations.Any(d => d.AlbumId == a.Id && d.Year != null && d.Year.Value == yearInt));
                }
                else
                {
                    // If not a number, search by year value as string
                    query = query.Where(a => _context.Digitizations.Any(d => d.AlbumId == a.Id && d.Year != null && d.Year.Value.ToString().Contains(yearValue)));
                }
            }

            return await query.ToPagedResultAsync(page, pageSize, a => a.Id);
        }

        public async Task<Album?> FindByTitleAndArtistAsync(string title, string artist)
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Title.ToLower() == title.ToLower() && a.Artist.Name.ToLower() == artist.ToLower());
        }
    }
}
