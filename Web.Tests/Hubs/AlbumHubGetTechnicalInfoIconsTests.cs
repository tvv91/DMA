using Moq;
using Web.Enums;
using Web.Models;
using Web.Tests.Helpers;

namespace Web.Tests.Hubs;

[Collection("AlbumHub")]
public class AlbumHubGetTechnicalInfoIconsTests
{
    private readonly AlbumHubTestFactory _factory = new();
    private const string ConnectionId = "conn-tech";

    [Fact]
    public async Task GetTechnicalInfoIcons_ReleaseNotFound_SendsNullPayload()
    {
        _factory.ReleaseServiceMock
            .Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync((Release?)null);

        var hub = _factory.CreateHub();
        await hub.GetTechnicalInfoIcons(ConnectionId, 1);

        var send = _factory.SendRecorder.FindSend("ReceivedTechnicalInfo");
        Assert.NotNull(send);
        Assert.Null(send.Value.Args[0]);
        Assert.Null(send.Value.Args[1]);
    }

    [Fact]
    public async Task GetTechnicalInfoIcons_NoTechnicalInfo_SendsNullPayload()
    {
        _factory.ReleaseServiceMock
            .Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(new Release
            {
                Id = 1,
                FormatInfo = new FormatInfo(),
                EquipmentInfo = new EquipmentInfo(),
            });

        var hub = _factory.CreateHub();
        await hub.GetTechnicalInfoIcons(ConnectionId, 1);

        var send = _factory.SendRecorder.FindSend("ReceivedTechnicalInfo");
        Assert.NotNull(send);
        Assert.Null(send.Value.Args[0]);
    }

    [Fact]
    public async Task GetTechnicalInfoIcons_WithFormatAndEquipment_SendsIconUrls()
    {
        var release = new Release
        {
            Id = 1,
            FormatInfo = new FormatInfo { VinylStateId = 1, BitnessId = 2 },
            EquipmentInfo = new EquipmentInfo { PlayerId = 3 },
        };

        _factory.ReleaseServiceMock
            .Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(release);
        _factory.ResourceIconServiceMock
            .Setup(s => s.GetIconUrlAsync(1, EntityType.VinylState))
            .ReturnsAsync("/resources/vinylstate/1.png");
        _factory.ResourceIconServiceMock
            .Setup(s => s.GetIconUrlAsync(2, EntityType.Bitness))
            .ReturnsAsync("/resources/bitness/2.png");
        _factory.ImageServiceMock
            .Setup(s => s.GetUrlAsync(3, EntityType.Player))
            .ReturnsAsync("/covers/player/3.jpg");

        var hub = _factory.CreateHub();
        await hub.GetTechnicalInfoIcons(ConnectionId, 1);

        var iconSends = _factory.SendRecorder.FindSends("ReceivedTechnicalInfoIcon");
        Assert.Equal(10, iconSends.Count);

        var vinyl = iconSends.Single(s => (string)s.Args[0]! == "vinylstate");
        Assert.Equal("/resources/vinylstate/1.png", vinyl.Args[1]);

        var player = iconSends.Single(s => (string)s.Args[0]! == "player");
        Assert.Equal("/covers/player/3.jpg", player.Args[1]);

        var digitalFormat = iconSends.Single(s => (string)s.Args[0]! == "digitalformat");
        Assert.Null(digitalFormat.Args[1]);
    }
}
