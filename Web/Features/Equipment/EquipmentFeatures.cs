using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;

namespace Web.Features.Equipment;

public sealed record GetEquipmentQuery(EntityType Category, int Id, string? Tab, int Page, int PageSize) : IRequest<EquipmentViewModel?>;
public sealed record GetEquipmentAlbumsQuery(EntityType Category, int Id, int Page, int PageSize) : IRequest<EquipmentReleasedAlbumsPageViewModel?>;
public sealed record CreateEquipmentCommand(EquipmentViewModel Request) : IRequest<int>;
public sealed record UpdateEquipmentCommand(EquipmentViewModel Request) : IRequest<int>;
public sealed record DeleteEquipmentCommand(EntityType Category, int Id) : IRequest<bool>;
public sealed record CreateEquipmentFormQuery : IRequest<EquipmentViewModel>;
public sealed record EquipmentPageQuery : IRequest<Unit>;
public sealed record GetEquipmentHubPageQuery(EntityType Category, int Page, int PageSize) : IRequest<PagedResult<IEquipment>>;
public sealed record FindEquipmentManufacturerQuery(EntityType Category, string Name) : IRequest<string?>;

public sealed class GetEquipmentHubPageQueryHandler(Context context) : IRequestHandler<GetEquipmentHubPageQuery, PagedResult<IEquipment>>
{
    public async Task<PagedResult<IEquipment>> Handle(GetEquipmentHubPageQuery request, CancellationToken cancellationToken)
    {
        return request.Category switch
        {
            EntityType.Adc => await Load(context.Set<Adc>(), request, cancellationToken),
            EntityType.Player => await Load(context.Set<Player>(), request, cancellationToken),
            EntityType.Amplifier => await Load(context.Set<Amplifier>(), request, cancellationToken),
            EntityType.Cartridge => await Load(context.Set<Cartridge>(), request, cancellationToken),
            EntityType.Wire => await Load(context.Set<Wire>(), request, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(request.Category))
        };
    }

    private static async Task<PagedResult<IEquipment>> Load<T>(DbSet<T> set, GetEquipmentHubPageQuery request, CancellationToken cancellationToken)
        where T : class, IEquipment
    {
        var query = set.Include(x => x.Manufacturer).OrderBy(x => x.Id);
        var total = await query.CountAsync(cancellationToken);
        var items = (await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken)).Cast<IEquipment>().ToList();
        return new PagedResult<IEquipment>(items, total, request.Page, request.PageSize);
    }
}

public sealed class FindEquipmentManufacturerQueryHandler(Context context) : IRequestHandler<FindEquipmentManufacturerQuery, string?>
{
    public async Task<string?> Handle(FindEquipmentManufacturerQuery request, CancellationToken cancellationToken)
    {
        return request.Category switch
        {
            EntityType.Adc => await Find(context.Set<Adc>(), request.Name, cancellationToken),
            EntityType.Player => await Find(context.Set<Player>(), request.Name, cancellationToken),
            EntityType.Amplifier => await Find(context.Set<Amplifier>(), request.Name, cancellationToken),
            EntityType.Cartridge => await Find(context.Set<Cartridge>(), request.Name, cancellationToken),
            EntityType.Wire => await Find(context.Set<Wire>(), request.Name, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(request.Category))
        };
    }

    private static async Task<string?> Find<T>(DbSet<T> set, string name, CancellationToken cancellationToken)
        where T : class, IEquipment => (await set.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Name == name, cancellationToken))?.Manufacturer?.Name;
}

public sealed class CreateEquipmentFormQueryHandler : IRequestHandler<CreateEquipmentFormQuery, EquipmentViewModel>
{
    public Task<EquipmentViewModel> Handle(CreateEquipmentFormQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(new EquipmentViewModel { Action = ActionType.Create, EquipmentType = EntityType.Adc });
}

public sealed class EquipmentPageQueryHandler : IRequestHandler<EquipmentPageQuery, Unit>
{
    public Task<Unit> Handle(EquipmentPageQuery request, CancellationToken cancellationToken) => Task.FromResult(Unit.Value);
}

public sealed class GetEquipmentQueryHandler(Context context, IImageService imageService) : IRequestHandler<GetEquipmentQuery, EquipmentViewModel?>
{
    public async Task<EquipmentViewModel?> Handle(GetEquipmentQuery request, CancellationToken cancellationToken)
    {
        IEquipment? equipment;
        switch (request.Category)
        {
            case EntityType.Adc:
                equipment = await context.Set<Adc>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                break;
            case EntityType.Player:
                equipment = await context.Set<Player>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                break;
            case EntityType.Amplifier:
                equipment = await context.Set<Amplifier>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                break;
            case EntityType.Cartridge:
                equipment = await context.Set<Cartridge>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                break;
            case EntityType.Wire:
                equipment = await context.Set<Wire>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(request.Category));
        }
        if (equipment is null) return null;

