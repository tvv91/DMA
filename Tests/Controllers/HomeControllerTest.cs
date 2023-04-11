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
            Mock<IDbRepository> mock = new Mock<IDbRepository>();
            mock.Setup(m => m.Albums).Returns(new TestData().GetData().AsQueryable());
            HomeController controller = new HomeController(mock.Object);
            IActionResult result = controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IQueryable<Album>>(viewResult.Model);
            Assert.Equal(25, model.Count());
        }

    }
}
