using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.Request;
using Web.Services;

namespace Web.SignalRHubs
{
    public class AlbumHub(
        IImageService imageService,
        IDigitizationRepository digitizationRepository,
        IAlbumRepository albumRepository,
        IAlbumService albumService,
        IDigitizationService digitizationService,
        IEquipmentRepository equipmentRepository,
        EntityFindOrCreateService entityService) : Hub
    {
        private readonly IImageService _imgService = imageService;
        private readonly IDigitizationRepository _digitizationRepository = digitizationRepository;
        private readonly IAlbumRepository _albumRepository = albumRepository;
        private readonly IAlbumService _albumService = albumService;
        private readonly IDigitizationService _digitizationService = digitizationService;
        private readonly IEquipmentRepository _equipmentRepository = equipmentRepository;
        private readonly EntityFindOrCreateService _entityService = entityService;
        private static readonly ConcurrentDictionary<int, string> _coverCache = new();

        private readonly Dictionary<string, EntityType> _categoryEntityMap = new Dictionary<string, EntityType>()
        {
            { "adc", EntityType.Adc },
            { "amplifier", EntityType.Amplifier },
            { "cartridge", EntityType.Cartridge },
            { "player", EntityType.Player },
            { "wire", EntityType.Wire },
        };

        /// <summary>
        /// Get album covers
        /// </summary>
        public async Task GetAlbumCovers(string connectionId, int[] albums)
        {
            Random.Shared.Shuffle(albums);

            foreach (var albumId in albums)
            {
                var cover = await GetCachedAlbumCoverAsync(albumId);
                await Clients.Client(connectionId).SendAsync("ReceivedAlbumCover", albumId, cover);
                await Task.Delay(100);
            }
        }

        private async Task<string> GetCachedAlbumCoverAsync(int albumId)
        {
            if (_coverCache.TryGetValue(albumId, out var cachedCover))
                return cachedCover;

            var cover = await _imgService.GetImageUrlAsync(albumId, EntityType.AlbumCover);
            _coverCache.TryAdd(albumId, cover);
            return cover;
        }

        public static void InvalidateAlbumCache(int albumId)
        {
            _coverCache.TryRemove(albumId, out _);
        }

        /// <summary>
        /// Get cover of specific album 
        /// </summary>
        public async Task GetAlbumCover(string connectionId, int albumId)
        {
            var imageUrl = await _imgService.GetImageUrlAsync(albumId, EntityType.AlbumCover);
            await Clients.Client(connectionId).SendAsync("ReceivedAlbumCoverDetailed", imageUrl);
        }

        public async Task CheckAlbum(string connectionId, int albumId, string album, string artist, string source)
        {
            var result = await _albumRepository.FindByAlbumAndArtistAsync(album, artist);

            if (result is null)
            {
                await Clients.Client(connectionId).SendAsync("AlbumIsExist", 0, 0);
                return;
            }

            if (result.Id != albumId)
            {
                await Clients.Client(connectionId).SendAsync("AlbumIsExist", 1, result.Id);
                return;
            }

            if (!string.IsNullOrWhiteSpace(source))
            {
                var sourceExists = await _digitizationRepository.ExistsByAlbumIdAndSourceAsync(result.Id, source);
                if (sourceExists)
                {
                    await Clients.Client(connectionId).SendAsync("AlbumIsExist", 100, result.Id);
                    return;
                }
            }

            await Clients.Client(connectionId).SendAsync("AlbumIsExist", 0, 0);
        }

        public async Task AddDigitization(string connectionId, CreateUpdateDigitizationRequest request)
        {
            try
            {
                Album album;
                
                // if no album - create one
                if (request.AlbumId == 0)
                {
                    album = await _albumService.CreateOrFindAlbumAsync(request.Album, request.Artist, request.Genre);
                }
                else
                {
                    album = await _albumRepository.GetByIdAsync(request.AlbumId);
                    if (album == null)
                    {
                        await Clients.Client(connectionId).SendAsync("DigitizationAdded", false, "Album not found", 0);
                        return;
                    }
                }

                var digitization = await MapRequestToDigitizationAsync(album.Id, request);
                digitization = await _digitizationService.AddAsync(digitization);

                // Get all digitizations for the album
                var digitizations = await _digitizationService.GetByAlbumIdAsync(album.Id);
                var digitizationList = MapDigitizationsToDto(digitizations);

                await Clients.Client(connectionId).SendAsync("DigitizationAdded", true, "", album.Id, digitizationList);
            }
            catch (Exception ex)
            {
                await Clients.Client(connectionId).SendAsync("DigitizationAdded", false, ex.Message, 0);
            }
        }

        public async Task UpdateDigitization(string connectionId, CreateUpdateDigitizationRequest request)
        {
            try
            {
                if (request.DigitizationId == 0)
                {
                    await Clients.Client(connectionId).SendAsync("DigitizationUpdated", false, "Digitization ID is required");
                    return;
                }

                var existing = await _digitizationRepository.GetByIdAsync(request.DigitizationId);
                if (existing == null)
                {
                    await Clients.Client(connectionId).SendAsync("DigitizationUpdated", false, "Digitization not found");
                    return;
                }

                var digitization = await MapRequestToDigitizationAsync(existing.AlbumId, request);
                digitization.Id = request.DigitizationId;
                digitization = await _digitizationService.UpdateAsync(digitization);

                // Get all digitizations for the album
                var digitizations = await _digitizationService.GetByAlbumIdAsync(existing.AlbumId);
                var digitizationList = MapDigitizationsToDto(digitizations);

                await Clients.Client(connectionId).SendAsync("DigitizationUpdated", true, "", digitizationList);
            }
            catch (Exception ex)
            {
                await Clients.Client(connectionId).SendAsync("DigitizationUpdated", false, ex.Message);
            }
        }

        public async Task RemoveDigitization(string connectionId, int digitizationId)
        {
            try
            {
                var digitization = await _digitizationRepository.GetByIdAsync(digitizationId);
                if (digitization == null)
                {
                    await Clients.Client(connectionId).SendAsync("DigitizationRemoved", false, "Digitization not found");
                    return;
                }

                var albumId = digitization.AlbumId;
                var success = await _digitizationService.DeleteAsync(digitizationId);

                if (success)
                {
                    // Get all remaining digitizations for the album
                    var digitizations = await _digitizationService.GetByAlbumIdAsync(albumId);
                    var digitizationList = MapDigitizationsToDto(digitizations);

                    await Clients.Client(connectionId).SendAsync("DigitizationRemoved", true, "", digitizationList);
                }
                else
                {
                    await Clients.Client(connectionId).SendAsync("DigitizationRemoved", false, "Failed to remove digitization");
                }
            }
            catch (Exception ex)
            {
                await Clients.Client(connectionId).SendAsync("DigitizationRemoved", false, ex.Message);
            }
        }

        public async Task GetManufacturer(string connectionId, string category, string value)
        {
            if (!_categoryEntityMap.TryGetValue(category, out var type))
            {
                await Clients.Client(connectionId).SendAsync("ReceivedManufacturer", category, string.Empty);
                return;
            }

            var item = await _equipmentRepository.GetManufacturerByNameAsync(value, type);
            var result = item?.Manufacturer?.Name ?? string.Empty;

            await Clients.Client(connectionId).SendAsync("ReceivedManufacturer", category, result);
        }

        public async Task GetTechnicalInfoIcons(string connectionId, int digitizationId)
        {
            var digitization = await _digitizationRepository.GetByIdAsync(digitizationId);

            if (digitization == null)
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfo", null, null);
                return;
            }

            var formatInfo = digitization.FormatInfo;
            var equipmentInfo = digitization.EquipmentInfo;

            if (formatInfo == null && equipmentInfo == null)
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfo", null, null);
                return;
            }