        var vm = new EquipmentViewModel
        {
            Id = equipment.Id,
            ModelName = equipment.Name,
            Description = equipment.Description,
            EquipmentType = request.Category,
            EquipmentCover = await imageService.GetUrlAsync(request.Id, request.Category),
            Manufacturer = equipment.Manufacturer?.Name
        };
        if (string.Equals(request.Tab, "albums", StringComparison.OrdinalIgnoreCase))
        {
            vm.ActiveTab = "albums";
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize <= 0 ? 18 : Math.Min(request.PageSize, 100);
            var releases = context.Releases.Where(r => r.EquipmentInfoId != null).AsNoTracking();
            releases = request.Category switch
            {
                EntityType.Player => releases.Where(r => r.EquipmentInfo!.PlayerId == request.Id),
                EntityType.Cartridge => releases.Where(r => r.EquipmentInfo!.CartridgeId == request.Id),
                EntityType.Amplifier => releases.Where(r => r.EquipmentInfo!.AmplifierId == request.Id),
                EntityType.Adc => releases.Where(r => r.EquipmentInfo!.AdcId == request.Id),
                EntityType.Wire => releases.Where(r => r.EquipmentInfo!.WireId == request.Id),
                _ => releases.Where(_ => false)
            };
            var albumQuery = context.Albums.Where(a => releases.Select(r => r.AlbumId).Distinct().Contains(a.Id))
                .Include(a => a.Artist).AsNoTracking().OrderBy(a => a.Artist!.Name).ThenBy(a => a.Title);
            var albums = new PagedResult<Album>(
                await albumQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken),
                await albumQuery.CountAsync(cancellationToken), page, pageSize);
            vm.ReleasedAlbumsPage = MapAlbums(request.Category, request.Id, albums);
        }
        return vm;
    }

    internal static EquipmentReleasedAlbumsPageViewModel MapAlbums(EntityType category, int equipmentId, PagedResult<Album> result) => new()
    {
        CurrentPage = result.CurrentPage,
        PageCount = result.TotalPages,
        PageSize = result.PageSize,
        HasResults = result.Items.Count > 0,
        CategorySegment = category.ToString().ToLowerInvariant(),
        EquipmentId = equipmentId,
        Albums = [.. result.Items.Select(a => new EquipmentAlbumRowViewModel
        {
            Id = a.Id,
            Title = a.Title,
            ArtistName = a.Artist?.Name ?? string.Empty,
            DetailUrl = $"/album/{a.Id}"
        })]
    };
}

public sealed class GetEquipmentAlbumsQueryHandler(Context context) : IRequestHandler<GetEquipmentAlbumsQuery, EquipmentReleasedAlbumsPageViewModel?>
{
    public async Task<EquipmentReleasedAlbumsPageViewModel?> Handle(GetEquipmentAlbumsQuery request, CancellationToken cancellationToken)
    {
        var exists = request.Category switch
        {
            EntityType.Adc => await context.Adces.AnyAsync(x => x.Id == request.Id, cancellationToken),
            EntityType.Player => await context.Players.AnyAsync(x => x.Id == request.Id, cancellationToken),
            EntityType.Amplifier => await context.Amplifiers.AnyAsync(x => x.Id == request.Id, cancellationToken),
            EntityType.Cartridge => await context.Cartridges.AnyAsync(x => x.Id == request.Id, cancellationToken),
            EntityType.Wire => await context.Wires.AnyAsync(x => x.Id == request.Id, cancellationToken),
            _ => false
        };
        if (!exists) return null;
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 18 : Math.Min(request.PageSize, 100);
        var releases = context.Releases.Where(r => r.EquipmentInfoId != null).AsNoTracking();
        releases = request.Category switch
        {
            EntityType.Player => releases.Where(r => r.EquipmentInfo!.PlayerId == request.Id),
            EntityType.Cartridge => releases.Where(r => r.EquipmentInfo!.CartridgeId == request.Id),
            EntityType.Amplifier => releases.Where(r => r.EquipmentInfo!.AmplifierId == request.Id),
            EntityType.Adc => releases.Where(r => r.EquipmentInfo!.AdcId == request.Id),
            EntityType.Wire => releases.Where(r => r.EquipmentInfo!.WireId == request.Id),
            _ => releases.Where(_ => false)
        };
        var albumQuery = context.Albums.Where(a => releases.Select(r => r.AlbumId).Distinct().Contains(a.Id))
            .Include(a => a.Artist).AsNoTracking().OrderBy(a => a.Artist!.Name).ThenBy(a => a.Title);
        var albums = new PagedResult<Album>(
            await albumQuery.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken),
            await albumQuery.CountAsync(cancellationToken), page, pageSize);
        return GetEquipmentQueryHandler.MapAlbums(request.Category, request.Id, albums);
    }
}

