using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.Request;
using Web.Services;
using Web.ViewModels;

namespace Web.SignalRHubs
{
    public class DefaultHub: Hub
    {
        private readonly IImageService _imgService;
        private readonly IDigitizationRepository _digitizationRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly IAlbumService _albumService;
        private readonly IDigitizationService _digitizationService;
        private readonly IPostRepository _postRepository;
        private readonly IPostService _postService;
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly DMADbContext _context;
        private static readonly ConcurrentDictionary<int, string> _coverCache = new();
        private const int ITEMS_PER_PAGE = 18;
        private const int POSTS_PER_PAGE = 10;

        public DefaultHub(
            IImageService coverImageService, 
            IDigitizationRepository digitizationRepository, 
            IAlbumRepository albumRepository,
            IAlbumService albumService,
            IDigitizationService digitizationService,
            IPostRepository postRepository,
            IPostService postService,
            IEquipmentRepository equipmentRepository,
            DMADbContext context)
        {
            _imgService = coverImageService;
            _digitizationRepository = digitizationRepository;
            _albumRepository = albumRepository;
            _albumService = albumService;
            _digitizationService = digitizationService;
            _postRepository = postRepository;
            _postService = postService;
            _equipmentRepository = equipmentRepository;
            _context = context;
        }

        private readonly Dictionary<string, EntityType> _categoryEntityMap = new Dictionary<string, EntityType>()
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
            Random.Shared.Shuffle(albums);

            foreach (var albumId in albums)
            {
                var cover = GetCachedAlbumCover(albumId);
                await Clients.Client(connectionId).SendAsync("ReceivedAlbumCover", albumId, cover);
                await Task.Delay(100);
            }
        }

        private string GetCachedAlbumCover(int albumId)
        {
            return _coverCache.GetOrAdd(albumId, id => _imgService.GetImageUrl(id, EntityType.AlbumCover));
        }

        public static void InvalidateAlbumCache(int albumId)
        {
            _coverCache.TryRemove(albumId, out _);
        }

        /// <summary>
        /// Get cover of specific album 
        /// </summary>
        /// <param name="connectionId">Connection Id</param>
        /// <param name="albumId">Album Id</param>
        /// <returns></returns>
        public async Task GetAlbumCover(string connectionId, int albumId)
        {
            await Clients.Client(connectionId).SendAsync("ReceivedAlbumCoverDetailed", _imgService.GetImageUrl(albumId, EntityType.AlbumCover));
        }

        public async Task GetEquipmentImage(string connectionId, int equipmentId, string type)
        {
            await Clients.Client(connectionId).SendAsync("ReceivedEquipmentImageDetailed", _imgService.GetImageUrl(equipmentId, Enum.Parse<EntityType>(type)));
        }

