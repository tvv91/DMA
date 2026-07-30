using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

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
