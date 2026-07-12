using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

[Collection("ImageController")]
public class ImageControllerUploadCoverTests : IDisposable
{
    private readonly ImageControllerTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Fact]
    public async Task UploadCover_InvalidModelState_ReturnsBadRequest()
    {
        _env.Controller.ModelState.AddModelError("file", "Invalid");

        var result = await _env.Controller.UploadCover(_env.CreateFileMock(
            ImageControllerTestEnvironment.PngSignatureBytes(),
            "test.png",
            "image/png").Object);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid image upload request.", badRequest.Value);
    }

    [Fact]
    public async Task UploadCover_NullFile_ReturnsBadRequest()
    {
        var result = await _env.Controller.UploadCover(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("No image file was provided.", badRequest.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task UploadCover_NonPositiveLength_ReturnsBadRequest(long length)
    {
        var file = _env.CreateFileMock(
            ImageControllerTestEnvironment.PngSignatureBytes(),
            "test.png",
            "image/png",
            length);

        var result = await _env.Controller.UploadCover(file.Object);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Image file size is invalid.", badRequest.Value);
    }

    [Fact]
    public async Task UploadCover_FileTooLarge_ReturnsBadRequest()
    {
        var file = _env.CreateFileMock(
            ImageControllerTestEnvironment.PngSignatureBytes(),
            "test.png",
            "image/png",
            ImageControllerTestEnvironment.MaxImageSizeBytes + 1);

        var result = await _env.Controller.UploadCover(file.Object);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Image file size is invalid.", badRequest.Value);
    }

    [Theory]
    [InlineData("image/gif")]
    [InlineData("application/pdf")]
    [InlineData("text/plain")]
    public async Task UploadCover_UnsupportedContentType_ReturnsBadRequest(string contentType)
    {
        var file = _env.CreateFileMock(
            ImageControllerTestEnvironment.PngSignatureBytes(),
            "test.png",
            contentType);

        var result = await _env.Controller.UploadCover(file.Object);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Only JPEG and PNG images are supported.", badRequest.Value);
    }

    [Theory]
    [InlineData("test.gif")]
    [InlineData("test.bmp")]
    [InlineData("test")]
    public async Task UploadCover_UnsupportedExtension_ReturnsBadRequest(string fileName)
    {
        var file = _env.CreateFileMock(
            ImageControllerTestEnvironment.PngSignatureBytes(),
            fileName,
            "image/png");

        var result = await _env.Controller.UploadCover(file.Object);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Only JPEG and PNG images are supported.", badRequest.Value);
    }

    [Fact]
    public async Task UploadCover_InvalidSignature_ReturnsBadRequest()
    {
        var file = _env.CreateFileMock(
            [0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77],
            "test.png",
            "image/png");

        var result = await _env.Controller.UploadCover(file.Object);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Image file content is invalid.", badRequest.Value);
    }

    [Fact]
    public async Task UploadCover_ValidPng_ReturnsJsonAndCreatesTempFile()
    {
        var file = _env.CreateFileMock(
            ImageControllerTestEnvironment.PngSignatureBytes(),
            "cover.png",
            "image/png");

        var result = await _env.Controller.UploadCover(file.Object);

        var json = Assert.IsType<JsonResult>(result);
        var filename = ImageControllerTestEnvironment.GetUploadedFilename(json.Value!);
        Assert.EndsWith(".png", filename);
        Assert.True(File.Exists(Path.Combine(_env.TempDirectory, filename)));
    }

    [Fact]
    public async Task UploadCover_ValidJpeg_ReturnsJsonWithJpgExtension()
    {
        var file = _env.CreateFileMock(
            ImageControllerTestEnvironment.JpegSignatureBytes(),
            "photo.jpeg",
            "image/jpeg");

        var result = await _env.Controller.UploadCover(file.Object);

        var json = Assert.IsType<JsonResult>(result);
        var filename = ImageControllerTestEnvironment.GetUploadedFilename(json.Value!);
        Assert.EndsWith(".jpg", filename);
        Assert.True(File.Exists(Path.Combine(_env.TempDirectory, filename)));
    }

    [Fact]
    public async Task UploadCover_ContentTypeCaseInsensitive_AcceptsUppercaseMimeType()
    {
        var file = _env.CreateFileMock(
            ImageControllerTestEnvironment.PngSignatureBytes(),
            "cover.png",
            "IMAGE/PNG");

        var result = await _env.Controller.UploadCover(file.Object);

        Assert.IsType<JsonResult>(result);
    }
}

[Collection("ImageController")]
public class ImageControllerDeleteTempImageTests : IDisposable
{
    private readonly ImageControllerTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void DeleteTempImage_NullOrWhitespaceFilename_ReturnsBadRequest(string? filename)
    {
        var result = _env.Controller.DeleteTempImage(filename!);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid image filename.", badRequest.Value);
    }

    [Theory]
    [InlineData("../secret.png")]
    [InlineData("folder/image.png")]
    [InlineData("not-a-guid.png")]
    [InlineData("ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ.jpg")]
    public void DeleteTempImage_InvalidFilename_ReturnsBadRequest(string filename)
    {
        var result = _env.Controller.DeleteTempImage(filename);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid image filename.", badRequest.Value);
    }

    [Fact]
    public void DeleteTempImage_ExistingFile_ReturnsOkAndDeletesFile()
    {
        var filename = $"{Guid.NewGuid():N}.png";
        var path = _env.CreateTempImageFile(filename);
        Assert.True(File.Exists(path));

        var result = _env.Controller.DeleteTempImage(filename);

        Assert.IsType<OkResult>(result);
        Assert.False(File.Exists(path));
    }

    [Fact]
    public void DeleteTempImage_MissingFile_ReturnsOk()
    {
        var filename = $"{Guid.NewGuid():N}.jpg";

        var result = _env.Controller.DeleteTempImage(filename);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public void DeleteTempImage_ValidJpegFilename_ReturnsOk()
    {
        var filename = $"{Guid.NewGuid():N}.jpg";
        var path = _env.CreateTempImageFile(filename, ImageControllerTestEnvironment.JpegSignatureBytes());

        var result = _env.Controller.DeleteTempImage(filename);

        Assert.IsType<OkResult>(result);
        Assert.False(File.Exists(path));
    }
}

public class ImageControllerFilenameValidationTests
{
    [Theory]
    [InlineData("photo.png", ".png")]
    [InlineData("photo.jpg", ".jpg")]
    [InlineData("photo.jpeg", ".jpg")]
    [InlineData("photo.JPEG", ".jpg")]
    public void GetSafeExtension_SupportedExtensions_ReturnNormalizedExtension(string fileName, string expected)
    {
        var extension = ImageControllerTestEnvironment.InvokeGetSafeExtension(fileName);

        Assert.Equal(expected, extension);
    }

    [Theory]
    [InlineData("photo.gif")]
    [InlineData("photo")]
    public void GetSafeExtension_UnsupportedExtensions_ReturnsNull(string fileName)
    {
        var extension = ImageControllerTestEnvironment.InvokeGetSafeExtension(fileName);

        Assert.Null(extension);
    }

    [Fact]
    public void GetSafeUploadedFilename_ValidGuidFilename_ReturnsFilename()
    {
        var filename = $"{Guid.NewGuid():N}.png";

        var result = ImageControllerTestEnvironment.InvokeGetSafeUploadedFilename(filename);

        Assert.Equal(filename, result);
    }

    [Fact]
    public void GetSafeUploadedFilename_PathTraversalAttempt_ReturnsNull()
    {
        var result = ImageControllerTestEnvironment.InvokeGetSafeUploadedFilename("../temp/evil.png");

        Assert.Null(result);
    }
}

[Collection("ImageController")]
public class ImageControllerPathTests : IDisposable
{
    private readonly ImageControllerTestEnvironment _env = new();

    public void Dispose() => _env.Dispose();

    [Fact]
    public void GetSafeTempDirectory_UsesWebRootTempFolder()
    {
        var method = typeof(Web.Controllers.ImageController).GetMethod(
            "GetSafeTempDirectory",
            BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException();

        var directory = (string)method.Invoke(_env.Controller, null)!;

        Assert.Equal(Path.GetFullPath(_env.TempDirectory), directory);
    }

    [Fact]
    public void GetSafeTempDirectory_NullWebRoot_UsesContentRootWwwroot()
    {
        using var env = new ImageControllerTestEnvironment(webRootPath: null);

        var method = typeof(Web.Controllers.ImageController).GetMethod(
            "GetSafeTempDirectory",
            BindingFlags.Instance | BindingFlags.NonPublic)
            ?? throw new InvalidOperationException();

        var directory = (string)method.Invoke(env.Controller, null)!;

        Assert.Equal(Path.GetFullPath(env.TempDirectory), directory);
    }
}
