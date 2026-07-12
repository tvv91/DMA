using Web.Enums;
using Web.Services;

namespace Web.Tests.Helpers;

[CollectionDefinition("LocalResourceIconService", DisableParallelization = true)]
public sealed class LocalResourceIconServiceCollection;

internal sealed class ResourceIconTestEnvironment : IDisposable
{
    public static readonly EntityType[] MappedEntityTypes =
    [
        EntityType.VinylState,
        EntityType.DigitalFormat,
        EntityType.Bitness,
        EntityType.Sampling,
        EntityType.SourceFormat,
    ];

    public static readonly EntityType[] UnmappedEntityTypes =
    [
        EntityType.Player,
        EntityType.Adc,
        EntityType.AlbumCover,
        EntityType.Artist,
        EntityType.Country,
    ];

    private readonly string _previousWorkingDirectory;

    public string Root { get; }
    public LocalResourceIconService Service { get; } = new();

    public ResourceIconTestEnvironment()
    {
        _previousWorkingDirectory = Directory.GetCurrentDirectory();
        Root = Path.Combine(Path.GetTempPath(), "dma-resource-icon-tests", Guid.NewGuid().ToString("N"));

        foreach (var type in MappedEntityTypes)
            Directory.CreateDirectory(GetResourceDirectory(type));

        Directory.SetCurrentDirectory(Root);
    }

    public string GetResourceDirectory(EntityType type)
    {
        var folder = type switch
        {
            EntityType.VinylState => "vinylstate",
            EntityType.DigitalFormat => "codec",
            EntityType.Bitness => "bitness",
            EntityType.Sampling => "sampling",
            EntityType.SourceFormat => "sourceformat",
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

        return Path.Combine(Root, "wwwroot", "resources", folder);
    }

    public string GetExpectedUrl(EntityType type, int id)
    {
        var folder = type switch
        {
            EntityType.VinylState => "resources/vinylstate",
            EntityType.DigitalFormat => "resources/codec",
            EntityType.Bitness => "resources/bitness",
            EntityType.Sampling => "resources/sampling",
            EntityType.SourceFormat => "resources/sourceformat",
            _ => throw new ArgumentOutOfRangeException(nameof(type)),
        };

        return $"/{folder}/{id}.png";
    }

    public void CreateIconFile(EntityType type, int id)
    {
        var directory = GetResourceDirectory(type);
        Directory.CreateDirectory(directory);
        File.WriteAllBytes(Path.Combine(directory, $"{id}.png"), [0x89, 0x50, 0x4E, 0x47]);
    }

    public void Dispose()
    {
        Directory.SetCurrentDirectory(_previousWorkingDirectory);

        if (Directory.Exists(Root))
            Directory.Delete(Root, recursive: true);
    }
}
