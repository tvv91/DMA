using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
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
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Player)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Cartridge)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Amplifier)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Adc)
                .Include(d => d.EquipmentInfo).ThenInclude(e => e!.Wire)
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
            var existing = await _context.Digitizations
                .Include(d => d.FormatInfo)
                .Include(d => d.EquipmentInfo)
                .FirstOrDefaultAsync(d => d.Id == digitization.Id);

            if (existing == null)
                throw new KeyNotFoundException($"Digitization {digitization.Id} not found");

            existing.AlbumId = digitization.AlbumId;
            existing.Source = digitization.Source;
            existing.Discogs = digitization.Discogs;
            existing.IsFirstPress = digitization.IsFirstPress;
            existing.CountryId = digitization.CountryId;
            existing.LabelId = digitization.LabelId;
            existing.ReissueId = digitization.ReissueId;
            existing.YearId = digitization.YearId;
            existing.StorageId = digitization.StorageId;
            existing.UpdateDate = DateTime.UtcNow;

            if (digitization.FormatInfo is not null)
            {
                existing.FormatInfo.Size = digitization.FormatInfo.Size;
                existing.FormatInfo.BitnessId = digitization.FormatInfo.BitnessId;
                existing.FormatInfo.SamplingId = digitization.FormatInfo.SamplingId;
                existing.FormatInfo.DigitalFormatId = digitization.FormatInfo.DigitalFormatId;
                existing.FormatInfo.SourceFormatId = digitization.FormatInfo.SourceFormatId;
                existing.FormatInfo.VinylStateId = digitization.FormatInfo.VinylStateId;
            }

            if (digitization.EquipmentInfo is not null)
            {
                existing.EquipmentInfo.PlayerId = digitization.EquipmentInfo.PlayerId;
                existing.EquipmentInfo.CartridgeId = digitization.EquipmentInfo.CartridgeId;
                existing.EquipmentInfo.AmplifierId = digitization.EquipmentInfo.AmplifierId;
                existing.EquipmentInfo.AdcId = digitization.EquipmentInfo.AdcId;
                existing.EquipmentInfo.WireId = digitization.EquipmentInfo.WireId;
            }

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var affected = await _context.Digitizations
                .Where(d => d.Id == id)
                .ExecuteDeleteAsync();

            return affected > 0;
        }        
    }
}
