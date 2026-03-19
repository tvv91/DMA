using Microsoft.AspNetCore.SignalR;
using Web.Enums;
using Web.Interfaces;
using Web.Services;
using Web.ViewModels;

namespace Web.SignalRHubs
{
    public class EquipmentHub(
        ICoverService imageService,
        IEquipmentService equipmentService) : Hub
    {
        private readonly ICoverService _imgService = imageService;
        private readonly IEquipmentService _equipmentService = equipmentService;
        private const int ITEMS_PER_PAGE = 18;

        private readonly Dictionary<string, EntityType> _categoryEntityMap = new()
        {
            { "adc", EntityType.Adc },
            { "amplifier", EntityType.Amplifier },
            { "cartridge", EntityType.Cartridge },
            { "player", EntityType.Player },
            { "wire", EntityType.Wire },
        };

        public async Task GetHardwareByCategory(string connectionId, string category, int page)
        {
            if (!_categoryEntityMap.TryGetValue(category, out var entityType))
                return;

            var pagedResult = await _equipmentService.GetListAsync(page, ITEMS_PER_PAGE, entityType);

            var result = pagedResult.Items
                .Select(x => new EquipmentViewModel
                {
                    Id = x.Id,
                    ModelName = x.Name,
                    Manufacturer = x.Manufacturer?.Name ?? "—",
                    EquipmentType = entityType
                })
                .ToList();

            int pageCount = pagedResult.TotalPages;

            await Clients.Client(connectionId).SendAsync("ReceivedItems", result);
            await Clients.Client(connectionId).SendAsync("ReceivedItemsCount", pageCount);

            foreach (var item in result)
            {
                var imageUrl = await _imgService.GetCoverUrlAsync(item.Id, entityType);
                await Clients.Client(connectionId).SendAsync(
                    "ReceivedItemImage",
                    item.Id,
                    imageUrl
                );
            }
        }

        public async Task GetManufacturer(string connectionId, string category, string value)
        {
            if (!_categoryEntityMap.TryGetValue(category, out var type))
            {
                await Clients.Client(connectionId).SendAsync("ReceivedManufacturer", category, string.Empty);
                return;
            }

            var item = await _equipmentService.GetManufacturerByNameAsync(value, type);
            var result = item?.Manufacturer?.Name ?? string.Empty;

            await Clients.Client(connectionId).SendAsync("ReceivedManufacturer", category, result);
        }

        public async Task GetEquipmentImage(string connectionId, int equipmentId, string type)
        {
            var imageUrl = await _imgService.GetCoverUrlAsync(equipmentId, Enum.Parse<EntityType>(type));
            await Clients.Client(connectionId).SendAsync("ReceivedEquipmentImageDetailed", imageUrl);
        }
    }
}

