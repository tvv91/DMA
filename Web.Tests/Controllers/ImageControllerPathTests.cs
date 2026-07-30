using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Web.Tests.Helpers;

namespace Web.Tests.Controllers;

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
