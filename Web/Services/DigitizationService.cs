using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Web.Common;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Models;

namespace Web.Services
{
    public class DigitizationService(Context context, TimeProvider timeProvider) : IDigitizationService
    {
        private readonly Context _context = context;
        private readonly TimeProvider _timeProvider = timeProvider;

        public async Task<IEnumerable<Digitization>> GetByAlbumIdAsync(int albumId)
        {
            return await _context.Digitizations
                .AsNoTracking()
                .Where(d => d.AlbumId == albumId)
                .Select(DigitizationProjection)
                .ToListAsync();
        }

        public async Task<Digitization?> GetByIdAsync(int id)
        {
            return await _context.Digitizations
                .AsNoTracking()
                .Where(d => d.Id == id)
                .Select(DigitizationProjection)
                .FirstOrDefaultAsync();
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
            digitization.AddedDate = _timeProvider.GetLocalNow().LocalDateTime;
            _context.Digitizations.Add(digitization);
            await _context.SaveChangesAsync();
            return digitization;
        }

        public async Task<Digitization> UpdateAsync(Digitization digitization)
        {
            digitization.UpdateDate = _timeProvider.GetUtcNow().UtcDateTime;
            
            var existing = await _context.Digitizations
                .Include(d => d.FormatInfo)
                .Include(d => d.EquipmentInfo)
                .FirstOrDefaultAsync(d => d.Id == digitization.Id);

            if (existing is null)
                throw new KeyNotFoundException($"Digitization {digitization.Id} not found");

            if (digitization.FormatInfo is not null && existing.FormatInfo is null)
            {
                existing.FormatInfo = new FormatInfo();
                _context.FormatInfos.Add(existing.FormatInfo);
                await _context.SaveChangesAsync();
            }

            if (digitization.EquipmentInfo is not null && existing.EquipmentInfo is null)
            {
                existing.EquipmentInfo = new EquipmentInfo();
                _context.EquipmentInfos.Add(existing.EquipmentInfo);
                await _context.SaveChangesAsync();
            }

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

            if (digitization.FormatInfo is not null && existing.FormatInfo is not null)
            {
                existing.FormatInfo.BitnessId = digitization.FormatInfo.BitnessId;
                existing.FormatInfo.SamplingId = digitization.FormatInfo.SamplingId;
                existing.FormatInfo.DigitalFormatId = digitization.FormatInfo.DigitalFormatId;
                existing.FormatInfo.SourceFormatId = digitization.FormatInfo.SourceFormatId;
                existing.FormatInfo.VinylStateId = digitization.FormatInfo.VinylStateId;
            }

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
            if (digitization is null)
                return false;

            _context.Digitizations.Remove(digitization);
            await _context.SaveChangesAsync();
            return true;
        }

