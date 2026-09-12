using MediatR;
using Web.Common;
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

public sealed class CreateEquipmentFormQueryHandler : IRequestHandler<CreateEquipmentFormQuery, EquipmentViewModel>
{
    public Task<EquipmentViewModel> Handle(CreateEquipmentFormQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(new EquipmentViewModel { Action = ActionType.Create, EquipmentType = EntityType.Adc });
}

public sealed class EquipmentPageQueryHandler : IRequestHandler<EquipmentPageQuery, Unit>
{
    public Task<Unit> Handle(EquipmentPageQuery request, CancellationToken cancellationToken) => Task.FromResult(Unit.Value);
}

public sealed class GetEquipmentQueryHandler(IEquipmentService equipmentService, IImageService imageService, IReleaseService releaseService) : IRequestHandler<GetEquipmentQuery, EquipmentViewModel?>
{
    public async Task<EquipmentViewModel?> Handle(GetEquipmentQuery request, CancellationToken cancellationToken)
    {
        var equipment = await equipmentService.GetByIdAsync(request.Id, request.Category);
        if (equipment is null) return null;

        var vm = equipmentService.MapEquipmentToViewModel(equipment, request.Category, await imageService.GetUrlAsync(request.Id, request.Category));
        if (string.Equals(request.Tab, "albums", StringComparison.OrdinalIgnoreCase))
        {
            vm.ActiveTab = "albums";
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize <= 0 ? 18 : Math.Min(request.PageSize, 100);
            var albums = await releaseService.GetAlbumsReleasedByEquipmentPagedAsync(request.Category, request.Id, page, pageSize);
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

public sealed class GetEquipmentAlbumsQueryHandler(IEquipmentService equipmentService, IReleaseService releaseService) : IRequestHandler<GetEquipmentAlbumsQuery, EquipmentReleasedAlbumsPageViewModel?>
{
    public async Task<EquipmentReleasedAlbumsPageViewModel?> Handle(GetEquipmentAlbumsQuery request, CancellationToken cancellationToken)
    {
        if (await equipmentService.GetByIdAsync(request.Id, request.Category) is null) return null;
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 18 : Math.Min(request.PageSize, 100);
        var albums = await releaseService.GetAlbumsReleasedByEquipmentPagedAsync(request.Category, request.Id, page, pageSize);
        return GetEquipmentQueryHandler.MapAlbums(request.Category, request.Id, albums);
    }
}

public sealed class CreateEquipmentCommandHandler(IEquipmentService equipmentService, IImageService imageService) : IRequestHandler<CreateEquipmentCommand, int>
{
    public async Task<int> Handle(CreateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var equipment = await equipmentService.CreateEquipmentAsync(request.Request);
        if (request.Request.EquipmentCover is not null)
            await imageService.SaveAsync(equipment.Id, request.Request.EquipmentCover, request.Request.EquipmentType);
        return equipment.Id;
    }
}

public sealed class UpdateEquipmentCommandHandler(IEquipmentService equipmentService, IImageService imageService) : IRequestHandler<UpdateEquipmentCommand, int>
{
    public async Task<int> Handle(UpdateEquipmentCommand request, CancellationToken cancellationToken)
    {
        var model = request.Request;
        var equipment = await equipmentService.UpdateEquipmentAsync(model);
        if (model.EquipmentCover is null)
            await imageService.RemoveAsync(equipment.Id, model.EquipmentType);
        else
            await imageService.SaveAsync(equipment.Id, model.EquipmentCover, model.EquipmentType);
        return equipment.Id;
    }
}

public sealed class DeleteEquipmentCommandHandler(IEquipmentService equipmentService, IImageService imageService) : IRequestHandler<DeleteEquipmentCommand, bool>
{
    public async Task<bool> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
    {
        var deleted = await equipmentService.DeleteEquipmentAsync(request.Id, request.Category);
        if (deleted) await imageService.RemoveAsync(request.Id, request.Category);
        return deleted;
    }
}
