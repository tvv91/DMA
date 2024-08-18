using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Web.Db;
using Web.Enum;
using Web.Models;
using Web.Services;

namespace Web.SignalRHubs
{
    public class DefaultHub : Hub
    {
        private readonly IImageService _coverImageService;
        private readonly ITechInfoRepository _techInfoRepository;

        public DefaultHub(IImageService coverImageService, ITechInfoRepository techInfoRepository)
        {
            _coverImageService = coverImageService;
            _techInfoRepository = techInfoRepository;
        }

        public async Task GetAlbumCovers(string connectionId, int[] albums)
        {
            // otherwise album covers will load sequentially
            Random.Shared.Shuffle(albums);
            foreach (var id in albums)
            { 
                await Clients.Client(connectionId).SendAsync("ReceivedAlbumConver", id, _coverImageService.GetImageUrl(id, EntityType.AlbumCover));
                Thread.Sleep(100);
            }
        }

        public async Task GetAlbumCover(string connectionId, int albumId)
        {
            await Clients.Client(connectionId).SendAsync("ReceivedAlbumConverDetailed", _coverImageService.GetImageUrl(albumId, EntityType.AlbumDetailCover));
        }

        public async Task GetTechnicalInfoIcons(string connectionId, int albumId)
        {
            TechnicalInfo ti = await _techInfoRepository.TechInfos.FirstOrDefaultAsync(x => x.AlbumId == albumId);
            if (ti == null)
            {
                await Clients.Client(connectionId).SendAsync("TechnicalInfoNotFound");
            } 
            else
            {
                await Clients.Client(connectionId).SendAsync("TechnicalInfoFound");
                if (ti.StateId != null)
                {
                    State state = await _techInfoRepository.States.FirstOrDefaultAsync(x => x.Id == ti.StateId);
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "vinylstate", _coverImageService.GetImageUrl(state.Id, EntityType.VinylState));
                }
                if (ti.CodecId != null)
                {
                    Codec codec = await _techInfoRepository.Codecs.FirstOrDefaultAsync(x => x.Id == ti.CodecId);
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "digitalformat", _coverImageService.GetImageUrl(codec.Id, EntityType.DigitalFormat));
                }
                
                if (ti.BitnessId != null)
                {
                    Bitness bitness = await _techInfoRepository.Bitnesses.FirstOrDefaultAsync(x => x.Id == ti.BitnessId);
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "bitness", _coverImageService.GetImageUrl(bitness.Id, EntityType.Bitness));
                }
                if (ti.SamplingId != null)
                {
                    Sampling sampling = await _techInfoRepository.Samplings.FirstOrDefaultAsync(x => x.Id == ti.SamplingId);
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "sampling", _coverImageService.GetImageUrl(sampling.Id, EntityType.Sampling));
                }
                if (ti.FormatId != null)
                {
                    Format format = await _techInfoRepository.Formats.FirstOrDefaultAsync(x => x.Id == ti.FormatId);
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "format", _coverImageService.GetImageUrl(format.Id, EntityType.SourceFormat));
                }
                if (ti.DeviceId != null)
                {
                    Device device = await _techInfoRepository.Devices.FirstOrDefaultAsync(x => x.Id == ti.DeviceId);
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "device", _coverImageService.GetImageUrl(device.Id, EntityType.Device));
                }
                if (ti.CartrigeId != null)
                {
                    Cartrige cartrige= await _techInfoRepository.Cartriges.FirstOrDefaultAsync(x => x.Id == ti.CartrigeId);
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "cartridge", _coverImageService.GetImageUrl(cartrige.Id, EntityType.Cartridge));
                }
                if (ti.AmplifierId != null)
                {
                    Amplifier amplifier = await _techInfoRepository.Amplifiers.FirstOrDefaultAsync(x => x.Id == ti.AmplifierId);
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "amp", _coverImageService.GetImageUrl(amplifier.Id, EntityType.Amp));
                }
                if (ti.AdcId != null)
                {
                    Adc adc = await _techInfoRepository.Adcs.FirstOrDefaultAsync(x => x.Id == ti.AdcId);
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "adc", _coverImageService.GetImageUrl(adc.Id, EntityType.Adc));
                }
                if (ti.ProcessingId != null)
                {
                    Processing processing= await _techInfoRepository.Processings.FirstOrDefaultAsync(x => x.Id == ti.ProcessingId);
                    await Clients.Client(connectionId).SendAsync("ReceivedTechnicalInfoIcon", "processing", _coverImageService.GetImageUrl(processing.Id, EntityType.Processing));
                }
            }
        }
    }
}