        private static readonly Expression<Func<Digitization, Digitization>> DigitizationProjection =
            x => new Digitization
            {
                Id = x.Id,
                AlbumId = x.AlbumId,
                AddedDate = x.AddedDate,
                UpdateDate = x.UpdateDate,
                Source = x.Source,
                Discogs = x.Discogs,
                IsFirstPress = x.IsFirstPress,
                Size = x.Size,
                CountryId = x.CountryId,
                Country = x.Country == null ? null : new Country { Id = x.Country.Id, Name = x.Country.Name },
                LabelId = x.LabelId,
                Label = x.Label == null ? null : new Label { Id = x.Label.Id, Name = x.Label.Name },
                ReissueId = x.ReissueId,
                Reissue = x.Reissue == null ? null : new Reissue { Id = x.Reissue.Id, Value = x.Reissue.Value },
                YearId = x.YearId,
                Year = x.Year == null ? null : new Year { Id = x.Year.Id, Value = x.Year.Value },
                StorageId = x.StorageId,
                Storage = x.Storage == null ? null : new Storage { Id = x.Storage.Id, Name = x.Storage.Name },
                FormatInfoId = x.FormatInfoId,
                FormatInfo = new FormatInfo
                {
                    Id = x.FormatInfoId ?? 0,
                    BitnessId = x.FormatInfo == null ? null : x.FormatInfo.BitnessId,
                    Bitness = x.FormatInfo == null || x.FormatInfo.Bitness == null
                        ? null
                        : new Bitness { Id = x.FormatInfo.Bitness.Id, Value = x.FormatInfo.Bitness.Value },
                    SamplingId = x.FormatInfo == null ? null : x.FormatInfo.SamplingId,
                    Sampling = x.FormatInfo == null || x.FormatInfo.Sampling == null
                        ? null
                        : new Sampling { Id = x.FormatInfo.Sampling.Id, Value = x.FormatInfo.Sampling.Value },
                    DigitalFormatId = x.FormatInfo == null ? null : x.FormatInfo.DigitalFormatId,
                    DigitalFormat = x.FormatInfo == null || x.FormatInfo.DigitalFormat == null
                        ? null
                        : new DigitalFormat { Id = x.FormatInfo.DigitalFormat.Id, Name = x.FormatInfo.DigitalFormat.Name },
                    SourceFormatId = x.FormatInfo == null ? null : x.FormatInfo.SourceFormatId,
                    SourceFormat = x.FormatInfo == null || x.FormatInfo.SourceFormat == null
                        ? null
                        : new SourceFormat { Id = x.FormatInfo.SourceFormat.Id, Name = x.FormatInfo.SourceFormat.Name },
                    VinylStateId = x.FormatInfo == null ? null : x.FormatInfo.VinylStateId,
                    VinylState = x.FormatInfo == null || x.FormatInfo.VinylState == null
                        ? null
                        : new VinylState { Id = x.FormatInfo.VinylState.Id, Name = x.FormatInfo.VinylState.Name }
                },
                EquipmentInfoId = x.EquipmentInfoId,
                EquipmentInfo = new EquipmentInfo
                {
                    Id = x.EquipmentInfoId ?? 0,
                    PlayerId = x.EquipmentInfo == null ? null : x.EquipmentInfo.PlayerId,
                    Player = new Player
                    {
                        Id = x.EquipmentInfo == null ? 0 : (x.EquipmentInfo.PlayerId ?? 0),
                        Name = x.EquipmentInfo == null || x.EquipmentInfo.Player == null ? string.Empty : x.EquipmentInfo.Player.Name,
                        ManufacturerId = x.EquipmentInfo == null || x.EquipmentInfo.Player == null ? null : x.EquipmentInfo.Player.ManufacturerId,
                        Manufacturer = x.EquipmentInfo == null || x.EquipmentInfo.Player == null || x.EquipmentInfo.Player.Manufacturer == null
                            ? null
                            : new Manufacturer
                            {
                                Id = x.EquipmentInfo.Player.Manufacturer.Id,
                                Name = x.EquipmentInfo.Player.Manufacturer.Name,
                                Type = x.EquipmentInfo.Player.Manufacturer.Type
                            }
                    },
                    CartridgeId = x.EquipmentInfo == null ? null : x.EquipmentInfo.CartridgeId,
                    Cartridge = new Cartridge
                    {
                        Id = x.EquipmentInfo == null ? 0 : (x.EquipmentInfo.CartridgeId ?? 0),
                        Name = x.EquipmentInfo == null || x.EquipmentInfo.Cartridge == null ? string.Empty : x.EquipmentInfo.Cartridge.Name,
                        ManufacturerId = x.EquipmentInfo == null || x.EquipmentInfo.Cartridge == null ? null : x.EquipmentInfo.Cartridge.ManufacturerId,
                        Manufacturer = x.EquipmentInfo == null || x.EquipmentInfo.Cartridge == null || x.EquipmentInfo.Cartridge.Manufacturer == null
                            ? null
                            : new Manufacturer
                            {
                                Id = x.EquipmentInfo.Cartridge.Manufacturer.Id,
                                Name = x.EquipmentInfo.Cartridge.Manufacturer.Name,
                                Type = x.EquipmentInfo.Cartridge.Manufacturer.Type
                            }
                    },
                    AmplifierId = x.EquipmentInfo == null ? null : x.EquipmentInfo.AmplifierId,
                    Amplifier = new Amplifier
                    {
                        Id = x.EquipmentInfo == null ? 0 : (x.EquipmentInfo.AmplifierId ?? 0),
                        Name = x.EquipmentInfo == null || x.EquipmentInfo.Amplifier == null ? string.Empty : x.EquipmentInfo.Amplifier.Name,
                        ManufacturerId = x.EquipmentInfo == null || x.EquipmentInfo.Amplifier == null ? null : x.EquipmentInfo.Amplifier.ManufacturerId,
                        Manufacturer = x.EquipmentInfo == null || x.EquipmentInfo.Amplifier == null || x.EquipmentInfo.Amplifier.Manufacturer == null
                            ? null
                            : new Manufacturer
                            {
                                Id = x.EquipmentInfo.Amplifier.Manufacturer.Id,
                                Name = x.EquipmentInfo.Amplifier.Manufacturer.Name,
                                Type = x.EquipmentInfo.Amplifier.Manufacturer.Type
                            }
                    },
                    AdcId = x.EquipmentInfo == null ? null : x.EquipmentInfo.AdcId,
                    Adc = new Adc
                    {
                        Id = x.EquipmentInfo == null ? 0 : (x.EquipmentInfo.AdcId ?? 0),
                        Name = x.EquipmentInfo == null || x.EquipmentInfo.Adc == null ? string.Empty : x.EquipmentInfo.Adc.Name,
                        ManufacturerId = x.EquipmentInfo == null || x.EquipmentInfo.Adc == null ? null : x.EquipmentInfo.Adc.ManufacturerId,
                        Manufacturer = x.EquipmentInfo == null || x.EquipmentInfo.Adc == null || x.EquipmentInfo.Adc.Manufacturer == null
                            ? null
                            : new Manufacturer
                            {
                                Id = x.EquipmentInfo.Adc.Manufacturer.Id,
                                Name = x.EquipmentInfo.Adc.Manufacturer.Name,
                                Type = x.EquipmentInfo.Adc.Manufacturer.Type
                            }
                    },
                    WireId = x.EquipmentInfo == null ? null : x.EquipmentInfo.WireId,
                    Wire = new Wire
                    {
                        Id = x.EquipmentInfo == null ? 0 : (x.EquipmentInfo.WireId ?? 0),
                        Name = x.EquipmentInfo == null || x.EquipmentInfo.Wire == null ? string.Empty : x.EquipmentInfo.Wire.Name,
                        ManufacturerId = x.EquipmentInfo == null || x.EquipmentInfo.Wire == null ? null : x.EquipmentInfo.Wire.ManufacturerId,
                        Manufacturer = x.EquipmentInfo == null || x.EquipmentInfo.Wire == null || x.EquipmentInfo.Wire.Manufacturer == null
                            ? null
                            : new Manufacturer
                            {
                                Id = x.EquipmentInfo.Wire.Manufacturer.Id,
                                Name = x.EquipmentInfo.Wire.Manufacturer.Name,
                                Type = x.EquipmentInfo.Wire.Manufacturer.Type
                            }
                    }
                }
            };
    }
}

