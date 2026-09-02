using Moq;
using Web.Common;
using Web.Interfaces;
using Web.Tests.Helpers;
using Web.ViewModels;

namespace Web.Tests.Hubs;

public class EquipmentHubGetHardwareByCategoryTests
{
    private readonly EquipmentHubTestFactory _factory = new();
    private const string ConnectionId = "conn-hardware";

    [Fact]
    public async Task GetHardwareByCategory_UnknownCategory_DoesNotSendAnything()
    {
        var hub = _factory.CreateHub();
        await hub.GetHardwareByCategory(ConnectionId, "unknown", 1);

        Assert.Empty(_factory.SendRecorder.Sends);
        _factory.EquipmentServiceMock.Verify(
            s => s.GetListAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<EntityType>()),
            Times.Never);
    }

    [Fact]
    public async Task GetHardwareByCategory_ValidCategory_SendsItemsAndPageCount()
    {
        var items = new List<IManufacturer>
        {
            new Player
            {
                Id = 1,
                Name = "SL-1200",
                Manufacturer = new Manufacturer { Name = "Technics" },
            },
            new Player { Id = 2, Name = "PL-12D", Manufacturer = null },
        };
        var paged = new PagedResult<IManufacturer>(items, totalItems: 20, currentPage: 1, pageSize: 18);

        _factory.EquipmentServiceMock
            .Setup(s => s.GetListAsync(1, 18, EntityType.Player))
            .ReturnsAsync(paged);
        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(It.IsAny<int>(), EntityType.Player))
            .ReturnsAsync("/covers/player/1.jpg");

        var hub = _factory.CreateHub();
        await hub.GetHardwareByCategory(ConnectionId, "player", 1);

        var itemsSend = _factory.SendRecorder.FindSend("ReceivedItems");
        Assert.NotNull(itemsSend);
        var viewModels = Assert.IsType<List<EquipmentViewModel>>(itemsSend.Value.Args[0]);
        Assert.Equal(2, viewModels.Count);
        Assert.Equal("SL-1200", viewModels[0].ModelName);
        Assert.Equal("Technics", viewModels[0].Manufacturer);
        Assert.Equal(EntityType.Player, viewModels[0].EquipmentType);
        Assert.Equal("—", viewModels[1].Manufacturer);

        var countSend = _factory.SendRecorder.FindSend("ReceivedItemsCount");
        Assert.NotNull(countSend);
        Assert.Equal(2, countSend.Value.Args[0]);
    }

    [Fact]
    public async Task GetHardwareByCategory_EmptyResult_SendsEmptyListAndZeroPages()
    {
        var paged = new PagedResult<IManufacturer>([], totalItems: 0, currentPage: 1, pageSize: 18);

        _factory.EquipmentServiceMock
            .Setup(s => s.GetListAsync(2, 18, EntityType.Wire))
            .ReturnsAsync(paged);

        var hub = _factory.CreateHub();
        await hub.GetHardwareByCategory(ConnectionId, "wire", 2);

        var itemsSend = _factory.SendRecorder.FindSend("ReceivedItems");
        Assert.NotNull(itemsSend);
        Assert.Empty(Assert.IsType<List<EquipmentViewModel>>(itemsSend.Value.Args[0]));

        var countSend = _factory.SendRecorder.FindSend("ReceivedItemsCount");
        Assert.NotNull(countSend);
        Assert.Equal(0, countSend.Value.Args[0]);
        Assert.Empty(_factory.SendRecorder.FindSends("ReceivedItemImage"));
    }

    [Fact]
    public async Task GetHardwareByCategory_SendsImageForEachItem()
    {
        var items = new List<IManufacturer>
        {
            new Adc { Id = 10, Name = "ADC-1" },
            new Adc { Id = 11, Name = "ADC-2" },
        };
        var paged = new PagedResult<IManufacturer>(items, totalItems: 2, currentPage: 1, pageSize: 18);

        _factory.EquipmentServiceMock
            .Setup(s => s.GetListAsync(1, 18, EntityType.Adc))
            .ReturnsAsync(paged);
        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(10, EntityType.Adc))
            .ReturnsAsync("/covers/adc/10.jpg");
        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(11, EntityType.Adc))
            .ReturnsAsync("/covers/adc/11.jpg");

        var hub = _factory.CreateHub();
        await hub.GetHardwareByCategory(ConnectionId, "adc", 1);

        var imageSends = _factory.SendRecorder.FindSends("ReceivedItemImage");
        Assert.Equal(2, imageSends.Count);
        Assert.Contains(imageSends, s => (int)s.Args[0]! == 10 && (string)s.Args[1]! == "/covers/adc/10.jpg");
        Assert.Contains(imageSends, s => (int)s.Args[0]! == 11 && (string)s.Args[1]! == "/covers/adc/11.jpg");
    }
}
