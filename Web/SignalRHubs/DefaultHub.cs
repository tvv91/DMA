using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Enums;
using Web.Extentsions;
using Web.Models;
using Web.Response;
using Web.Services;

namespace Web.SignalRHubs
{
    public class DefaultHub : Hub
    {
        private readonly IImageService _imgService;
        private readonly ITechInfoRepository _techInfoRepository;
        private const int ITEMS_PER_PAGE = 20;
        private readonly Dictionary<string, EntityType> _categoryEntitityMap = new Dictionary<string, EntityType>()
        {
            { "adc", EntityType.Adc },
            { "amplifier", EntityType.Amp },
            { "cartrige", EntityType.Cartridge },
            { "player", EntityType.Player },
            { "wire", EntityType.Wire },
        };

        public DefaultHub(IImageService coverImageService, ITechInfoRepository techInfoRepository)
        {
            _imgService = coverImageService;
            _techInfoRepository = techInfoRepository;
        }

        public async Task GetAlbumCovers(string connectionId, int[] albums)
        {
            // otherwise album covers will load sequentially
            Random.Shared.Shuffle(albums);
            await Task.WhenAll(albums.Select(async id => await Clients.Client(connectionId).SendAsync("ReceivedAlbumConver", id, _imgService.GetImageUrl(id, EntityType.AlbumCover))));
        }

        public async Task GetAlbumCover(string connectionId, int albumId)
        {
            await Clients.Client(connectionId).SendAsync("ReceivedAlbumConverDetailed", _imgService.GetImageUrl(albumId, EntityType.AlbumDetailCover));
        }
        
        private async Task<List<EquipmentResponse>> GetAdcItems(int page)
        {
            return await _techInfoRepository.Adcs
                .Skip((page - 1)* ITEMS_PER_PAGE)
                .Take(ITEMS_PER_PAGE)
                .Include(x => x.Manufacturer)
                .Select(x => new EquipmentResponse()
                {
                    Id = x.Id,
                    Model = x.Data,
                    Manufacturer = x.Manufacturer.Data,
                })
                .ToListAsync();
        }

        private async Task<List<EquipmentResponse>> GetAmplifierItems(int page)
        {
            return await _techInfoRepository.Amplifiers
                .Skip((page - 1) * ITEMS_PER_PAGE)
                .Take(ITEMS_PER_PAGE)
                .Include(x => x.Manufacturer)
                .Select(x => new EquipmentResponse()
                {
                    Id = x.Id,
                    Model = x.Data,
                    Manufacturer = x.Manufacturer.Data,
                }).ToListAsync();
        }

        private async Task<List<EquipmentResponse>> GetCartrigeItems(int page)
        {
            return await _techInfoRepository.Cartriges
                .Skip((page - 1) * ITEMS_PER_PAGE)
                .Take(ITEMS_PER_PAGE)
                .Include(x => x.Manufacturer)
                .Select(x => new EquipmentResponse()
                {
                    Id = x.Id,
                    Model = x.Data,
                    Manufacturer = x.Manufacturer.Data,
                }).ToListAsync();
        }

        private async Task<List<EquipmentResponse>> GetPlayerItems(int page)
        {
            return await _techInfoRepository.Players
                .Skip((page - 1) * ITEMS_PER_PAGE)
                .Take(ITEMS_PER_PAGE)
                .Include(x => x.Manufacturer)
                .Select(x => new EquipmentResponse()
                {
                    Id = x.Id,
                    Model = x.Data,
                    Manufacturer = x.Manufacturer.Data,
                }).ToListAsync();
        }

        private async Task<List<EquipmentResponse>> GetWireItems(int page)
        {
            return await _techInfoRepository.Wires
                .Skip((page - 1) * ITEMS_PER_PAGE)
                .Take(ITEMS_PER_PAGE)
                .Include(x => x.Manufacturer)
                .Select(x => new EquipmentResponse()
                {
                    Id = x.Id,
                    Model = x.Data,
                    Manufacturer = x.Manufacturer.Data,
                }).ToListAsync();
        }