        public async Task CheckAlbum(string connectionId, int currentAlbum, string album, string artist, string source)
        {
            var result = await _albumRepository.FindByTitleAndArtistAsync(album, artist);

            if (result is not null)
            {
                // "100 or 50" is detection level.
                // 100 means that user trying add album to db that alredy exists from same source
                // 50 means that album already exists but with different properties (another release, digitized hardware, etc.)
                /*
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
                }*/
            }
            else
            {
                // if nothing found send 0 for reset warn message (if set)
                await Clients.Client(connectionId).SendAsync("AlbumIsExist", 0, 0);
            }
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
                var digitizationList = digitizations.Select(d => new
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
                    Size = d.FormatInfo?.Size
                }).ToList();

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
                var digitizationList = digitizations.Select(d => new
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
                    Size = d.FormatInfo?.Size
                }).ToList();

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
                    var digitizationList = digitizations.Select(d => new
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
                        Size = d.FormatInfo?.Size
                    }).ToList();

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

        private async Task<Digitization> MapRequestToDigitizationAsync(int albumId, CreateUpdateDigitizationRequest request)
        {
            var digitization = new Digitization
            {
                AlbumId = albumId,
                AddedDate = DateTime.Now,
                Source = request.Source,
                Discogs = request.Discogs,
                IsFirstPress = request.IsFirstPress
            };

            // Find or create Year
            if (request.Year.HasValue)
            {
                var year = await FindOrCreateYearAsync(request.Year.Value);
                digitization.YearId = year.Id;
            }

            // Find or create Reissue
            if (request.Reissue.HasValue)
            {
                var reissue = await FindOrCreateReissueAsync(request.Reissue.Value);
                digitization.ReissueId = reissue.Id;
            }

            // Find or create Country
            if (!string.IsNullOrWhiteSpace(request.Country))
            {
                var country = await FindOrCreateCountryAsync(request.Country);
                digitization.CountryId = country.Id;
            }

            // Find or create Label
            if (!string.IsNullOrWhiteSpace(request.Label))
            {
                var label = await FindOrCreateLabelAsync(request.Label);
                digitization.LabelId = label.Id;
            }

            // Find or create Storage
            if (!string.IsNullOrWhiteSpace(request.Storage))
            {
                var storage = await FindOrCreateStorageAsync(request.Storage);
                digitization.StorageId = storage.Id;
            }

            // FormatInfo
            var formatInfo = new FormatInfo
            {
                Size = request.Size
            };

            // Find or create Bitness
            if (request.Bitness.HasValue)
            {
                var bitness = await FindOrCreateBitnessAsync(request.Bitness.Value);
                formatInfo.BitnessId = bitness.Id;
            }

            // Find or create Sampling
            if (request.Sampling.HasValue)
            {
                var sampling = await FindOrCreateSamplingAsync(request.Sampling.Value);
                formatInfo.SamplingId = sampling.Id;
            }

            // Find or create DigitalFormat
            if (!string.IsNullOrWhiteSpace(request.DigitalFormat))
            {
                var digitalFormat = await FindOrCreateDigitalFormatAsync(request.DigitalFormat);
                formatInfo.DigitalFormatId = digitalFormat.Id;
            }

            // Find or create SourceFormat
            if (!string.IsNullOrWhiteSpace(request.SourceFormat))
            {
                var sourceFormat = await FindOrCreateSourceFormatAsync(request.SourceFormat);
                formatInfo.SourceFormatId = sourceFormat.Id;
            }

            // Find or create VinylState
            if (!string.IsNullOrWhiteSpace(request.VinylState))
            {
                var vinylState = await FindOrCreateVinylStateAsync(request.VinylState);
                formatInfo.VinylStateId = vinylState.Id;
            }

            digitization.FormatInfo = formatInfo;

            // EquipmentInfo
            var equipmentInfo = new EquipmentInfo();

            // Find or create Player
            if (!string.IsNullOrWhiteSpace(request.Player))
            {
                var player = await FindOrCreatePlayerAsync(request.Player, request.PlayerManufacturer);
                equipmentInfo.PlayerId = player.Id;
            }

            // Find or create Cartridge
            if (!string.IsNullOrWhiteSpace(request.Cartridge))
            {
                var cartridge = await FindOrCreateCartridgeAsync(request.Cartridge, request.CartridgeManufacturer);
                equipmentInfo.CartridgeId = cartridge.Id;
            }

            // Find or create Amplifier
            if (!string.IsNullOrWhiteSpace(request.Amplifier))
            {
                var amplifier = await FindOrCreateAmplifierAsync(request.Amplifier, request.AmplifierManufacturer);
                equipmentInfo.AmplifierId = amplifier.Id;
            }

            // Find or create Adc
            if (!string.IsNullOrWhiteSpace(request.Adc))
            {
                var adc = await FindOrCreateAdcAsync(request.Adc, request.AdcManufacturer);
                equipmentInfo.AdcId = adc.Id;
            }

            // Find or create Wire
            if (!string.IsNullOrWhiteSpace(request.Wire))
            {
                var wire = await FindOrCreateWireAsync(request.Wire, request.WireManufacturer);
                equipmentInfo.WireId = wire.Id;
            }

            digitization.EquipmentInfo = equipmentInfo;

            return digitization;
        }

        #region Find or Create Helper Methods

        private async Task<Year> FindOrCreateYearAsync(int yearValue)
        {
            var year = await _context.Years.FirstOrDefaultAsync(y => y.Value == yearValue);
            if (year == null)
            {
                year = new Year { Value = yearValue };
                _context.Years.Add(year);
                await _context.SaveChangesAsync();
            }
            return year;
        }

        private async Task<Reissue> FindOrCreateReissueAsync(int reissueValue)
        {
            var reissue = await _context.Reissues.FirstOrDefaultAsync(r => r.Value == reissueValue);
            if (reissue == null)
            {
                reissue = new Reissue { Value = reissueValue };
                _context.Reissues.Add(reissue);
                await _context.SaveChangesAsync();
            }
            return reissue;
        }

        private async Task<Country> FindOrCreateCountryAsync(string countryName)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(c => c.Name.ToLower() == countryName.ToLower());
            if (country == null)
            {
                country = new Country { Name = countryName };
                _context.Countries.Add(country);
                await _context.SaveChangesAsync();
            }
            return country;
        }

        private async Task<Label> FindOrCreateLabelAsync(string labelName)
        {
            var label = await _context.Labels.FirstOrDefaultAsync(l => l.Name.ToLower() == labelName.ToLower());
            if (label == null)
            {
                label = new Label { Name = labelName };
                _context.Labels.Add(label);
                await _context.SaveChangesAsync();
            }
            return label;
        }

        private async Task<Storage> FindOrCreateStorageAsync(string storageData)
        {
            var storage = await _context.Storages.FirstOrDefaultAsync(s => s.Data.ToLower() == storageData.ToLower());
            if (storage == null)
            {
                storage = new Storage { Data = storageData };
                _context.Storages.Add(storage);
                await _context.SaveChangesAsync();
            }
            return storage;
        }

        private async Task<Bitness> FindOrCreateBitnessAsync(int bitnessValue)
        {
            var bitness = await _context.Bitnesses.FirstOrDefaultAsync(b => b.Value == bitnessValue);
            if (bitness == null)
            {
                bitness = new Bitness { Value = bitnessValue };
                _context.Bitnesses.Add(bitness);
                await _context.SaveChangesAsync();
            }
            return bitness;
        }

        private async Task<Sampling> FindOrCreateSamplingAsync(double samplingValue)
        {
            var sampling = await _context.Samplings.FirstOrDefaultAsync(s => s.Value == samplingValue);
            if (sampling == null)
            {
                sampling = new Sampling { Value = samplingValue };
                _context.Samplings.Add(sampling);
                await _context.SaveChangesAsync();
            }
            return sampling;
        }

        private async Task<DigitalFormat> FindOrCreateDigitalFormatAsync(string formatName)
        {
            var format = await _context.DigitalFormats.FirstOrDefaultAsync(f => f.Name.ToLower() == formatName.ToLower());
            if (format == null)
            {
                format = new DigitalFormat { Name = formatName };
                _context.DigitalFormats.Add(format);
                await _context.SaveChangesAsync();
            }
            return format;
        }

        private async Task<SourceFormat> FindOrCreateSourceFormatAsync(string formatName)
        {
            var format = await _context.SourceFormats.FirstOrDefaultAsync(f => f.Name.ToLower() == formatName.ToLower());
            if (format == null)
            {
                format = new SourceFormat { Name = formatName };
                _context.SourceFormats.Add(format);
                await _context.SaveChangesAsync();
            }
            return format;
        }

        private async Task<VinylState> FindOrCreateVinylStateAsync(string stateName)
        {
            var state = await _context.VinylStates.FirstOrDefaultAsync(v => v.Name.ToLower() == stateName.ToLower());
            if (state == null)
            {
                state = new VinylState { Name = stateName };
                _context.VinylStates.Add(state);
                await _context.SaveChangesAsync();
            }
            return state;
        }

        private async Task<Player> FindOrCreatePlayerAsync(string playerName, string? manufacturerName = null)
        {
            var player = await _context.Players
                .Include(p => p.Manufacturer)
                .FirstOrDefaultAsync(p => p.Name.ToLower() == playerName.ToLower());
            
            if (player == null)
            {
                player = new Player { Name = playerName };
                
                // Find or create manufacturer if provided
                if (!string.IsNullOrWhiteSpace(manufacturerName))
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.PlayerManufacturer);
                    player.ManufacturerId = manufacturer?.Id;
                }
                
                _context.Players.Add(player);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Update manufacturer if provided and not already set
                if (!string.IsNullOrWhiteSpace(manufacturerName) && player.ManufacturerId == null)
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.PlayerManufacturer);
                    if (manufacturer != null)
                    {
                        player.ManufacturerId = manufacturer.Id;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            
            return player;
        }

        private async Task<Cartridge> FindOrCreateCartridgeAsync(string cartridgeName, string? manufacturerName = null)
        {
            var cartridge = await _context.Cartridges
                .Include(c => c.Manufacturer)
                .FirstOrDefaultAsync(c => c.Name.ToLower() == cartridgeName.ToLower());
            
            if (cartridge == null)
            {
                cartridge = new Cartridge { Name = cartridgeName };
                
                // Find or create manufacturer if provided
                if (!string.IsNullOrWhiteSpace(manufacturerName))
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.CartridgeManufacturer);
                    cartridge.ManufacturerId = manufacturer?.Id;
                }
                
                _context.Cartridges.Add(cartridge);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Update manufacturer if provided and not already set
                if (!string.IsNullOrWhiteSpace(manufacturerName) && cartridge.ManufacturerId == null)
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.CartridgeManufacturer);
                    if (manufacturer != null)
                    {
                        cartridge.ManufacturerId = manufacturer.Id;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            
            return cartridge;
        }

        private async Task<Amplifier> FindOrCreateAmplifierAsync(string amplifierName, string? manufacturerName = null)
        {
            var amplifier = await _context.Amplifiers
                .Include(a => a.Manufacturer)
                .FirstOrDefaultAsync(a => a.Name.ToLower() == amplifierName.ToLower());
            
            if (amplifier == null)
            {
                amplifier = new Amplifier { Name = amplifierName };
                
                // Find or create manufacturer if provided
                if (!string.IsNullOrWhiteSpace(manufacturerName))
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.AmplifierManufacturer);
                    amplifier.ManufacturerId = manufacturer?.Id;
                }
                
                _context.Amplifiers.Add(amplifier);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Update manufacturer if provided and not already set
                if (!string.IsNullOrWhiteSpace(manufacturerName) && amplifier.ManufacturerId == null)
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.AmplifierManufacturer);
                    if (manufacturer != null)
                    {
                        amplifier.ManufacturerId = manufacturer.Id;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            
            return amplifier;
        }

        private async Task<Adc> FindOrCreateAdcAsync(string adcName, string? manufacturerName = null)
        {
            var adc = await _context.Adces
                .Include(a => a.Manufacturer)
                .FirstOrDefaultAsync(a => a.Name.ToLower() == adcName.ToLower());
            
            if (adc == null)
            {
                adc = new Adc { Name = adcName };
                
                // Find or create manufacturer if provided
                if (!string.IsNullOrWhiteSpace(manufacturerName))
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.AdcManufacturer);
                    adc.ManufacturerId = manufacturer?.Id;
                }
                
                _context.Adces.Add(adc);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Update manufacturer if provided and not already set
                if (!string.IsNullOrWhiteSpace(manufacturerName) && adc.ManufacturerId == null)
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.AdcManufacturer);
                    if (manufacturer != null)
                    {
                        adc.ManufacturerId = manufacturer.Id;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            
            return adc;
        }

        private async Task<Wire> FindOrCreateWireAsync(string wireName, string? manufacturerName = null)
        {
            var wire = await _context.Wires
                .Include(w => w.Manufacturer)
                .FirstOrDefaultAsync(w => w.Name.ToLower() == wireName.ToLower());
            
            if (wire == null)
            {
                wire = new Wire { Name = wireName };
                
                // Find or create manufacturer if provided
                if (!string.IsNullOrWhiteSpace(manufacturerName))
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.WireManufacturer);
                    wire.ManufacturerId = manufacturer?.Id;
                }
                
                _context.Wires.Add(wire);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Update manufacturer if provided and not already set
                if (!string.IsNullOrWhiteSpace(manufacturerName) && wire.ManufacturerId == null)
                {
                    var manufacturer = await FindOrCreateManufacturerAsync(manufacturerName, EntityType.WireManufacturer);
                    if (manufacturer != null)
                    {
                        wire.ManufacturerId = manufacturer.Id;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            
            return wire;
        }

        private async Task<Manufacturer?> FindOrCreateManufacturerAsync(string manufacturerName, EntityType manufacturerType)
        {
            if (string.IsNullOrWhiteSpace(manufacturerName))
                return null;

            // First, try to find existing manufacturer by name and type (case-insensitive)
            var existingManufacturer = await _context.Manufacturer
                .FirstOrDefaultAsync(m => m.Name.ToLower() == manufacturerName.ToLower() && m.Type == manufacturerType);

            if (existingManufacturer != null)
                return existingManufacturer;

            // If not found by name and type, try to find by name only (to reuse existing manufacturers)
            var existingByName = await _context.Manufacturer
                .FirstOrDefaultAsync(m => m.Name.ToLower() == manufacturerName.ToLower());

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
                Name = manufacturerName.Trim(),
                Type = manufacturerType
            };

            _context.Manufacturer.Add(newManufacturer);
            await _context.SaveChangesAsync();

            return newManufacturer;
        }

        #endregion
        #endregion

        #region Equipment workload

        public async Task GetHardwareByCategory(string connectionId, string category, int page)
        {
            if (!_categoryEntityMap.TryGetValue(category, out var entityType))
                return;

            var pagedResult = await _equipmentRepository.GetListAsync(page, ITEMS_PER_PAGE, entityType);

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
                await Clients.Client(connectionId).SendAsync(
                    "ReceivedItemImage",
                    item.Id,
                    _imgService.GetImageUrl(item.Id, entityType)
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
                    url = _imgService.GetImageUrl(kvp.Value.id.Value, kvp.Value.type);

                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", category, url);
            }
        }

        #endregion

        #region Blog workload
        public async Task GetPosts(string connectionId, int page, string searchText, string category, string year, bool onlyDrafts)
        {
            var result = await _postRepository.GetFilteredListAsync(page, POSTS_PER_PAGE, searchText, category, year, onlyDrafts);

            var response = result.Items
                .OrderByDescending(p => p.CreatedDate ?? DateTime.MinValue)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Description,
                    p.IsDraft,
                    Created = p.CreatedDate.HasValue ? p.CreatedDate.Value.ToShortDateString() : null,
                    Categories = p.PostCategories.Select(pc => pc.Category.Title).ToList()
                }).ToList();

            await Clients.Client(connectionId)
                .SendAsync("ReceivedPosts", response, result.TotalPages);
        }

        public async Task GetBlogTree(string connectionId)
        {
            var posts = await _postRepository.GetListAsync(1, int.MaxValue);
            var allPosts = posts.Items.Where(p => !p.IsDraft && p.CreatedDate.HasValue).ToList();

            var tree = allPosts
                .SelectMany(p => p.PostCategories.Any() 
                    ? p.PostCategories.Select(pc => new { Post = p, Category = pc.Category })
                    : new[] { new { Post = p, Category = (Category?)null } })
                .GroupBy(x => x.Category?.Title ?? "Uncategorized")
                .Select(catGroup => new
                {
                    Category = catGroup.Key,
                    Posts = catGroup
                        .Select(x => x.Post)
                        .Distinct()
                        .GroupBy(p => p.CreatedDate!.Value.Year)
                        .OrderByDescending(g => g.Key)
                        .Select(yearGroup => new
                        {
                            Year = yearGroup.Key,
                            Posts = yearGroup
                                .OrderByDescending(p => p.CreatedDate)
                                .Select(p => new
                                {
                                    Id = p.Id,
                                    Title = p.Title,
                                    Created = p.CreatedDate!.Value.ToShortDateString()
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .Where(cat => cat.Posts.Any())
                .OrderBy(x => x.Category)
                .ToList();

            await Clients.Client(connectionId)
                .SendAsync("ReceivedBlogTree", tree);
        }

        private void UpdatePostFields(Post post, string title, string description, string content, DateTime date, bool isNew = false)
        {
            post.Title = title;
            post.Description = description;
            post.Content = content;

            if (isNew)
                post.CreatedDate = date;
            else
                post.UpdatedDate = date;
        }

        public async Task AutoSavePost(string connectionId, int id, string title, string description, string content, string category)
        {
            var now = DateTime.UtcNow;
            bool isNew = id == 0;

            try
            {
                if (isNew)
                {
                    // Create new draft post
                    var model = new PostViewModel
                    {
                        Title = title,
                        Description = description,
                        Content = content,
                        Category = category
                    };

                    var post = await _postService.CreateDraftPostAsync(model);
                    await Clients.Client(connectionId).SendAsync("PostCreated", post.Id, now);
                }
                else
                {
                    // Update existing post
                    var model = new PostViewModel
                    {
                        Title = title,
                        Description = description,
                        Content = content,
                        Category = category
                    };

                    await _postService.UpdatePostAsync(id, model);
                    await Clients.Client(connectionId).SendAsync("PostUpdated", now);
                }
            }
            catch (KeyNotFoundException)
            {
                // Post not found, ignore silently for autosave
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid breaking autosave
                // TODO: Add proper logging
            }
        }
        #endregion
    }
}
