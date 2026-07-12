using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Controllers;

namespace Web.Tests.Helpers;

[CollectionDefinition("ImageController", DisableParallelization = true)]
public sealed class ImageControllerCollection;

internal sealed class ImageControllerTestEnvironment : IDisposable
{
    public const long MaxImageSizeBytes = 5 * 1024 * 1024;

    public string Root { get; }
    public string WebRoot { get; }
    public string TempDirectory { get; }
    public Mock<ILogger<ImageController>> LoggerMock { get; } = new();
    public ImageController Controller { get; }

    public ImageControllerTestEnvironment(string? webRootPath = null)
    {
        Root = Path.Combine(Path.GetTempPath(), "dma-image-controller-tests", Guid.NewGuid().ToString("N"));
        WebRoot = webRootPath ?? Path.Combine(Root, "wwwroot");
        TempDirectory = Path.Combine(WebRoot, "temp");
        Directory.CreateDirectory(TempDirectory);

        var environmentMock = new Mock<IWebHostEnvironment>();
        environmentMock.Setup(e => e.WebRootPath).Returns(webRootPath!);
        environmentMock.Setup(e => e.ContentRootPath).Returns(Root);

        Controller = new ImageController(environmentMock.Object, LoggerMock.Object);
    }

    public static byte[] PngSignatureBytes() =>
        [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00];

    public static byte[] JpegSignatureBytes() =>
        [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46];

    public Mock<IFormFile> CreateFileMock(
        byte[] content,
        string fileName,
        string contentType,
        long? lengthOverride = null)
    {
        var fileMock = new Mock<IFormFile>();
        var length = lengthOverride ?? content.Length;

        fileMock.Setup(f => f.Length).Returns(length);
        fileMock.Setup(f => f.ContentType).Returns(contentType);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(content));
        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((stream, _) => stream.Write(content, 0, content.Length))
            .Returns(Task.CompletedTask);

        return fileMock;
    }

    public string CreateTempImageFile(string filename, byte[]? content = null)
    {
        var path = Path.Combine(TempDirectory, filename);
        File.WriteAllBytes(path, content ?? PngSignatureBytes());
        return path;
    }

    public static string? InvokeGetSafeExtension(string fileName)
    {
        var method = typeof(ImageController).GetMethod(
            "GetSafeExtension",
            BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("GetSafeExtension not found.");

        return (string?)method.Invoke(null, [fileName]);
    }

    public static string? InvokeGetSafeUploadedFilename(string filename)
    {
        var method = typeof(ImageController).GetMethod(
            "GetSafeUploadedFilename",
            BindingFlags.Static | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException("GetSafeUploadedFilename not found.");

        return (string?)method.Invoke(null, [filename]);
    }

    public static string GetUploadedFilename(object jsonValue)
    {
        var property = jsonValue.GetType().GetProperty("Filename")
            ?? throw new InvalidOperationException("Filename property not found.");

        return (string)property.GetValue(jsonValue)!;
    }

    public void Dispose()
    {
        if (Directory.Exists(Root))
            Directory.Delete(Root, recursive: true);
    }
}
