using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;

namespace Web.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly DMADbContext _context;

        public EquipmentService(IEquipmentRepository equipmentRepository, DMADbContext context)
        {
            _equipmentRepository = equipmentRepository;
            _context = context;
        }

        public async Task<IManufacturer?> GetByIdAsync(int id, EntityType type)
        {
            return await _equipmentRepository.GetByIdAsync(id, type);
        }

        public async Task<IManufacturer> CreateEquipmentAsync(EquipmentViewModel request)
        {
            var equipment = await MapViewModelToEquipmentAsync(request);
            return await _equipmentRepository.AddAsync(equipment, request.EquipmentType);
        }

        public async Task<IManufacturer> UpdateEquipmentAsync(EquipmentViewModel request)
        {
            var equipment = await MapViewModelToEquipmentAsync(request);
            return await _equipmentRepository.UpdateAsync(equipment, request.EquipmentType);
        }

        public async Task<bool> DeleteEquipmentAsync(int id, EntityType type)
        {
            return await _equipmentRepository.DeleteAsync(id, type);
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
            var manufacturer = await FindOrCreateManufacturerAsync(request.Manufacturer, GetManufacturerType(request.EquipmentType));

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

        private static EntityType GetManufacturerType(EntityType equipmentType)
        {
            return equipmentType switch
            {
                EntityType.Adc => EntityType.AdcManufacturer,
                EntityType.Amplifier => EntityType.AmplifierManufacturer,
                EntityType.Cartridge => EntityType.CartridgeManufacturer,
                EntityType.Player => EntityType.PlayerManufacturer,
                EntityType.Wire => EntityType.WireManufacturer,
                _ => throw new ArgumentOutOfRangeException(nameof(equipmentType), equipmentType, "Unknown equipment type")
            };
        }

        private async Task<Manufacturer?> FindOrCreateManufacturerAsync(string? name, EntityType manufacturerType)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            // First, try to find existing manufacturer by name and type (case-insensitive)
            var existingManufacturer = await _context.Manufacturer
                .FirstOrDefaultAsync(m => m.Name.ToLower() == name.ToLower() && m.Type == manufacturerType);

            if (existingManufacturer != null)
                return existingManufacturer;

            // If not found by name and type, try to find by name only (to reuse existing manufacturers)
            var existingByName = await _context.Manufacturer
                .FirstOrDefaultAsync(m => m.Name.ToLower() == name.ToLower());

            if (existingByName != null)
            {
                // Update the type if it's different (to gradually fix data inconsistencies)
                if (existingByName.Type != manufacturerType)
                {
                    existingByName.Type = manufacturerType;
                    await _context.SaveChangesAsync();
                }
                return existingByName;
            }

            // If not found at all, create a new one
            var newManufacturer = new Manufacturer
            {
                Name = name.Trim(),
                Type = manufacturerType
            };

            _context.Manufacturer.Add(newManufacturer);
            await _context.SaveChangesAsync();

            return newManufacturer;
        }
    }
}


