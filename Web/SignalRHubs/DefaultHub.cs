using Microsoft.AspNetCore.SignalR;
using Web.Services;

namespace Web.SignalRHubs
{
    public class DefaultHub : Hub
    {
        private readonly ICoverImageService _coverImageService;

        public DefaultHub(ICoverImageService coverImageService)
        {
            _coverImageService = coverImageService;
        }

        public async Task GetAlbumCovers(string connectionId, int[] albums)
        {
            // otherwise album covers will load sequentially
            Random.Shared.Shuffle(albums);
            foreach (var id in albums)
            { 
                await Clients.Client(connectionId).SendAsync("ReceivedAlbumConver", id, _coverImageService.GetImageUrl(id));
                Thread.Sleep(100);
            }
        }
    }
}
