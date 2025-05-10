using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;
using Web.Controllers;
using Web.Db;
using Web.Models;
using Web.Services;
using Web.ViewModels;
using Xunit;

namespace Tests.Controllers
{
    public class AlbumControllerTests
    {
        private static readonly Mock<IAlbumRepository> _albumRepoMock = new Mock<IAlbumRepository>();
        private static readonly Mock<ITechInfoRepository> _techInfoRepoMock = new Mock<ITechInfoRepository>(); 
        private static readonly Mock<IImageService> _imageServiceMock = new Mock<IImageService>();

        static AlbumControllerTests()
        {
            _albumRepoMock.Setup(x => x.Albums).Returns(TestData.GetAlbums().BuildMock());
            _techInfoRepoMock.Setup(x => x.TechInfos).Returns(new List<TechnicalInfo>().BuildMock());
        }

        #region GET /Index
        [Fact]
        public async Task GET_Index_Default()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AlbumViewModel>(viewResult.ViewData.Model);
            Assert.Equal(7, model.PageCount);
            Assert.Equal(15, model.Albums.Count());
        }

        [Fact]
        public async Task GET_Index_Page_0()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Index(0);
            var viewResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(viewResult.StatusCode, 400);
            Assert.Equal(viewResult.Value, "Page number should be positive");
        }

        [Fact]
        public async Task GET_Index_Page_1()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Index(1);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AlbumViewModel>(viewResult.ViewData.Model);
            Assert.Equal(7, model.PageCount);
            Assert.Equal(15, model.Albums.Count());
        }

        [Fact]
        public async Task GET_Index_Page_500()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Index(500);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AlbumViewModel>(viewResult.ViewData.Model);
            Assert.Empty(model.Albums);
        }

        [Fact]
        public async Task GET_Index_Pagination_From_1_To_100_Page()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            int albumIndex = 1;
            for (int i = 1; i <= 7; i++)
            {
                var result = await controller.Index(i);
                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<AlbumViewModel>(viewResult.ViewData.Model);
                foreach (var album in model.Albums)
                {
                    Assert.Equal("Artist 1", album.Artist.Data);
                    Assert.Equal($"Album {albumIndex++}", album.Data);
                }
            }            
        }
        #endregion
    }
}
