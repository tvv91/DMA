using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using MediatR;
using Web.Enums;
using Web.Features.Albums;
using Web.Features.Equipment;
using Web.Interfaces;
using Web.Models;
using Web.Request;

namespace Web.SignalRHubs
{
    public class AlbumHub(
        IImageService imageService,
        IResourceIconService resourceIconService,
        ISender sender) : Hub
    {
        private readonly IImageService _imgService = imageService;
        private readonly IResourceIconService _resourceIconService = resourceIconService;
        private readonly ISender _sender = sender;
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
        public async Task GetAlbumCover(string connectionId, int albumId)
        {
            var imageUrl = await _imgService.GetUrlAsync(albumId, EntityType.AlbumCover);
            await Clients.Client(connectionId).SendAsync("ReceivedAlbumCoverDetailed", imageUrl);
        }

        public async Task CheckAlbum(string connectionId, int albumId, string album, string artist, string source)
        {
            var result = await _sender.Send(new CheckAlbumQuery(albumId, album, artist, source));
            await Clients.Client(connectionId).SendAsync("AlbumIsExist", result.Status, result.Id);
        }

        public async Task AddRelease(string connectionId, CreateUpdateReleaseRequest request)
        {
            try
            {
                var added = await _sender.Send(new AddReleaseCommand(request));
                await Clients.Client(connectionId).SendAsync("ReleaseAdded", added.Success, added.Error, added.AlbumId, added.Releases);
            }
            catch (Exception ex)
            {
                await Clients.Client(connectionId).SendAsync("ReleaseAdded", false, ex.Message, 0);
            }
        }

        public async Task UpdateRelease(string connectionId, CreateUpdateReleaseRequest request)
        {
            try
            {
                if (request.ReleaseId == 0)
                {
                    await Clients.Client(connectionId).SendAsync("ReleaseUpdated", false, "Release ID is required");
                    return;
                }

                var updated = await _sender.Send(new UpdateReleaseCommand(request));
                await Clients.Client(connectionId).SendAsync("ReleaseUpdated", updated.Success, updated.Error, updated.Releases);
            }
            catch (Exception ex)
            {
                await Clients.Client(connectionId).SendAsync("ReleaseUpdated", false, ex.Message);
            }
        }

        public async Task RemoveRelease(string connectionId, int releaseId)
        {
            try
            {
                var removed = await _sender.Send(new DeleteReleaseCommand(releaseId));
                await Clients.Client(connectionId).SendAsync("ReleaseRemoved", removed.Success, removed.Error, removed.Releases);
            }
            catch (Exception ex)
            {
                await Clients.Client(connectionId).SendAsync("ReleaseRemoved", false, ex.Message);
            }
        }

        public async Task GetManufacturer(string connectionId, string category, string value)
        {
            if (!_categoryEntityMap.TryGetValue(category, out var type))
            {
                await Clients.Client(connectionId).SendAsync("ReceivedManufacturer", category, string.Empty);
                return;
            }

            var result = await _sender.Send(new FindEquipmentManufacturerQuery(type, value)) ?? string.Empty;

            await Clients.Client(connectionId).SendAsync("ReceivedManufacturer", category, result);
        }

        public async Task GetTechnicalInfoIcons(string connectionId, int releaseId)
        {
            var technicalInfo = await _sender.Send(new GetTechnicalInfoIconsQuery(releaseId));

            if (technicalInfo is null)
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfo", null, null);
                return;
            }
            if (technicalInfo.Values.Values.All(x => x.Id is null))
            {
                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfo", null, null);
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

                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", category, url);
            }
        }

    }
}

