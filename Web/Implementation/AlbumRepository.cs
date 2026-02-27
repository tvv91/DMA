using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Enums;
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

        public async Task<PagedResult<Album>> GetAlbumsByEquipmentAsync(EntityType equipmentType, int equipmentId, int page, int pageSize)
        {
            IQueryable<int> eqInfoIdsQuery = equipmentType switch
            {
                EntityType.Player => _context.EquipmentInfos.Where(e => e.PlayerId == equipmentId).Select(e => e.Id),
                EntityType.Cartridge => _context.EquipmentInfos.Where(e => e.CartridgeId == equipmentId).Select(e => e.Id),
                EntityType.Amplifier => _context.EquipmentInfos.Where(e => e.AmplifierId == equipmentId).Select(e => e.Id),
                EntityType.Adc => _context.EquipmentInfos.Where(e => e.AdcId == equipmentId).Select(e => e.Id),
                EntityType.Wire => _context.EquipmentInfos.Where(e => e.WireId == equipmentId).Select(e => e.Id),
                _ => _context.EquipmentInfos.Where(e => false).Select(e => e.Id)
            };

            var albumIdsQuery = _context.Digitizations
                .Where(d => d.EquipmentInfoId != null && eqInfoIdsQuery.Contains(d.EquipmentInfoId!.Value))
                .Select(d => d.AlbumId)
                .Distinct()
                .OrderBy(id => id);

            var totalCount = await albumIdsQuery.CountAsync();
            var pagedAlbumIds = await albumIdsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (pagedAlbumIds.Count == 0)
                return new PagedResult<Album>(new List<Album>(), totalCount, page, pageSize);

            var albums = await _context.Albums
                .Include(a => a.Artist)
                .Include(a => a.Genre)
                .Where(a => pagedAlbumIds.Contains(a.Id))
                .AsNoTracking()
                .ToListAsync();

            var ordered = pagedAlbumIds.Select(id => albums.First(a => a.Id == id)).ToList();
            return new PagedResult<Album>(ordered, totalCount, page, pageSize);
        }

        public async Task<Album?> FindByAlbumAndArtistAsync(string album, string artist)
        {
            return await _context.Albums
                .Include(a => a.Artist)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Title.ToLower() == album.ToLower() && a.Artist.Name.ToLower() == artist.ToLower());
        }
    }
}
