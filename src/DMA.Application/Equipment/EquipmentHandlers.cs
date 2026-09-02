using DMA.Domain.Common;
using DMA.Domain.Equipment;
using MediatR;

namespace DMA.Application.Equipment;

public sealed record GetEquipmentByIdQuery(int Id, EntityType Type) : IRequest<IManufacturerEquipment?>;

public sealed class GetEquipmentByIdQueryHandler(IEquipmentRepository equipmentRepository)
    : IRequestHandler<GetEquipmentByIdQuery, IManufacturerEquipment?>
{
    public Task<IManufacturerEquipment?> Handle(GetEquipmentByIdQuery request, CancellationToken cancellationToken) =>
        equipmentRepository.GetByIdAsync(request.Id, request.Type, cancellationToken);
}

public sealed record GetEquipmentListQuery(int Page, int PageSize, EntityType Type) : IRequest<PagedResult<IManufacturerEquipment>>;

public sealed class GetEquipmentListQueryHandler(IEquipmentRepository equipmentRepository)
    : IRequestHandler<GetEquipmentListQuery, PagedResult<IManufacturerEquipment>>
{
    public Task<PagedResult<IManufacturerEquipment>> Handle(GetEquipmentListQuery request, CancellationToken cancellationToken) =>
        equipmentRepository.GetListPagedAsync(request.Page, request.PageSize, request.Type, cancellationToken);
}

public sealed record GetEquipmentByNameQuery(string Name, EntityType Type) : IRequest<IManufacturerEquipment?>;

public sealed class GetEquipmentByNameQueryHandler(IEquipmentRepository equipmentRepository)
    : IRequestHandler<GetEquipmentByNameQuery, IManufacturerEquipment?>
{
    public Task<IManufacturerEquipment?> Handle(GetEquipmentByNameQuery request, CancellationToken cancellationToken) =>
        equipmentRepository.GetByNameAsync(request.Name, request.Type, cancellationToken);
}

public sealed record UpsertEquipmentCommand(
    EntityType EquipmentType,
    int Id,
    string ModelName,
    string? Description,
    string? Manufacturer) : IRequest<IManufacturerEquipment>;

public sealed class UpsertEquipmentCommandHandler(
    IEquipmentRepository equipmentRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpsertEquipmentCommand, IManufacturerEquipment>
{
    public async Task<IManufacturerEquipment> Handle(UpsertEquipmentCommand request, CancellationToken cancellationToken)
    {
        Manufacturer? manufacturer = null;
        if (!string.IsNullOrWhiteSpace(request.Manufacturer))
        {
            var normalizedName = request.Manufacturer.Trim();
            manufacturer = await equipmentRepository.FindManufacturerByNameAsync(normalizedName, cancellationToken);
            if (manufacturer is null)
            {
                manufacturer = new Manufacturer { Name = normalizedName };
                equipmentRepository.AddManufacturer(manufacturer);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        IManufacturerEquipment equipment = request.EquipmentType switch
        {
            EntityType.Adc => new Adc
            {
                Id = request.Id,
                Name = request.ModelName,
                Description = request.Description,
                Manufacturer = manufacturer
            },
            EntityType.Amplifier => new Amplifier
            {
                Id = request.Id,
                Name = request.ModelName,
                Description = request.Description,
                Manufacturer = manufacturer
            },
            EntityType.Cartridge => new Cartridge
            {
                Id = request.Id,
                Name = request.ModelName,
                Description = request.Description,
                Manufacturer = manufacturer
            },
            EntityType.Player => new Player
            {
                Id = request.Id,
                Name = request.ModelName,
                Description = request.Description,
                Manufacturer = manufacturer
            },
            EntityType.Wire => new Wire
            {
                Id = request.Id,
                Name = request.ModelName,
                Description = request.Description,
                Manufacturer = manufacturer
            },
            _ => throw new ArgumentOutOfRangeException(nameof(request.EquipmentType), request.EquipmentType, "Unknown equipment type")
        };

        if (request.Id > 0)
        {
            equipmentRepository.Update(equipment);
        }
        else
        {
            equipmentRepository.Add(equipment);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return equipment;
    }
}

public sealed record DeleteEquipmentCommand(int Id, EntityType Type) : IRequest<bool>;

public sealed class DeleteEquipmentCommandHandler(IEquipmentRepository equipmentRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteEquipmentCommand, bool>
{
    public async Task<bool> Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
    {
        var item = await equipmentRepository.GetByIdAsync(request.Id, request.Type, cancellationToken);
        if (item is null)
            return false;

        equipmentRepository.Remove(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
