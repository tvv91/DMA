using Microsoft.EntityFrameworkCore;
using Web.Common;
using Web.Db;
using Web.Enums;
using Web.Extentions;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;

namespace Web.Services
{
    public class EquipmentService(Context context) : IEquipmentService
    {
        private readonly Context _context = context;

        public async Task<IManufacturer?> GetByIdAsync(int id, EntityType type)
        {
            return type switch
            {
                EntityType.Adc => await _context.Set<Adc>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id),
                EntityType.Player => await _context.Set<Player>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id),
                EntityType.Amplifier => await _context.Set<Amplifier>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id),
                EntityType.Cartridge => await _context.Set<Cartridge>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id),
                EntityType.Wire => await _context.Set<Wire>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Id == id),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        public async Task<PagedResult<IManufacturer>> GetListAsync(int page, int pageSize, EntityType type)
        {
            IQueryable<IManufacturer> query = type switch
            {
                EntityType.Adc => _context.Set<Adc>().Include(x => x.Manufacturer),
                EntityType.Player => _context.Set<Player>().Include(x => x.Manufacturer),
                EntityType.Amplifier => _context.Set<Amplifier>().Include(x => x.Manufacturer),
                EntityType.Cartridge => _context.Set<Cartridge>().Include(x => x.Manufacturer),
                EntityType.Wire => _context.Set<Wire>().Include(x => x.Manufacturer),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };

            return await query.ToPagedResultAsync(page, pageSize, x => x.Id);
        }

        public async Task<IManufacturer?> GetManufacturerByNameAsync(string name, EntityType type)
        {
            return type switch
            {
                EntityType.Adc => await _context.Set<Adc>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Name == name),
                EntityType.Player => await _context.Set<Player>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Name == name),
                EntityType.Amplifier => await _context.Set<Amplifier>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Name == name),
                EntityType.Cartridge => await _context.Set<Cartridge>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Name == name),
                EntityType.Wire => await _context.Set<Wire>().Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Name == name),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        public async Task<IManufacturer> CreateEquipmentAsync(EquipmentViewModel request)
        {
            var equipment = await MapViewModelToEquipmentAsync(request);
            _context.Add(equipment);
            await _context.SaveChangesAsync();
            return equipment;
        }

        public async Task<IManufacturer> UpdateEquipmentAsync(EquipmentViewModel request)
        {
            var equipment = await MapViewModelToEquipmentAsync(request);
            _context.Update(equipment);
            await _context.SaveChangesAsync();
            return equipment;
        }

        public async Task<bool> DeleteEquipmentAsync(int id, EntityType type)
        {
            var item = await GetByIdAsync(id, type);
            if (item is null) return false;

            _context.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public EquipmentViewModel MapEquipmentToViewModel(IManufacturer equipment, EntityType type, string? imageUrl = null)
        {
            return new EquipmentViewModel
            {
                Id = equipment.Id,
                ModelName = equipment.Name,
                Description = equipment.Description,
                EquipmentType = type,
                EquipmentCover = imageUrl,
                Manufacturer = equipment.Manufacturer?.Name
            };
        }

        public async Task<IManufacturer> MapViewModelToEquipmentAsync(EquipmentViewModel request)
        {
            var manufacturer = await FindOrCreateManufacturerAsync(request.Manufacturer);

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

        private async Task<Manufacturer?> FindOrCreateManufacturerAsync(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var normalizedName = name.Trim();

            var existing = await _context.Manufacturer
                .FirstOrDefaultAsync(m => m.Name == normalizedName);

            if (existing is not null)
                return existing;

            var newManufacturer = new Manufacturer { Name = normalizedName };
            _context.Manufacturer.Add(newManufacturer);
            await _context.SaveChangesAsync();

            return newManufacturer;
        }
    }
}


