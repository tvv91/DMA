using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Enums;
using Web.Extentions;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;

namespace Web.Features.Equipment;

internal static class EquipmentFeatureHelpers
{
    internal static IQueryable<IManufacturer> Query(Context context, EntityType type) => type switch
    {
        EntityType.Adc => context.Set<Adc>().Include(x => x.Manufacturer),
        EntityType.Player => context.Set<Player>().Include(x => x.Manufacturer),
        EntityType.Amplifier => context.Set<Amplifier>().Include(x => x.Manufacturer),
        EntityType.Cartridge => context.Set<Cartridge>().Include(x => x.Manufacturer),
        EntityType.Wire => context.Set<Wire>().Include(x => x.Manufacturer),
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };

    internal static async Task<IManufacturer?> GetByIdAsync(Context context, int id, EntityType type) =>
        await Query(context, type).FirstOrDefaultAsync(x => x.Id == id);

    internal static EquipmentViewModel ToViewModel(IManufacturer item, EntityType type, string? imageUrl) => new()
    {
        Id = item.Id,
        ModelName = item.Name,
        Description = item.Description,
        EquipmentType = type,
        EquipmentCover = imageUrl,
        Manufacturer = item.Manufacturer?.Name
    };

    internal static async Task<Manufacturer?> FindOrCreateManufacturerAsync(Context context, string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        var normalized = name.Trim();
        var existing = await context.Manufacturer.FirstOrDefaultAsync(m => m.Name == normalized);
        if (existing is not null) return existing;
        var manufacturer = new Manufacturer { Name = normalized };
        context.Manufacturer.Add(manufacturer);
        await context.SaveChangesAsync();
        return manufacturer;
    }

    internal static async Task<IManufacturer> FromViewModelAsync(Context context, EquipmentViewModel request) => request.EquipmentType switch
    {
        EntityType.Adc => new Adc { Id = request.Id, Name = request.ModelName, Description = request.Description, Manufacturer = await FindOrCreateManufacturerAsync(context, request.Manufacturer) },
        EntityType.Amplifier => new Amplifier { Id = request.Id, Name = request.ModelName, Description = request.Description, Manufacturer = await FindOrCreateManufacturerAsync(context, request.Manufacturer) },
        EntityType.Cartridge => new Cartridge { Id = request.Id, Name = request.ModelName, Description = request.Description, Manufacturer = await FindOrCreateManufacturerAsync(context, request.Manufacturer) },
        EntityType.Player => new Player { Id = request.Id, Name = request.ModelName, Description = request.Description, Manufacturer = await FindOrCreateManufacturerAsync(context, request.Manufacturer) },
        EntityType.Wire => new Wire { Id = request.Id, Name = request.ModelName, Description = request.Description, Manufacturer = await FindOrCreateManufacturerAsync(context, request.Manufacturer) },
        _ => throw new ArgumentOutOfRangeException(nameof(request.EquipmentType), request.EquipmentType, "Unknown equipment type")
    };

    internal static async Task<PagedResult<Album>> GetReleasedAlbumsAsync(Context context, EntityType type, int id, int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        var releases = context.Releases.Where(r => r.EquipmentInfoId != null).AsNoTracking();
        releases = type switch
        {
            EntityType.Player => releases.Where(r => r.EquipmentInfo!.PlayerId == id),
            EntityType.Cartridge => releases.Where(r => r.EquipmentInfo!.CartridgeId == id),
            EntityType.Amplifier => releases.Where(r => r.EquipmentInfo!.AmplifierId == id),
            EntityType.Adc => releases.Where(r => r.EquipmentInfo!.AdcId == id),
            EntityType.Wire => releases.Where(r => r.EquipmentInfo!.WireId == id),
            _ => releases.Where(_ => false)
        };
        var albums = context.Albums.Where(a => releases.Select(r => r.AlbumId).Distinct().Contains(a.Id))
            .Include(a => a.Artist).AsNoTracking().OrderBy(a => a.Artist!.Name).ThenBy(a => a.Title);
        var total = await albums.CountAsync();
        return new PagedResult<Album>(await albums.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(), total, page, pageSize);
    }
}
