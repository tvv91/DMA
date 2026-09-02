using DMA.Domain.Common;
using DMA.Domain.Equipment;
using Microsoft.EntityFrameworkCore;

namespace DMA.Infrastructure.Persistence.Repositories;

public class EquipmentRepository(DmaDbContext context) : IEquipmentRepository
{
    private readonly DmaDbContext _context = context;

    public Task<IManufacturerEquipment?> GetByIdAsync(int id, EntityType type, CancellationToken cancellationToken = default) =>
        type switch
        {
            EntityType.Adc => GetByIdInternalAsync<Adc>(id, cancellationToken),
            EntityType.Player => GetByIdInternalAsync<Player>(id, cancellationToken),
            EntityType.Amplifier => GetByIdInternalAsync<Amplifier>(id, cancellationToken),
            EntityType.Cartridge => GetByIdInternalAsync<Cartridge>(id, cancellationToken),
            EntityType.Wire => GetByIdInternalAsync<Wire>(id, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

    private async Task<IManufacturerEquipment?> GetByIdInternalAsync<T>(int id, CancellationToken cancellationToken)
        where T : class, IManufacturerEquipment =>
        await _context.Set<T>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedResult<IManufacturerEquipment>> GetListPagedAsync(int page, int pageSize, EntityType type, CancellationToken cancellationToken = default)
    {
        if (page < 1)
            page = 1;

        var query = GetQueryable(type);
        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<IManufacturerEquipment>(items, totalItems, page, pageSize);
    }

    public IQueryable<IManufacturerEquipment> GetQueryable(EntityType type) =>
        type switch
        {
            EntityType.Adc => _context.Set<Adc>().Include(x => x.Manufacturer).Cast<IManufacturerEquipment>(),
            EntityType.Player => _context.Set<Player>().Include(x => x.Manufacturer).Cast<IManufacturerEquipment>(),
            EntityType.Amplifier => _context.Set<Amplifier>().Include(x => x.Manufacturer).Cast<IManufacturerEquipment>(),
            EntityType.Cartridge => _context.Set<Cartridge>().Include(x => x.Manufacturer).Cast<IManufacturerEquipment>(),
            EntityType.Wire => _context.Set<Wire>().Include(x => x.Manufacturer).Cast<IManufacturerEquipment>(),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

    public Task<IManufacturerEquipment?> GetByNameAsync(string name, EntityType type, CancellationToken cancellationToken = default) =>
        type switch
        {
            EntityType.Adc => GetByNameInternalAsync<Adc>(name, cancellationToken),
            EntityType.Player => GetByNameInternalAsync<Player>(name, cancellationToken),
            EntityType.Amplifier => GetByNameInternalAsync<Amplifier>(name, cancellationToken),
            EntityType.Cartridge => GetByNameInternalAsync<Cartridge>(name, cancellationToken),
            EntityType.Wire => GetByNameInternalAsync<Wire>(name, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

    private async Task<IManufacturerEquipment?> GetByNameInternalAsync<T>(string name, CancellationToken cancellationToken)
        where T : class, IManufacturerEquipment =>
        await _context.Set<T>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

    public void Add(IManufacturerEquipment equipment) => _context.Add(equipment);

    public void Update(IManufacturerEquipment equipment) => _context.Update(equipment);

    public void Remove(IManufacturerEquipment equipment) => _context.Remove(equipment);

    public Task<Manufacturer?> FindManufacturerByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Task.FromResult<Manufacturer?>(null);

        var normalizedName = name.Trim();
        return _context.Manufacturer.FirstOrDefaultAsync(m => m.Name == normalizedName, cancellationToken);
    }

    public void AddManufacturer(Manufacturer manufacturer) => _context.Manufacturer.Add(manufacturer);
}
