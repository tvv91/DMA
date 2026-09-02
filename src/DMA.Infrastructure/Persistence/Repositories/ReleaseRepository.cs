using DMA.Domain.Albums;
using DMA.Domain.Common;
using DMA.Domain.Equipment;
using DMA.Domain.ReferenceData;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DMA.Infrastructure.Persistence.Repositories;

public class ReleaseRepository(DmaDbContext context) : IReleaseRepository
{
    private readonly DmaDbContext _context = context;

    public async Task<IEnumerable<Release>> GetByAlbumIdAsync(int albumId, CancellationToken cancellationToken = default) =>
        await _context.Releases
            .AsNoTracking()
            .Where(d => d.AlbumId == albumId)
            .Select(ReleaseProjection)
            .ToListAsync(cancellationToken);

    public Task<Release?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Releases
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(ReleaseProjection)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<PagedResult<Album>> GetAlbumsReleasedByEquipmentPagedAsync(EntityType equipmentType, int equipmentId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page < 1)
            page = 1;

        var releasesQuery = _context.Releases
            .Where(d => d.EquipmentInfoId != null)
            .AsNoTracking();

        releasesQuery = equipmentType switch
        {
            EntityType.Player => releasesQuery.Where(d => d.EquipmentInfo!.PlayerId == equipmentId),
            EntityType.Cartridge => releasesQuery.Where(d => d.EquipmentInfo!.CartridgeId == equipmentId),
            EntityType.Amplifier => releasesQuery.Where(d => d.EquipmentInfo!.AmplifierId == equipmentId),
            EntityType.Adc => releasesQuery.Where(d => d.EquipmentInfo!.AdcId == equipmentId),
            EntityType.Wire => releasesQuery.Where(d => d.EquipmentInfo!.WireId == equipmentId),
            _ => releasesQuery.Where(_ => false)
        };

        var distinctAlbumIds = releasesQuery.Select(d => d.AlbumId).Distinct();
        var albumsQuery = _context.Albums
            .Where(a => distinctAlbumIds.Contains(a.Id))
            .Include(a => a.Artist)
            .AsNoTracking()
            .OrderBy(a => a.Artist!.Name)
            .ThenBy(a => a.Title);

        var totalItems = await albumsQuery.CountAsync(cancellationToken);
        var items = await albumsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Album>(items, totalItems, page, pageSize);
    }

    public Task<bool> ExistsByAlbumIdAndSourceAsync(int albumId, string source, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(source))
            return Task.FromResult(false);

        return _context.Releases.AnyAsync(d =>
            d.AlbumId == albumId &&
            d.Source != null &&
            d.Source.Equals(source), cancellationToken);
    }

    public void Add(Release release) => _context.Releases.Add(release);

    public Task<Release?> FindTrackedWithDetailsAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Releases
            .Include(d => d.FormatInfo)
            .Include(d => d.EquipmentInfo)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

    public void AddFormatInfo(FormatInfo formatInfo) => _context.FormatInfos.Add(formatInfo);

    public void AddEquipmentInfo(EquipmentInfo equipmentInfo) => _context.EquipmentInfos.Add(equipmentInfo);

    public Task<Release?> FindAsync(int id, CancellationToken cancellationToken = default) =>
        _context.Releases.FindAsync([id], cancellationToken).AsTask();

    public void Remove(Release release) => _context.Releases.Remove(release);

    private static readonly Expression<Func<Release, Release>> ReleaseProjection =
        x => new Release
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
                            Name = x.EquipmentInfo.Player.Manufacturer.Name
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
                            Name = x.EquipmentInfo.Cartridge.Manufacturer.Name
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
                            Name = x.EquipmentInfo.Amplifier.Manufacturer.Name
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
                            Name = x.EquipmentInfo.Adc.Manufacturer.Name
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
                            Name = x.EquipmentInfo.Wire.Manufacturer.Name
                        }
                }
            }
        };
}
