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
                .Include(d => d.Album)
                    .ThenInclude(a => a.Artist)
                .Include(d => d.Album)
                    .ThenInclude(a => a.Genre)
                .Include(d => d.Format).ThenInclude(f => f.Bitness)
                .Include(d => d.Format).ThenInclude(f => f.Sampling)
                .Include(d => d.Format).ThenInclude(f => f.DigitalFormat)
                .Include(d => d.Format).ThenInclude(f => f.SourceFormat)
                .Include(d => d.Format).ThenInclude(f => f.VinylState)
                .Include(d => d.Equipment).ThenInclude(e => e.Player)
                .Include(d => d.Equipment).ThenInclude(e => e.Cartridge)
                .Include(d => d.Equipment).ThenInclude(e => e.Amplifier)
                .Include(d => d.Equipment).ThenInclude(e => e.Adc)
                .Include(d => d.Equipment).ThenInclude(e => e.Wire)
                .AsQueryable();

            return await query.ToPagedResultAsync(page, pageSize, a => a.Id);
        }

        public async Task<Digitization?> GetByIdAsync(int id)
        {
            return await _context.Digitizations
                .Include(d => d.Album)
                    .ThenInclude(a => a.Artist)
                .Include(d => d.Album)
                    .ThenInclude(a => a.Genre)
                .Include(d => d.Format).ThenInclude(f => f.Bitness)
                .Include(d => d.Format).ThenInclude(f => f.Sampling)
                .Include(d => d.Format).ThenInclude(f => f.DigitalFormat)
                .Include(d => d.Format).ThenInclude(f => f.SourceFormat)
                .Include(d => d.Format).ThenInclude(f => f.VinylState)
                .Include(d => d.Equipment).ThenInclude(e => e.Player)
                .Include(d => d.Equipment).ThenInclude(e => e.Cartridge)
                .Include(d => d.Equipment).ThenInclude(e => e.Amplifier)
                .Include(d => d.Equipment).ThenInclude(e => e.Adc)
                .Include(d => d.Equipment).ThenInclude(e => e.Wire)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<PagedResult<Digitization>> GetByAlbumIdAsync(int albumId, int page, int pageSize)
        {
            var query = _context.Digitizations
                .Where(d => d.AlbumId == albumId)
                .Include(d => d.Format)
                .Include(d => d.Equipment)
                .AsQueryable();

            return await query.ToPagedResultAsync(page, pageSize, a => a.Id);
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
                .Include(d => d.Format)
                .Include(d => d.Equipment)
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

            if (digitization.Format is not null)
            {
                existing.Format.Size = digitization.Format.Size;
                existing.Format.BitnessId = digitization.Format.BitnessId;
                existing.Format.SamplingId = digitization.Format.SamplingId;
                existing.Format.DigitalFormatId = digitization.Format.DigitalFormatId;
                existing.Format.SourceFormatId = digitization.Format.SourceFormatId;
                existing.Format.VinylStateId = digitization.Format.VinylStateId;
            }

            if (digitization.Equipment is not null)
            {
                existing.Equipment.PlayerId = digitization.Equipment.PlayerId;
                existing.Equipment.CartridgeId = digitization.Equipment.CartridgeId;
                existing.Equipment.AmplifierId = digitization.Equipment.AmplifierId;
                existing.Equipment.AdcId = digitization.Equipment.AdcId;
                existing.Equipment.WireId = digitization.Equipment.WireId;
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
