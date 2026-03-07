using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Enums;
using Web.Extentions;
using Web.Interfaces;
using Web.Models;

namespace Web.Implementation
{
    public class DigitizationRepository : IDigitizationRepository
    {
        private readonly DMADbContext _context;

        public DigitizationRepository(DMADbContext ctx) => _context = ctx;

        public async Task<PagedResult<Digitization>> GetListAsync(int page, int pageSize)
        {
            var query = _context.Digitizations
                .Include(d => d.Year)
                .Include(d => d.Reissue)
                .Include(d => d.Label)
                .Include(d => d.Country)
                .Include(d => d.Storage)
                .Include(d => d.Album).ThenInclude(a => a.Artist)
                .Include(d => d.Album).ThenInclude(a => a.Genre)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.Bitness)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.Sampling)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.DigitalFormat)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.SourceFormat)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.VinylState)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Player)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Cartridge)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Amplifier)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Adc)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Wire)
                .AsSplitQuery()
                .AsNoTracking()
                .AsQueryable();

            return await query.ToPagedResultAsync(page, pageSize, a => a.Id);
        }

        public async Task<Digitization?> GetByIdAsync(int id)
        {
            return await _context.Digitizations
                .Include(d => d.Year)
                .Include(d => d.Reissue)
                .Include(d => d.Label)
                .Include(d => d.Country)
                .Include(d => d.Storage)
                .Include(d => d.Album).ThenInclude(a => a.Artist)
                .Include(d => d.Album).ThenInclude(a => a.Genre)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.Bitness)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.Sampling)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.DigitalFormat)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.SourceFormat)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.VinylState)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Player).ThenInclude(p => p!.Manufacturer)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Cartridge).ThenInclude(c => c!.Manufacturer)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Amplifier).ThenInclude(a => a!.Manufacturer)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Adc).ThenInclude(a => a!.Manufacturer)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Wire).ThenInclude(w => w!.Manufacturer)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        
        public async Task<IEnumerable<Digitization>> GetByAlbumIdAsync(int albumId)
        {
            return await _context.Digitizations
                .Where(d => d.AlbumId == albumId)
                .Include(d => d.Year)
                .Include(d => d.Reissue)
                .Include(d => d.Label)
                .Include(d => d.Country)
                .Include(d => d.Storage)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.Bitness)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.Sampling)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.DigitalFormat)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.SourceFormat)
                .Include(d => d.FormatInfo).ThenInclude(f => f!.VinylState)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Player).ThenInclude(p => p!.Manufacturer)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Cartridge).ThenInclude(c => c!.Manufacturer)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Amplifier).ThenInclude(a => a!.Manufacturer)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Adc).ThenInclude(a => a!.Manufacturer)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Wire).ThenInclude(w => w!.Manufacturer)
                .AsSplitQuery()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Digitization> AddAsync(Digitization digitization)
        {
            _context.Digitizations.Add(digitization);
            await _context.SaveChangesAsync();
            return digitization;
        }

        public async Task<Digitization> UpdateAsync(Digitization digitization)
        {
            await _context.SaveChangesAsync();
            return digitization;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var digitization = await _context.Digitizations.FindAsync(id);
            if (digitization == null)
            {
                return false;
            }

            _context.Digitizations.Remove(digitization);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Digitization>> GetDigitizationsByEquipmentAsync(EntityType equipmentType, int equipmentId)
        {
            var query = _context.Digitizations
                .Include(d => d.Album).ThenInclude(a => a.Artist)
                .Where(d => d.EquipmentInfoId != null)
                .AsNoTracking();

            query = equipmentType switch
            {
                EntityType.Player => query.Where(d => d.EquipmentInfo!.PlayerId == equipmentId),
                EntityType.Cartridge => query.Where(d => d.EquipmentInfo!.CartridgeId == equipmentId),
                EntityType.Amplifier => query.Where(d => d.EquipmentInfo!.AmplifierId == equipmentId),
                EntityType.Adc => query.Where(d => d.EquipmentInfo!.AdcId == equipmentId),
                EntityType.Wire => query.Where(d => d.EquipmentInfo!.WireId == equipmentId),
                _ => query.Where(_ => false)
            };

            return await query.ToListAsync();
        }

        public async Task<PagedResult<Album>> GetAlbumsDigitizedByEquipmentPagedAsync(EntityType equipmentType, int equipmentId, int page, int pageSize)
        {
            if (page < 1) page = 1;

            var digitizationsQuery = _context.Digitizations
                .Where(d => d.EquipmentInfoId != null)
                .AsNoTracking();

            digitizationsQuery = equipmentType switch
            {
                EntityType.Player => digitizationsQuery.Where(d => d.EquipmentInfo!.PlayerId == equipmentId),
                EntityType.Cartridge => digitizationsQuery.Where(d => d.EquipmentInfo!.CartridgeId == equipmentId),
                EntityType.Amplifier => digitizationsQuery.Where(d => d.EquipmentInfo!.AmplifierId == equipmentId),
                EntityType.Adc => digitizationsQuery.Where(d => d.EquipmentInfo!.AdcId == equipmentId),
                EntityType.Wire => digitizationsQuery.Where(d => d.EquipmentInfo!.WireId == equipmentId),
                _ => digitizationsQuery.Where(_ => false)
            };

            var distinctAlbumIds = digitizationsQuery.Select(d => d.AlbumId).Distinct();
            var albumsQuery = _context.Albums
                .Where(a => distinctAlbumIds.Contains(a.Id))
                .Include(a => a.Artist)
                .AsNoTracking()
                .OrderBy(a => a.Artist!.Name)
                .ThenBy(a => a.Title);

            var totalItems = await albumsQuery.CountAsync();
            var items = await albumsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Album>(items, totalItems, page, pageSize);
        }

        public async Task<bool> ExistsByAlbumIdAsync(int albumId)
        {
            return await _context.Digitizations.AnyAsync(d => d.AlbumId == albumId);
        }

        public async Task<bool> ExistsByAlbumIdAndSourceAsync(int albumId, string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return false;

            return await _context.Digitizations
                .AnyAsync(d =>
                    d.AlbumId == albumId &&
                    d.Source != null &&
                    d.Source.Equals(source));
        }
    }
}
