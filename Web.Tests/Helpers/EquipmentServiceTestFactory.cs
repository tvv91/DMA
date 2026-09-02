using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Interfaces;
using Web.Services;
using Web.ViewModels;

namespace Web.Tests.Helpers;

internal sealed class EquipmentServiceTestFactory : IDisposable
{
    public static readonly EntityType[] SupportedEquipmentTypes =
    [
        EntityType.Player,
        EntityType.Adc,
        EntityType.Amplifier,
        EntityType.Cartridge,
        EntityType.Wire,
    ];

    private readonly TestMediatorContext _mediatorContext;

    public Context Context { get; }
    public EquipmentService Service { get; }

    public EquipmentServiceTestFactory()
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        Context = new Context(options);
        Context.Database.EnsureCreated();
        _mediatorContext = MediatorTestHelper.Create(Context);
        Service = new EquipmentService(_mediatorContext);
    }

    public async Task<IManufacturer> SeedEquipmentAsync(
        EntityType type,
        string modelName = "Test Model",
        string? manufacturerName = "Test Manufacturer",
        string? description = "Test description")
    {
        Manufacturer? manufacturer = null;
        if (manufacturerName is not null)
        {
            manufacturer = new Manufacturer { Name = manufacturerName };
            Context.Manufacturer.Add(manufacturer);
            await Context.SaveChangesAsync();
        }

        IManufacturer equipment = type switch
        {
            EntityType.Player => new Player { Name = modelName, Description = description, Manufacturer = manufacturer, ManufacturerId = manufacturer?.Id },
            EntityType.Adc => new Adc { Name = modelName, Description = description, Manufacturer = manufacturer, ManufacturerId = manufacturer?.Id },
            EntityType.Amplifier => new Amplifier { Name = modelName, Description = description, Manufacturer = manufacturer, ManufacturerId = manufacturer?.Id },
            EntityType.Cartridge => new Cartridge { Name = modelName, Description = description, Manufacturer = manufacturer, ManufacturerId = manufacturer?.Id },
            EntityType.Wire => new Wire { Name = modelName, Description = description, Manufacturer = manufacturer, ManufacturerId = manufacturer?.Id },
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

        Context.Add(equipment);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
        return equipment;
    }

    public static EquipmentViewModel CreateViewModel(
        EntityType type,
        int id = 0,
        string modelName = "New Model",
        string? manufacturer = "New Manufacturer",
        string? description = "New description") =>
        new()
        {
            Id = id,
            ModelName = modelName,
            Manufacturer = manufacturer,
            Description = description,
            EquipmentType = type,
        };

    public int CountEquipment(EntityType type) => type switch
    {
        EntityType.Player => Context.Players.Count(),
        EntityType.Adc => Context.Adces.Count(),
        EntityType.Amplifier => Context.Amplifiers.Count(),
        EntityType.Cartridge => Context.Cartridges.Count(),
        EntityType.Wire => Context.Wires.Count(),
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    public void Dispose()
    {
        _mediatorContext.Dispose();
        Context.Dispose();
    }
}
