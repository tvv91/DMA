using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Enums;
using Web.Services;

namespace Web.Tests.Helpers;

internal sealed class ImageTestEnvironment : IDisposable
{
    public static readonly EntityType[] MappedEntityTypes =
    [
        EntityType.AlbumCover,
        EntityType.Player,
        EntityType.Cartridge,
        EntityType.Amplifier,
        EntityType.Adc,
        EntityType.Wire,
    ];

    public static readonly EntityType[] UnmappedEntityTypes =
    [
        EntityType.VinylState,
        EntityType.DigitalFormat,
        EntityType.Artist,
        EntityType.Genre,
    ];

    public string Root { get; }
    public string WebRoot { get; }
    public Mock<ILogger<LocalStorageImageService>> LoggerMock { get; } = new();
    public LocalStorageImageService Service { get; }

    public ImageTestEnvironment()
    {
        Root = Path.Combine(Path.GetTempPath(), "dma-image-tests", Guid.NewGuid().ToString("N"));
        WebRoot = Path.Combine(Root, "wwwroot");
        Directory.CreateDirectory(Path.Combine(WebRoot, "temp"));

        var environmentMock = new Mock<IWebHostEnvironment>();
        environmentMock.Setup(e => e.WebRootPath).Returns(WebRoot);
        environmentMock.Setup(e => e.ContentRootPath).Returns(Root);

        Service = new LocalStorageImageService(environmentMock.Object, LoggerMock.Object);
    }

    public string GetCoverDirectory(EntityType type)
    {
        var folder = type switch
        {
            EntityType.AlbumCover => "covers/album",
            EntityType.Player => "covers/player",
            EntityType.Cartridge => "covers/cartridge",
            EntityType.Amplifier => "covers/amp",
            EntityType.Adc => "covers/adc",
            EntityType.Wire => "covers/wire",
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

        return Path.Combine(WebRoot, folder);
    }

    public string GetExpectedCoverUrl(EntityType type, int id)
    {
        var folder = type switch
        {
            EntityType.AlbumCover => "covers/album",
            EntityType.Player => "covers/player",
            EntityType.Cartridge => "covers/cartridge",
            EntityType.Amplifier => "covers/amp",
            EntityType.Adc => "covers/adc",
            EntityType.Wire => "covers/wire",
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

        return $"/{folder}/{id}.jpg";
    }

    public string GetCoverFilePath(EntityType type, int id) =>
        Path.Combine(GetCoverDirectory(type), $"{id}.jpg");

    public void CreateCoverFile(EntityType type, int id)
    {
        var directory = GetCoverDirectory(type);
        Directory.CreateDirectory(directory);
        File.WriteAllBytes(Path.Combine(directory, $"{id}.jpg"), [0xFF, 0xD8, 0xFF, 0xE0]);
    }

    public string CreateTempImageFile(string extension = ".jpg")
    {
        var filename = $"{Guid.NewGuid():N}{extension}";
        var path = Path.Combine(WebRoot, "temp", filename);
        File.WriteAllBytes(path, [0xFF, 0xD8, 0xFF, 0xE0]);
        return filename;
    }

    public string TempFilePath(string filename) =>
        Path.Combine(WebRoot, "temp", filename);

    public void Dispose()
    {
        if (Directory.Exists(Root))
            Directory.Delete(Root, recursive: true);
    }
}
