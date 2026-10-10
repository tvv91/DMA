using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Web.Authorization;
using Web.Enums;
using Web.Features.Albums;
using Web.Features.Equipment;
using Web.Features.Albums.Releases;
using Web.Infrastructure.Icons;
using Web.Infrastructure.Storage;
using Microsoft.Extensions.Logging;

namespace Web.Hubs
{
    public class AlbumHub(
        IImageService imageService,
        IResourceIconService resourceIconService,
        ISender sender,
        ILogger<AlbumHub> logger) : Hub
    {
        private readonly IImageService _imgService = imageService;
        private readonly IResourceIconService _resourceIconService = resourceIconService;
        private readonly ISender _sender = sender;
        private readonly ILogger<AlbumHub> _logger = logger;
        private static readonly ConcurrentDictionary<int, string> _coverCache = new();

        private readonly Dictionary<string, EntityType> _categoryEntityMap = new()
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
        public async Task GetAlbumCovers(int[] albums)
        {
            Random.Shared.Shuffle(albums);

            foreach (var albumId in albums)
            {
                var cover = await GetCachedAlbumCoverAsync(albumId);
                await Clients.Caller.SendAsync("ReceivedAlbumCover", albumId, cover);
            }
        }

        private async Task<string> GetCachedAlbumCoverAsync(int albumId)
        {
            if (_coverCache.TryGetValue(albumId, out var cachedCover))
                return cachedCover;

            var cover = await _imgService.GetUrlAsync(albumId, EntityType.AlbumCover);
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
        public async Task GetAlbumCover(int albumId)
        {
            var imageUrl = await _imgService.GetUrlAsync(albumId, EntityType.AlbumCover);
            await Clients.Caller.SendAsync("ReceivedAlbumCoverDetailed", imageUrl);
        }

        public async Task CheckAlbum(int albumId, string album, string artist, string source)
        {
            var result = await _sender.Send(new CheckAlbumQuery(albumId, album, artist, source));
            await Clients.Caller.SendAsync("AlbumIsExist", result.Status, result.Id);
        }

        [Authorize(Roles = RoleNames.Admin)]
        public async Task AddRelease(AddReleaseRequest request)
        {
            try
            {
                var added = await _sender.Send(new AddReleaseCommand(request));
                await Clients.Caller.SendAsync("ReleaseAdded", added.Success, added.Error, added.AlbumId, added.Releases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add release");
                await Clients.Caller.SendAsync("ReleaseAdded", false, "Failed to add release.", 0);
            }
        }

        [Authorize(Roles = RoleNames.Admin)]
        public async Task UpdateRelease(UpdateReleaseRequest request)
        {
            try
            {
                if (request.ReleaseId == 0)
                {
                    await Clients.Caller.SendAsync("ReleaseUpdated", false, "Release ID is required");
                    return;
                }

                var updated = await _sender.Send(new UpdateReleaseCommand(request));
                await Clients.Caller.SendAsync("ReleaseUpdated", updated.Success, updated.Error, updated.Releases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update release {ReleaseId}", request.ReleaseId);
                await Clients.Caller.SendAsync("ReleaseUpdated", false, "Failed to update release.");
            }
        }

        [Authorize(Roles = RoleNames.Admin)]
        public async Task RemoveRelease(int releaseId)
        {
            try
            {
                var removed = await _sender.Send(new DeleteReleaseCommand(releaseId));
                await Clients.Caller.SendAsync("ReleaseRemoved", removed.Success, removed.Error, removed.Releases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove release {ReleaseId}", releaseId);
                await Clients.Caller.SendAsync("ReleaseRemoved", false, "Failed to remove release.");
            }
        }

        public async Task GetManufacturer(string category, string value)
        {
            if (!_categoryEntityMap.TryGetValue(category, out var type))
            {
                await Clients.Caller.SendAsync("ReceivedManufacturer", category, string.Empty);
                return;
            }

            var result = await _sender.Send(new FindEquipmentManufacturerQuery(type, value)) ?? string.Empty;

            await Clients.Caller.SendAsync("ReceivedManufacturer", category, result);
        }

        public async Task GetTechnicalInfoIcons(int releaseId)
        {
            var technicalInfo = await _sender.Send(new GetTechnicalInfoIconsQuery(releaseId));

            if (technicalInfo is null)
            {
                await Clients.Caller.SendAsync("ReceivedTechnicalInfo", null, null);
                return;
            }
            if (technicalInfo.Values.Values.All(x => x.Id is null))
            {
                await Clients.Caller.SendAsync("ReceivedTechnicalInfo", null, null);
                return;
            }
            foreach (var kvp in technicalInfo.Values)
            {
                string category = kvp.Key;
                string? url = null;

                if (kvp.Value.Id.HasValue)
                {
                    url = kvp.Value.Resource
                        ? await _resourceIconService.GetIconUrlAsync(kvp.Value.Id.Value, kvp.Value.Type)
                        : await _imgService.GetUrlAsync(kvp.Value.Id.Value, kvp.Value.Type);
                }

                await Clients.Caller.SendAsync("ReceivedTechnicalInfoIcon", category, url);
            }
        }

    }
}

