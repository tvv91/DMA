using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Enums;
using Web.Extentions;
using Web.Interfaces;
using Web.Models;

namespace Web.Services
{
    public class DigitizationService : IDigitizationService
    {
        private readonly DMADbContext _context;

        public DigitizationService(DMADbContext context)
        {
            _context = context;
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

        public async Task<bool> ExistsByAlbumIdAndSourceAsync(int albumId, string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return false;

            return await _context.Digitizations.AnyAsync(d =>
                d.AlbumId == albumId &&
                d.Source != null &&
                d.Source.Equals(source));
        }

        public async Task<Digitization> AddAsync(Digitization digitization)
        {
            digitization.AddedDate = DateTime.Now;
            _context.Digitizations.Add(digitization);
            await _context.SaveChangesAsync();
            return digitization;
        }

        public async Task<Digitization> UpdateAsync(Digitization digitization)
        {
            digitization.UpdateDate = DateTime.UtcNow;
            
            // Business logic: Load existing digitization with related entities
            var existing = await _context.Digitizations
                .Include(d => d.FormatInfo)
                .Include(d => d.EquipmentInfo)
                .FirstOrDefaultAsync(d => d.Id == digitization.Id);

            if (existing == null)
                throw new KeyNotFoundException($"Digitization {digitization.Id} not found");

            // Business logic: Create FormatInfo if it doesn't exist but is being updated
            if (digitization.FormatInfo is not null && existing.FormatInfo == null)
            {
                existing.FormatInfo = new FormatInfo();
                _context.FormatInfos.Add(existing.FormatInfo);
                await _context.SaveChangesAsync(); // Save to get the ID
            }

            // Business logic: Create EquipmentInfo if it doesn't exist but is being updated
            if (digitization.EquipmentInfo is not null && existing.EquipmentInfo == null)
            {
                existing.EquipmentInfo = new EquipmentInfo();
                _context.EquipmentInfos.Add(existing.EquipmentInfo);
                await _context.SaveChangesAsync(); // Save to get the ID
            }

            // Business logic: Update basic properties
            existing.AlbumId = digitization.AlbumId;
            existing.Source = digitization.Source;
            existing.Discogs = digitization.Discogs;
            existing.IsFirstPress = digitization.IsFirstPress;
            existing.CountryId = digitization.CountryId;
            existing.LabelId = digitization.LabelId;
            existing.ReissueId = digitization.ReissueId;
            existing.YearId = digitization.YearId;
            existing.StorageId = digitization.StorageId;
            existing.Size = digitization.Size;
            existing.UpdateDate = digitization.UpdateDate;

            // Business logic: Update FormatInfo - FormatInfo should already exist (created above if needed)
            if (digitization.FormatInfo is not null && existing.FormatInfo is not null)
            {
                existing.FormatInfo.BitnessId = digitization.FormatInfo.BitnessId;
                existing.FormatInfo.SamplingId = digitization.FormatInfo.SamplingId;
                existing.FormatInfo.DigitalFormatId = digitization.FormatInfo.DigitalFormatId;
                existing.FormatInfo.SourceFormatId = digitization.FormatInfo.SourceFormatId;
                existing.FormatInfo.VinylStateId = digitization.FormatInfo.VinylStateId;
            }

            // Business logic: Update EquipmentInfo - EquipmentInfo should already exist (created above if needed)
            if (digitization.EquipmentInfo is not null && existing.EquipmentInfo is not null)
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
            var digitization = await _context.Digitizations.FindAsync(id);
            if (digitization == null)
                return false;

            _context.Digitizations.Remove(digitization);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

