using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.ViewModels;

namespace Web.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;

        public EquipmentService(IEquipmentRepository equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
        }

        public async Task<IManufacturer?> GetByIdAsync(int id, EntityType type)
        {
            return await _equipmentRepository.GetByIdAsync(id, type);
        }

        public async Task<IManufacturer> CreateEquipmentAsync(EquipmentViewModel request)
        {
            var equipment = MapViewModelToEquipment(request);
            return await _equipmentRepository.AddAsync(equipment, request.EquipmentType);
        }

        public async Task<IManufacturer> UpdateEquipmentAsync(EquipmentViewModel request)
        {
            var equipment = MapViewModelToEquipment(request);
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

        public IManufacturer MapViewModelToEquipment(EquipmentViewModel request)
        {
            Manufacturer? CreateManufacturer(string? name, EntityType type) => 
                string.IsNullOrWhiteSpace(name) ? null : new Manufacturer { Name = name, Type = type };

            return request.EquipmentType switch
            {
                EntityType.Adc => new Adc
                {
                    Id = request.Id,
                    Name = request.ModelName,
                    Description = request.Description,
                    Manufacturer = CreateManufacturer(request.Manufacturer, EntityType.Adc)
                },
                EntityType.Amplifier => new Amplifier
                {
                    Id = request.Id,
                    Name = request.ModelName,
                    Description = request.Description,
                    Manufacturer = CreateManufacturer(request.Manufacturer, EntityType.Amplifier)
                },
                EntityType.Cartridge => new Cartridge
                {
                    Id = request.Id,
                    Name = request.ModelName,
                    Description = request.Description,
                    Manufacturer = CreateManufacturer(request.Manufacturer, EntityType.Cartridge)
                },
                EntityType.Player => new Player
                {
                    Id = request.Id,
                    Name = request.ModelName,
                    Description = request.Description,
                    Manufacturer = CreateManufacturer(request.Manufacturer, EntityType.Player)
                },
                EntityType.Wire => new Wire
                {
                    Id = request.Id,
                    Name = request.ModelName,
                    Description = request.Description,
                    Manufacturer = CreateManufacturer(request.Manufacturer, EntityType.Wire)
                },
                _ => throw new ArgumentOutOfRangeException(nameof(request.EquipmentType), request.EquipmentType, "Unknown equipment type")
            };
        }
    }
}

