using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web.Db;
using Web.Enums;
using Web.Response;
using Web.Services;
using Web.ViewModels;

namespace Web.SignalRHubs
{
    public class DefaultHub : Hub
    {
        private readonly IImageService _imgService;
        private readonly ITechInfoRepository _techInfoRepository;
        private readonly IAlbumRepository _albumRepository;
        private const int ITEMS_PER_PAGE = 20;

        public DefaultHub(IImageService coverImageService, ITechInfoRepository techInfoRepository, IAlbumRepository albumRepository)
        {
            _imgService = coverImageService;
            _techInfoRepository = techInfoRepository;
            _albumRepository = albumRepository;
        }

        private readonly Dictionary<string, EntityType> _categoryEntitityMap = new Dictionary<string, EntityType>()
        {
            { "adc", EntityType.Adc },
            { "amplifier", EntityType.Amplifier },
            { "cartridge", EntityType.Cartridge },
            { "player", EntityType.Player },
            { "wire", EntityType.Wire },
        };

        #region Albums workload
        
        /// <summary>
        /// Get album covers
        /// </summary>
        /// <param name="connectionId">Connection Id</param>
        /// <param name="albums">List of album ids</param>
        /// <returns></returns>
        public async Task GetAlbumCovers(string connectionId, int[] albums)
        {
            await Task.WhenAll(albums.Select(async albumId => await Clients.Client(connectionId).SendAsync("ReceivedAlbumConver", albumId, _imgService.GetImageUrl(albumId, EntityType.AlbumCover))));
        }

        /// <summary>
        /// Get cover of specific album 
        /// </summary>
        /// <param name="connectionId">Connection Id</param>
        /// <param name="albumId">Album Id</param>
        /// <returns></returns>
        public async Task GetAlbumCover(string connectionId, int albumId)
        {
            await Clients.Client(connectionId).SendAsync("ReceivedAlbumConverDetailed", _imgService.GetImageUrl(albumId, EntityType.AlbumCover));
        }

        public async Task GetEquipmentImage(string connectionId, int equipmentId, string type)
        {
            await Clients.Client(connectionId).SendAsync("ReceivedEquipmentImageDetailed", _imgService.GetImageUrl(equipmentId, Enum.Parse<EntityType>(type)));
        }

        /// <summary>
        /// Check if similar albums already exists in db / possible dublications checker
        /// </summary>
        /// <param name="connectionId">Connection Id</param>
        /// <param name="currentAlbum">Current album Id</param>
        /// <param name="album">Album title</param>
        /// <param name="artist">Artist title</param>
        /// <param name="source">Album sources</param>
        /// <returns></returns>
        public async Task CheckAlbum(string connectionId, int currentAlbum, string album, string artist, string source)
        {
            var result = await _albumRepository.Albums.Include(x => x.Artist).Where(x => x.Data == album && x.Artist.Data == artist && x.Id != currentAlbum).ToListAsync();

            if (result?.Count > 0)
            {
                // "100 or 50" is detection level.
                // 100 means that user trying add album to db that alredy exists from same source
                // 50 means that album already exists but with different properties (another release, digitized hardware, etc.)
                if (source != null)
                {
                    var containsSource = result.Where(x => x.Source == source).Select(x => x.Id).ToArray();

                    if (containsSource.Length > 0)
                    {
                        await Clients.Client(connectionId).SendAsync("AlbumIsExist", 100, containsSource);
                    }
                    else
                    {
                        await Clients.Client(connectionId).SendAsync("AlbumIsExist", 50, result.Select(x => x.Id).ToArray());
                    }
                }
                else
                {
                    await Clients.Client(connectionId).SendAsync("AlbumIsExist", 50, result.Select(x => x.Id).ToArray());
                }
            }
            else
            {
                // if nothing found send 0 for reset warn message (if set)
                await Clients.Client(connectionId).SendAsync("AlbumIsExist", 0, 0);
            }
        }
        #endregion

        #region TechInfo workload
        // TODO: Is it possible to refactor all this methods to single (generic?) or by using expression trees?

        /// <summary>
        /// Get list of adc
        /// </summary>
        /// <param name="page">Page number</param>
        /// <returns></returns>
        private async Task<List<EquipmentViewModel>> GetAdcItems(int page)
        {
            return await _techInfoRepository.Adcs
                .Skip((page - 1) * ITEMS_PER_PAGE)
                .Take(ITEMS_PER_PAGE)
                .Include(x => x.Manufacturer)
                .Select(x => new EquipmentViewModel()
                {
                    Id = x.Id,
                    Model = x.Data,
                    Manufacturer = x.Manufacturer.Data,
                    EquipmentType = EntityType.Adc
                })
                .ToListAsync();
        }

        /// <summary>
        /// Get list of amp
        /// </summary>
        /// <param name="page">Page number</param>
        /// <returns></returns>
        private async Task<List<EquipmentViewModel>> GetAmplifierItems(int page)
        {
            return await _techInfoRepository.Amplifiers
                .Skip((page - 1) * ITEMS_PER_PAGE)
                .Take(ITEMS_PER_PAGE)
                .Include(x => x.Manufacturer)
                .Select(x => new EquipmentViewModel()
                {
                    Id = x.Id,
                    Model = x.Data,
                    Manufacturer = x.Manufacturer.Data,
                    EquipmentType = EntityType.Amplifier
                }).ToListAsync();
        }

