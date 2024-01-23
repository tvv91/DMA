using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers;
using Web.Db;
using Web.Models;
using Xunit;

namespace Tests.Controllers
{
    public class HomeControllerTest
    {
        
        [Fact]
        public void Can_Use_Repository()
        {
            Mock<IAlbumRepository> mock = new Mock<IAlbumRepository>();
            mock.Setup(m => m.Albums).Returns(new TestData().GetData().AsQueryable());
            HomeController controller = new HomeController(mock.Object);
            controller.PageSize = 25;
            IEnumerable<Album>? result = (controller.Index() as ViewResult)?.ViewData.Model as IEnumerable<Album>;
            Album[] albums = result?.ToArray();
            Assert.True(albums.Length == 25);            
        }

        [Fact]
        public void Can_Paginate()
        {
            Mock<IAlbumRepository> mock = new Mock<IAlbumRepository>();
            mock.Setup(m => m.Albums).Returns(new TestData().GetData().AsQueryable());
            HomeController controller = new HomeController(mock.Object);
            controller.PageSize = 5;
            IEnumerable<Album>? result = (controller.Index() as ViewResult)?.ViewData.Model as IEnumerable<Album>;
            Album[] albums = result?.ToArray();
            Assert.True(albums.Length == 5);
            Assert.Equal("Album1", albums[0].Data);
            Assert.Equal("Album2", albums[1].Data);
            Assert.Equal("Album3", albums[2].Data);
            Assert.Equal("Album4", albums[3].Data);
            Assert.Equal("Album5", albums[4].Data);
        }
    }
}
