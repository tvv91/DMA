using Moq;
using Web.Controllers;
using Web.Enums;
using Web.Interfaces;

namespace Web.Tests.Helpers;

internal sealed class AlbumControllerTestFactory
{
    public Mock<IAlbumService> AlbumServiceMock { get; } = new();
    public Mock<IImageService> ImageServiceMock { get; } = new();

    public AlbumController CreateController() =>
        new(AlbumServiceMock.Object, ImageServiceMock.Object);
}