        public async Task GetHardwareByCategory(string connectionId, string category, int page)
        {
            var result = new List<EquipmentResponse>();
            int itemsCount = 0;
            
            switch (category)
            {
                case "adc":
                    itemsCount = await _techInfoRepository.Adcs.CountAsync();
                    result = await GetAdcItems(page);
                    break;
                case "amplifier":
                    itemsCount = await _techInfoRepository.Amplifiers.CountAsync();
                    result = await GetAmplifierItems(page);
                    break;
                case "cartrige":
                    itemsCount = await _techInfoRepository.Cartriges.CountAsync();
                    result = await GetCartrigeItems(page);
                    break;
                case "player":
                    itemsCount = await _techInfoRepository.Players.CountAsync();
                    result = await GetPlayerItems(page);
                    break;
                case "wire":
                    itemsCount = await _techInfoRepository.Wires.CountAsync();
                    result = await GetWireItems(page);
                    break;
            }

            int pageCount = itemsCount % ITEMS_PER_PAGE == 0 ? itemsCount / ITEMS_PER_PAGE : itemsCount / ITEMS_PER_PAGE + 1;
            
            await Clients.Client(connectionId).SendAsync("ReceivedItems", result);
            await Clients.Client(connectionId).SendAsync("ReceivedItemsCount", pageCount);

            result.Shuffle();
            
            foreach (var item in result)
            {
                await Clients.Clients(connectionId).SendAsync("ReceivedItemImage", item.Id, _imgService.GetImageUrl(item.Id, _categoryEntitityMap[category]));          
            }
        }

        /// <summary>
        /// Auto load manufacturer if exists
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="category"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task GetManufacturer(string connectionId, string category, string value)
        {
            string result = string.Empty;
            switch (category)
            {
                case "adc":
                    var adc_manufacturer = await _techInfoRepository.Adcs.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Data == value);
                    if (adc_manufacturer?.Manufacturer != null)
                    {
                        result = adc_manufacturer.Manufacturer.Data;
                    }
                    break;
                case "amp":
                    var amp_manufacturer = await _techInfoRepository.Amplifiers.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Data == value);
                    if (amp_manufacturer?.Manufacturer != null)
                    {
                        result = amp_manufacturer.Manufacturer.Data;
                    }
                    break;
                case "cartridge":
                    var cartrige_manufacturer = await _techInfoRepository.Cartriges.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Data == value);
                    if (cartrige_manufacturer?.Manufacturer != null)
                    {
                        result = cartrige_manufacturer.Manufacturer.Data;
                    }
                    break;
                case "player":
                    var player_manufacturer = await _techInfoRepository.Players.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Data == value);
                    if (player_manufacturer?.Manufacturer != null)
                    {
                        result = player_manufacturer.Manufacturer.Data;
                    }
                    break;
                case "wire":
                    var wire_manufacturer = await _techInfoRepository.Wires.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Data == value);
                    if (wire_manufacturer?.Manufacturer != null)
                    {
                        result = wire_manufacturer.Manufacturer.Data;
                    }
                    break;
            }
            await Clients.Client(connectionId).SendAsync("ReceivedManufacturer", category, result);
        }

        public async Task GetTechnicalInfoIcons(string connectionId, int albumId)
        {
            var tInfo = await _techInfoRepository.TechInfos.FirstOrDefaultAsync(x => x.AlbumId == albumId);

            if (tInfo?.VinylStateId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "vinylstate", _imgService.GetImageUrl(tInfo.VinylStateId.Value, EntityType.VinylState));
            }

            if (tInfo?.DigitalFormatId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "digitalformat", _imgService.GetImageUrl(tInfo.DigitalFormatId.Value, EntityType.DigitalFormat));
            }

            if (tInfo?.BitnessId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "bitness", _imgService.GetImageUrl(tInfo.BitnessId.Value, EntityType.Bitness));
            }

            if (tInfo?.SamplingId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "sampling", _imgService.GetImageUrl(tInfo.SamplingId.Value, EntityType.Sampling));
            }

            if (tInfo?.SourceFormatId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "format", _imgService.GetImageUrl(tInfo.SourceFormatId.Value, EntityType.SourceFormat));
            }

            if (tInfo?.PlayerId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "player", _imgService.GetImageUrl(tInfo.PlayerId.Value, EntityType.Player));
            }

            if (tInfo?.CartrigeId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "cartridge", _imgService.GetImageUrl(tInfo.CartrigeId.Value, EntityType.Cartridge));
            }

            if (tInfo?.AmplifierId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "amp", _imgService.GetImageUrl(tInfo.AmplifierId.Value, EntityType.Amp));
            }

            if (tInfo?.AdcId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "adc", _imgService.GetImageUrl(tInfo.AdcId.Value, EntityType.Adc));
            }

            if (tInfo?.WireId != null)
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "wire", _imgService.GetImageUrl(tInfo.WireId.Value, EntityType.Wire));
            }
        }
    }
}
