using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.Services;
using Web.ViewModels;

namespace Web.SignalRHubs
{
    public class DefaultHub : Hub
    {
        private readonly IImageService _imgService;
        private readonly IDigitizationRepository _techInfoRepository;
        private readonly IAlbumRepository _albumRepository;
        private readonly IPostRepository _postRepository;
        private readonly IEquipmentRepository _equipmentRepository;
        private static readonly ConcurrentDictionary<int, string> _coverCache = new();
        private const int ITEMS_PER_PAGE = 18;
        private const int POSTS_PER_PAGE = 10;

        public DefaultHub(
            IImageService coverImageService, 
            IDigitizationRepository techInfoRepository, 
            IAlbumRepository albumRepository, 
            IPostRepository postRepository,
            IEquipmentRepository equipmentRepository)
        {
            _imgService = coverImageService;
            _techInfoRepository = techInfoRepository;
            _albumRepository = albumRepository;
            _postRepository = postRepository;
            _equipmentRepository = equipmentRepository;
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
            // show random album cover on view
            Random.Shared.Shuffle(albums);
            const int chunkSize = 50;

            for (int i = 0; i < albums.Length; i += chunkSize)
            {
                var chunk = albums
                    .Skip(i)
                    .Take(chunkSize)
                    .Select(albumId => Clients.Client(connectionId)
                    .SendAsync("ReceivedAlbumCover", albumId, GetCachedAlbumCover(albumId)));

                await Task.WhenAll(chunk);
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

        public async Task GetTechnicalInfoIcons(string connectionId, int albumId)
        {
            var tInfo = await _techInfoRepository.TechInfos.FirstOrDefaultAsync(x => x.AlbumId == albumId);

            var mapping = new Dictionary<string, (int? id, EntityType type)>
            {
                ["vinylstate"] = (tInfo?.VinylStateId, EntityType.VinylState),
                ["digitalformat"] = (tInfo?.DigitalFormatId, EntityType.DigitalFormat),
                ["bitness"] = (tInfo?.BitnessId, EntityType.Bitness),
                ["sampling"] = (tInfo?.SamplingId, EntityType.Sampling),
                ["format"] = (tInfo?.SourceFormatId, EntityType.SourceFormat),
                ["player"] = (tInfo?.PlayerId, EntityType.Player),
                ["cartridge"] = (tInfo?.CartridgeId, EntityType.Cartridge),
                ["amp"] = (tInfo?.AmplifierId, EntityType.Amplifier),
                ["adc"] = (tInfo?.AdcId, EntityType.Adc),
                ["wire"] = (tInfo?.WireId, EntityType.Wire),
            };

            foreach (var kvp in mapping)
            {
                string? url = null;

                if (kvp.Value.id.HasValue)
                {
                    url = _imgService.GetImageUrl(kvp.Value.id.Value, kvp.Value.type);
                }

                await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", kvp.Key, url);
            }
        }

        #endregion

        #region Blog workload
        public async Task GetPosts(string connectionId, int page, string searchText, string category, string year, bool onlyDrafts)
        {
            var query = _postRepository.PostCategories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
                query = query.Where(x => x.Post.Title.Contains(searchText) || x.Post.Description.Contains(searchText) || x.Post.Content.Contains(searchText));

            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(x => x.Category.Title == category);

            if (!string.IsNullOrWhiteSpace(year) && int.TryParse(year, out var yearValue))
                query = query.Where(x => x.Post.CreatedDate.HasValue && x.Post.CreatedDate.Value.Year == yearValue);

            if (onlyDrafts)
                query = query.Where(x => x.Post.IsDraft == onlyDrafts);

            int totalItems = await query.CountAsync();
            int pageCount = totalItems % POSTS_PER_PAGE == 0 ? totalItems / POSTS_PER_PAGE : totalItems / POSTS_PER_PAGE + 1;

            var result = await query
                .OrderByDescending(x => x.Post.CreatedDate)
                .Skip((page - 1) * POSTS_PER_PAGE)
                .Take(POSTS_PER_PAGE)
                .Select(x => new
                {
                    x.Post,
                    Category = x.Category.Title,
                    Created = x.Post.CreatedDate.HasValue ? x.Post.CreatedDate.Value.ToShortDateString() : null
                })
                .ToListAsync();

            await Clients.Client(connectionId).SendAsync("ReceivedPosts", result, pageCount);
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

            Post? post;
            bool isNew = id == 0;

            if (isNew)
            {
                post = new Post { IsDraft = true };
                UpdatePostFields(post, title, description, content, now, isNew: true);
                var categoryEntity = await _postRepository.GetOrCreateCategory(category);
                var postCategory = new PostCategory { Post = post, Category = categoryEntity };
                await _postRepository.AddPostAsync(postCategory);
                await Clients.Client(connectionId).SendAsync("PostCreated", post.Id, now);
            }
            else
            {
                post = await _postRepository.Posts.FirstOrDefaultAsync(x => x.Id == id);
               
                if (post == null) 
                    return;

                await _postRepository.UpdatePostAsync(post, title, description, content, category);
                await Clients.Client(connectionId).SendAsync("PostUpdated", now);
            }
        }
        #endregion
    }
}