            var mapping = new Dictionary<string, (int? id, EntityType type)>
            {
                ["vinylstate"] = (formatInfo?.VinylStateId, EntityType.VinylState),
                ["digitalformat"] = (formatInfo?.DigitalFormatId, EntityType.DigitalFormat),
                ["bitness"] = (formatInfo?.BitnessId, EntityType.Bitness),
                ["sampling"] = (formatInfo?.SamplingId, EntityType.Sampling),
                ["format"] = (formatInfo?.SourceFormatId, EntityType.SourceFormat),
                ["player"] = (equipmentInfo?.PlayerId, EntityType.Player),
                ["cartridge"] = (equipmentInfo?.CartridgeId, EntityType.Cartridge),
                ["amp"] = (equipmentInfo?.AmplifierId, EntityType.Amplifier),
                ["adc"] = (equipmentInfo?.AdcId, EntityType.Adc),
                ["wire"] = (equipmentInfo?.WireId, EntityType.Wire),
            };

            foreach (var kvp in mapping)
            {
                string category = kvp.Key;
                string? url = null;

                if (kvp.Value.id.HasValue)
                    url = await _imgService.GetImageUrlAsync(kvp.Value.id.Value, kvp.Value.type);

                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", category, url);
            }
        }

        private async Task<Digitization> MapRequestToDigitizationAsync(int albumId, CreateUpdateDigitizationRequest request)
        {
            var digitization = new Digitization
            {
                AlbumId = albumId,
                AddedDate = DateTime.Now,
                Source = request.Source,
                Discogs = request.Discogs,
                IsFirstPress = request.IsFirstPress,
                Size = request.Size
            };

            // Find or create Year
            if (request.Year.HasValue)
            {
                var year = await _entityService.FindOrCreateYearAsync(request.Year.Value);
                digitization.YearId = year.Id;
            }

            // Find or create Reissue
            if (request.Reissue.HasValue)
            {
                var reissue = await _entityService.FindOrCreateReissueAsync(request.Reissue.Value);
                digitization.ReissueId = reissue.Id;
            }

            // Find or create Country
            if (!string.IsNullOrWhiteSpace(request.Country))
            {
                var country = await _entityService.FindOrCreateCountryAsync(request.Country);
                digitization.CountryId = country.Id;
            }

            // Find or create Label
            if (!string.IsNullOrWhiteSpace(request.Label))
            {
                var label = await _entityService.FindOrCreateLabelAsync(request.Label);
                digitization.LabelId = label.Id;
            }

            // Find or create Storage
            if (!string.IsNullOrWhiteSpace(request.Storage))
            {
                var storage = await _entityService.FindOrCreateStorageAsync(request.Storage);
                digitization.StorageId = storage.Id;
            }

            // FormatInfo
            var formatInfo = new FormatInfo();

            // Find or create Bitness
            if (request.Bitness.HasValue)
            {
                var bitness = await _entityService.FindOrCreateBitnessAsync(request.Bitness.Value);
                formatInfo.BitnessId = bitness.Id;
            }

            // Find or create Sampling
            if (request.Sampling.HasValue)
            {
                var sampling = await _entityService.FindOrCreateSamplingAsync(request.Sampling.Value);
                formatInfo.SamplingId = sampling.Id;
            }

            // Find or create DigitalFormat
            if (!string.IsNullOrWhiteSpace(request.DigitalFormat))
            {
                var digitalFormat = await _entityService.FindOrCreateDigitalFormatAsync(request.DigitalFormat);
                formatInfo.DigitalFormatId = digitalFormat.Id;
            }

            // Find or create SourceFormat
            if (!string.IsNullOrWhiteSpace(request.SourceFormat))
            {
                var sourceFormat = await _entityService.FindOrCreateSourceFormatAsync(request.SourceFormat);
                formatInfo.SourceFormatId = sourceFormat.Id;
            }

            // Find or create VinylState
            if (!string.IsNullOrWhiteSpace(request.VinylState))
            {
                var vinylState = await _entityService.FindOrCreateVinylStateAsync(request.VinylState);
                formatInfo.VinylStateId = vinylState.Id;
            }

            digitization.FormatInfo = formatInfo;

            // EquipmentInfo
            var equipmentInfo = new EquipmentInfo();

            // Find or create Player
            if (!string.IsNullOrWhiteSpace(request.Player))
            {
                var player = await _entityService.FindOrCreatePlayerAsync(request.Player, request.PlayerManufacturer);
                equipmentInfo.PlayerId = player.Id;
            }

            // Find or create Cartridge
            if (!string.IsNullOrWhiteSpace(request.Cartridge))
            {
                var cartridge = await _entityService.FindOrCreateCartridgeAsync(request.Cartridge, request.CartridgeManufacturer);
                equipmentInfo.CartridgeId = cartridge.Id;
            }

            // Find or create Amplifier
            if (!string.IsNullOrWhiteSpace(request.Amplifier))
            {
                var amplifier = await _entityService.FindOrCreateAmplifierAsync(request.Amplifier, request.AmplifierManufacturer);
                equipmentInfo.AmplifierId = amplifier.Id;
            }

            // Find or create Adc
            if (!string.IsNullOrWhiteSpace(request.Adc))
            {
                var adc = await _entityService.FindOrCreateAdcAsync(request.Adc, request.AdcManufacturer);
                equipmentInfo.AdcId = adc.Id;
            }

            // Find or create Wire
            if (!string.IsNullOrWhiteSpace(request.Wire))
            {
                var wire = await _entityService.FindOrCreateWireAsync(request.Wire, request.WireManufacturer);
                equipmentInfo.WireId = wire.Id;
            }

            digitization.EquipmentInfo = equipmentInfo;

            return digitization;
        }

        private static List<object> MapDigitizationsToDto(IEnumerable<Digitization> digitizations)
        {
            return digitizations.Select(d => new
            {
                Id = d.Id,
                VinylState = d.FormatInfo?.VinylState?.Name,
                Bitness = d.FormatInfo?.Bitness?.Value,
                Sampling = d.FormatInfo?.Sampling?.Value,
                DigitalFormat = d.FormatInfo?.DigitalFormat?.Name,
                SourceFormat = d.FormatInfo?.SourceFormat?.Name,
                Player = d.EquipmentInfo?.Player?.Name,
                PlayerManufacturer = d.EquipmentInfo?.Player?.Manufacturer?.Name,
                Cartridge = d.EquipmentInfo?.Cartridge?.Name,
                CartridgeManufacturer = d.EquipmentInfo?.Cartridge?.Manufacturer?.Name,
                Amplifier = d.EquipmentInfo?.Amplifier?.Name,
                AmplifierManufacturer = d.EquipmentInfo?.Amplifier?.Manufacturer?.Name,
                Adc = d.EquipmentInfo?.Adc?.Name,
                AdcManufacturer = d.EquipmentInfo?.Adc?.Manufacturer?.Name,
                Wire = d.EquipmentInfo?.Wire?.Name,
                WireManufacturer = d.EquipmentInfo?.Wire?.Manufacturer?.Name,
                Source = d.Source,
                Year = d.Year?.Value,
                Reissue = d.Reissue?.Value,
                Country = d.Country?.Name,
                Label = d.Label?.Name,
                Storage = d.Storage?.Data,
                Discogs = d.Discogs,
                Size = d.Size
            }).ToList<object>();
        }
    }
}