        /// <summary>
        /// Get list of cartridges
        /// </summary>
        /// <param name="page">Page number</param>
        /// <returns></returns>
        private async Task<List<EquipmentViewModel>> GetCartridgeItems(int page)
        {
            return await _techInfoRepository.Cartridges
                .Skip((page - 1) * ITEMS_PER_PAGE)
                .Take(ITEMS_PER_PAGE)
                .Include(x => x.Manufacturer)
                .Select(x => new EquipmentViewModel()
                {
                    Id = x.Id,
                    Model = x.Data,
                    Manufacturer = x.Manufacturer.Data,
                    EquipmentType = EntityType.Cartridge
                }).ToListAsync();
        }

        /// <summary>
        /// Get list of players
        /// </summary>
        /// <param name="page">Page number</param>
        /// <returns></returns>
        private async Task<List<EquipmentViewModel>> GetPlayerItems(int page)
        {
            return await _techInfoRepository.Players
                .Skip((page - 1) * ITEMS_PER_PAGE)
                .Take(ITEMS_PER_PAGE)
                .Include(x => x.Manufacturer)
                .Select(x => new EquipmentViewModel()
                {
                    Id = x.Id,
                    Model = x.Data,
                    Manufacturer = x.Manufacturer.Data,
                    EquipmentType = EntityType.Player
                }).ToListAsync();
        }

        /// <summary>
        /// Get list of wires
        /// </summary>
        /// <param name="page">Page number</param>
        /// <returns></returns>
        private async Task<List<EquipmentViewModel>> GetWireItems(int page)
        {
            return await _techInfoRepository.Wires
                .Skip((page - 1) * ITEMS_PER_PAGE)
                .Take(ITEMS_PER_PAGE)
                .Include(x => x.Manufacturer)
                .Select(x => new EquipmentViewModel()
                {
                    Id = x.Id,
                    Model = x.Data,
                    Manufacturer = x.Manufacturer.Data,
                    EquipmentType = EntityType.Amplifier
                }).ToListAsync();
        }

        /// <summary>
        /// Get hardware by category
        /// </summary>
        /// <param name="connectionId">Connection Id</param>
        /// <param name="category">Category</param>
        /// <param name="page">Page number</param>
        /// <returns></returns>
        public async Task GetHardwareByCategory(string connectionId, string category, int page)
        {
            var result = new List<EquipmentViewModel>();
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
                case "cartridge":
                    itemsCount = await _techInfoRepository.Cartridges.CountAsync();
                    result = await GetCartridgeItems(page);
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

            foreach (var item in result)
            {
                await Clients.Clients(connectionId).SendAsync("ReceivedItemImage", item.Id, _imgService.GetImageUrl(item.Id, _categoryEntitityMap[category]));
            }
        }

        /// <summary>
        /// Auto load manufacturer if exists for specific hardware
        /// </summary>
        /// <param name="connectionId">Connection Id</param>
        /// <param name="category">Category</param>
        /// <param name="value">Value from input</param>
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
                    var cartridge_manufacturer = await _techInfoRepository.Cartridges.Include(x => x.Manufacturer).FirstOrDefaultAsync(x => x.Data == value);
                    if (cartridge_manufacturer?.Manufacturer != null)
                    {
                        result = cartridge_manufacturer.Manufacturer.Data;
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

        //TODO: Make more generic? Just detect entity type, all another parts same
        /// <summary>
        /// Get technical info icons for album
        /// </summary>
        /// <param name="connectionId">Connection Id</param>
        /// <param name="albumId">Album ID</param>
        /// <returns></returns>
        public async Task GetTechnicalInfoIcons(string connectionId, int albumId)
        {
            var tInfo = await _techInfoRepository.TechInfos.FirstOrDefaultAsync(x => x.AlbumId == albumId);

            if (tInfo != null)
            {
                if (tInfo?.VinylStateId != null)
                {
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "vinylstate", _imgService.GetIconUrl(tInfo.VinylStateId.Value, EntityType.VinylState));
                }

                if (tInfo?.DigitalFormatId != null)
                {
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "digitalformat", _imgService.GetIconUrl(tInfo.DigitalFormatId.Value, EntityType.DigitalFormat));
                }

                if (tInfo?.BitnessId != null)
                {
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "bitness", _imgService.GetIconUrl(tInfo.BitnessId.Value, EntityType.Bitness));
                }

                if (tInfo?.SamplingId != null)
                {
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "sampling", _imgService.GetIconUrl(tInfo.SamplingId.Value, EntityType.Sampling));
                }

                if (tInfo?.SourceFormatId != null)
                {
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "format", _imgService.GetIconUrl(tInfo.SourceFormatId.Value, EntityType.SourceFormat));
                }

                if (tInfo?.PlayerId != null)
                {
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "player", _imgService.GetImageUrl(tInfo.PlayerId.Value, EntityType.Player));
                }

                if (tInfo?.CartridgeId != null)
                {
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "cartridge", _imgService.GetImageUrl(tInfo.CartridgeId.Value, EntityType.Cartridge));
                }

                if (tInfo?.AmplifierId != null)
                {
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "amp", _imgService.GetImageUrl(tInfo.AmplifierId.Value, EntityType.Amplifier));
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

        #endregion
    }
}
