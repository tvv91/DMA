using DMA.Application.Equipment;
using DMA.Application.ReferenceData;
using MediatR;
using Web.Interfaces;
using Web.ViewModels;

namespace Web.Services;

public class EquipmentService(IMediator mediator) : IEquipmentService
{
    public Task<IManufacturer?> GetByIdAsync(int id, EntityType type) =>
        mediator.Send(new GetEquipmentByIdQuery(id, type));

    public async Task<PagedResult<IManufacturer>> GetListAsync(int page, int pageSize, EntityType type)
    {
        var result = await mediator.Send(new GetEquipmentListQuery(page, pageSize, type));
        return new PagedResult<IManufacturer>(result.Items.Cast<IManufacturer>().ToList(), result.TotalItems, result.CurrentPage, result.PageSize);
    }

    public Task<IManufacturer?> GetManufacturerByNameAsync(string name, EntityType type) =>
        mediator.Send(new GetEquipmentByNameQuery(name, type));

    public Task<IManufacturer> CreateEquipmentAsync(EquipmentViewModel request) =>
        mediator.Send(new UpsertEquipmentCommand(
            request.EquipmentType,
            request.Id,
            request.ModelName,
            request.Description,
            request.Manufacturer));

    public Task<IManufacturer> UpdateEquipmentAsync(EquipmentViewModel request) =>
        mediator.Send(new UpsertEquipmentCommand(
            request.EquipmentType,
            request.Id,
            request.ModelName,
            request.Description,
            request.Manufacturer));

    public Task<bool> DeleteEquipmentAsync(int id, EntityType type) =>
        mediator.Send(new DeleteEquipmentCommand(id, type));

    public EquipmentViewModel MapEquipmentToViewModel(IManufacturer equipment, EntityType type, string? imageUrl = null) => new()
    {
        Id = equipment.Id,
        ModelName = equipment.Name,
        Description = equipment.Description,
        EquipmentType = type,
        EquipmentCover = imageUrl,
        Manufacturer = equipment.Manufacturer?.Name
    };

    public async Task<IManufacturer> MapViewModelToEquipmentAsync(EquipmentViewModel request)
    {
        Manufacturer? manufacturer = null;
        if (!string.IsNullOrWhiteSpace(request.Manufacturer))
            manufacturer = await mediator.Send(new FindOrCreateManufacturerCommand(request.Manufacturer));

        return request.EquipmentType switch
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
    }
}
