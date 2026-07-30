using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

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
