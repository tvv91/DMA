using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;
using Web.Controllers;
using Web.Db;
using Web.Models;
using Web.ViewModels;
using Xunit;

namespace Tests.Controllers
{
    public class AlbumControllerTest
    {
        private Mock<IAlbumRepository> _mock;
        private AlbumController _controller;
        
        public AlbumControllerTest()
        {
            _mock = new Mock<IAlbumRepository>();
            _mock.Setup(m => m.Albums).Returns(new TestData().GetData().BuildMock());
            _controller = new AlbumController(_mock.Object);
        }

        [Fact]
        public async Task ShouldReturn15AlbumsPerPage()
        {
            IActionResult result = await _controller.Index();
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            AlbumViewModel model = Assert.IsType<AlbumViewModel>(viewResult.ViewData.Model);
            Assert.Equal(15, model.Albums.Count());          
        }

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

        [Fact]
        public async Task ShouldReturn10AlbumsOnPage2()
        {
            IActionResult result = await _controller.Index(2);
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            AlbumViewModel model = Assert.IsType<AlbumViewModel>(viewResult.ViewData.Model);
            Assert.Equal(10, model.Albums.Count());
        }

        #region Create album
        [Fact]
        public async Task CreateNewAlbum()
        {
            IActionResult result = await _controller.Create();
            ViewResult viewResult = Assert.IsType<ViewResult>(result);
            AlbumViewModel model = Assert.IsType<AlbumViewModel>(viewResult.ViewData.Model);
            //Assert.Equal(10, model.Albums.Count());
        }
        #endregion
    }
}
