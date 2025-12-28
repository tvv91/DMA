using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Interfaces;
using Web.Models;

namespace Web.Services
{
    public class DigitizationService : IDigitizationService
    {
        private readonly IDigitizationRepository _digitizationRepository;
        private readonly DMADbContext _context;

        public DigitizationService(IDigitizationRepository digitizationRepository, DMADbContext context)
        {
            _digitizationRepository = digitizationRepository;
            _context = context;
        }

        public async Task<IEnumerable<Digitization>> GetByAlbumIdAsync(int albumId)
        {
            return await _digitizationRepository.GetByAlbumIdAsync(albumId);
        }

        public async Task<Digitization> AddAsync(Digitization digitization)
        {
            digitization.AddedDate = DateTime.Now;
            return await _digitizationRepository.AddAsync(digitization);
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
            existing.UpdateDate = digitization.UpdateDate;

            // Business logic: Update FormatInfo - FormatInfo should already exist (created above if needed)
            if (digitization.FormatInfo is not null && existing.FormatInfo is not null)
            {
                existing.FormatInfo.Size = digitization.FormatInfo.Size;
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

            // Repository only saves changes
            return await _digitizationRepository.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _digitizationRepository.DeleteAsync(id);
        }
    }
}