public sealed class CreateEquipmentCommandHandler(Context context, IImageService imageService) : IRequestHandler<CreateEquipmentCommand, int>
{
    public async Task<int> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var manufacturer = string.IsNullOrWhiteSpace(request.Request.Manufacturer) ? null : await context.Manufacturer.FirstOrDefaultAsync(m => m.Name == request.Request.Manufacturer.Trim(), cancellationToken);
        if (manufacturer is null && !string.IsNullOrWhiteSpace(request.Request.Manufacturer))
        {
            manufacturer = new Manufacturer { Name = request.Request.Manufacturer.Trim() };
            context.Manufacturer.Add(manufacturer);
            await context.SaveChangesAsync(cancellationToken);
        }
        IEquipment equipment;
        switch (request.Request.EquipmentType)
        {
            case EntityType.Adc: equipment = new Adc { Id = request.Request.Id, Name = request.Request.ModelName, Description = request.Request.Description, Manufacturer = manufacturer }; break;
            case EntityType.Amplifier: equipment = new Amplifier { Id = request.Request.Id, Name = request.Request.ModelName, Description = request.Request.Description, Manufacturer = manufacturer }; break;
            case EntityType.Cartridge: equipment = new Cartridge { Id = request.Request.Id, Name = request.Request.ModelName, Description = request.Request.Description, Manufacturer = manufacturer }; break;
            case EntityType.Player: equipment = new Player { Id = request.Request.Id, Name = request.Request.ModelName, Description = request.Request.Description, Manufacturer = manufacturer }; break;
            case EntityType.Wire: equipment = new Wire { Id = request.Request.Id, Name = request.Request.ModelName, Description = request.Request.Description, Manufacturer = manufacturer }; break;
            default: throw new ArgumentOutOfRangeException(nameof(request.Request.EquipmentType));
        }
        context.Add(equipment);
        await context.SaveChangesAsync(cancellationToken);
        if (request.Request.EquipmentCover is not null)
            await imageService.SaveAsync(equipment.Id, request.Request.EquipmentCover, request.Request.EquipmentType);
        return equipment.Id;
    }
}

public sealed class UpdateEquipmentCommandHandler(Context context, IImageService imageService) : IRequestHandler<UpdateEquipmentCommand, int>
{
    public async Task<int> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var model = request.Request;
        var manufacturer = string.IsNullOrWhiteSpace(model.Manufacturer) ? null : await context.Manufacturer.FirstOrDefaultAsync(m => m.Name == model.Manufacturer.Trim(), cancellationToken);
        if (manufacturer is null && !string.IsNullOrWhiteSpace(model.Manufacturer))
        {
            manufacturer = new Manufacturer { Name = model.Manufacturer.Trim() };
            context.Manufacturer.Add(manufacturer);
            await context.SaveChangesAsync(cancellationToken);
        }
        IEquipment equipment;
        switch (model.EquipmentType)
        {
            case EntityType.Adc: equipment = new Adc { Id = model.Id, Name = model.ModelName, Description = model.Description, Manufacturer = manufacturer }; break;
            case EntityType.Amplifier: equipment = new Amplifier { Id = model.Id, Name = model.ModelName, Description = model.Description, Manufacturer = manufacturer }; break;
            case EntityType.Cartridge: equipment = new Cartridge { Id = model.Id, Name = model.ModelName, Description = model.Description, Manufacturer = manufacturer }; break;
            case EntityType.Player: equipment = new Player { Id = model.Id, Name = model.ModelName, Description = model.Description, Manufacturer = manufacturer }; break;
            case EntityType.Wire: equipment = new Wire { Id = model.Id, Name = model.ModelName, Description = model.Description, Manufacturer = manufacturer }; break;
            default: throw new ArgumentOutOfRangeException(nameof(model.EquipmentType));
        }
        context.Update(equipment);
        await context.SaveChangesAsync(cancellationToken);
        if (model.EquipmentCover is null)
            await imageService.RemoveAsync(equipment.Id, model.EquipmentType);
        else
            await imageService.SaveAsync(equipment.Id, model.EquipmentCover, model.EquipmentType);
        return equipment.Id;
    }
}

public sealed class DeleteEquipmentCommandHandler(Context context, IImageService imageService) : IRequestHandler<DeleteEquipmentCommand, bool>
{
    public async Task<bool> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
    {
        object? equipment;
        switch (request.Category)
        {
            case EntityType.Adc: equipment = await context.Adces.FindAsync([request.Id], cancellationToken); break;
            case EntityType.Player: equipment = await context.Players.FindAsync([request.Id], cancellationToken); break;
            case EntityType.Amplifier: equipment = await context.Amplifiers.FindAsync([request.Id], cancellationToken); break;
            case EntityType.Cartridge: equipment = await context.Cartridges.FindAsync([request.Id], cancellationToken); break;
            case EntityType.Wire: equipment = await context.Wires.FindAsync([request.Id], cancellationToken); break;
            default: equipment = null; break;
        }
        if (equipment is null) return false;
        context.Remove(equipment);
        await context.SaveChangesAsync(cancellationToken);
        await imageService.RemoveAsync(request.Id, request.Category);
        return true;
    }

}
