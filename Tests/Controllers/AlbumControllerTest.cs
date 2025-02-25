using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;
using Web.Controllers;
using Web.Db;
using Web.Models;
using Web.Request;
using Web.Services;
using Web.ViewModels;
using Xunit;

namespace Tests.Controllers
{
    public class AlbumControllerTest
    {
        private Mock<IAlbumRepository> _mockRepo;
        private Mock<IImageService> _mockImageService;
        private Mock<ITechInfoRepository> _mockTInfo;
        private AlbumController _controller;

        public AlbumControllerTest()
        {
            _mockRepo = new Mock<IAlbumRepository>();
            _mockImageService = new Mock<IImageService>();
            _mockRepo.Setup(m => m.Albums).Returns(new TestData().GetData().BuildMock());
            _mockTInfo = new Mock<ITechInfoRepository>();
            _controller = new AlbumController(_mockRepo.Object, _mockImageService.Object, _mockTInfo.Object);
        }

        /*
        [Fact]
        public async Task ShouldReturn15AlbumsPerPage()
        {
            IActionResult result = await _controller.Index();
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            AlbumViewModel model = Assert.IsType<AlbumViewModel>(viewResult.ViewData.Model);
            Assert.Equal(15, model.Albums.Count());          
        }
        */

        [Fact]
        public async Task AlbumsShoudContainsArtist ()
        {
            IActionResult result = await _controller.Index();
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            AlbumViewModel model = Assert.IsType<AlbumViewModel>(viewResult.ViewData.Model);
            var albums = model.Albums.ToList();
            foreach (var album in model.Albums)
            {
                Assert.NotNull(album.Artist);
            }
        }

        /*
        [Fact]
        public async Task ShouldReturn10AlbumsOnPage2()
        {
            IActionResult result = await _controller.Index(2);
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            AlbumViewModel model = Assert.IsType<AlbumViewModel>(viewResult.ViewData.Model);
            Assert.Equal(10, model.Albums.Count());
        }
        */

        #region Album creation
        // GET album/create
        // Just return New.cshtml view
        [Fact]
        public void GET_Create()
        {
            IActionResult result =  _controller.Create();
            ViewResult viewResult = result as ViewResult;
            Assert.NotNull(viewResult);
            Assert.Equal("CreateOrUpdate", viewResult.ViewName);
        }

        // If no data passed, return New view
        [Fact]
        public async Task POST_New_Album_With_Empty_Request()
        {
            IActionResult result = await _controller.NewAlbum(new AlbumDataRequest { });
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult);
            Assert.Equal("CreateOrUpdate", viewResult.ViewName);
        }

        // Should create album and redirect to album/{id} with id of created album
        [Fact]
        public async Task POST_New_Album_With_Request()
        {
            var request = new AlbumDataRequest
            {
                Artist = "Artist",
                Album = "Album",
                Genre = "Hard Rock",
                Year = 1991
            };

            _mockRepo.Setup(m => m.CreateOrUpdateAlbumAsync(request)).ReturnsAsync(new Album
            {
                Id = 123,
                Data = request.Album,
                Artist = new Artist { Data = request.Artist },
                Genre = new Genre { Data = request.Genre },
                Year = new Year { Data = request.Year.Value }
            });

            IActionResult result = await _controller.NewAlbum(request);
            _mockRepo.Verify(r => r.CreateOrUpdateAlbumAsync(request), Times.Once());
            var redirecResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("123", redirecResult.Url);
        }
        #endregion
    }
}
