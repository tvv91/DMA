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
