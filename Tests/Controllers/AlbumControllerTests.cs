using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;
using Web.Controllers;
using Web.Db;
using Web.Enums;
using Web.Models;
using Web.Services;
using Web.ViewModels;
using Xunit;

namespace Tests.Controllers
{
    public class AlbumControllerTests
    {
        private readonly Mock<IAlbumRepository> _albumRepoMock = new Mock<IAlbumRepository>();
        private readonly Mock<ITechInfoRepository> _techInfoRepoMock = new Mock<ITechInfoRepository>(); 
        private readonly Mock<IImageService> _imageServiceMock = new Mock<IImageService>();

        public AlbumControllerTests()
        {
            _albumRepoMock.Setup(x => x.Albums).Returns(new TestData().GetAlbums().BuildMock());
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

        #region GET album/id
        // should return album with info
        [Fact]
        public async Task GET_Album_1()
        {
            var album = _albumRepoMock.Object.Albums.FirstOrDefault(x => x.Id == 1);
            _albumRepoMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(album);
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.GetById(1);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AlbumDetailsViewModel>(viewResult.ViewData.Model);
            Assert.NotNull(model);
            Assert.Equal(1, model.Album.Id);
            Assert.Equal("Artist 1", model.Album.Artist.Data);
            Assert.Equal(2.4, model.Album.Size);
            Assert.Equal("https://somelink.com", model.Album.Source);
            Assert.Equal("https://somelink.com", model.Album.Discogs);
            Assert.Equal("Artist 1", model.Album.Artist.Data);
            Assert.Equal("Heavy Metal", model.Album.Genre?.Data);
            Assert.Equal(2005, model.Album.Year?.Data);
            Assert.Equal(2020, model.Album.Reissue?.Data);
            Assert.Equal("USA", model.Album.Country?.Data);
            Assert.Equal("Roadrunner Records", model.Album.Label?.Data);
            Assert.Equal("D1", model.Album.Storage?.Data);
            Assert.NotNull(model.Album.TechnicalInfo);
            Assert.Equal(24, model.Album.TechnicalInfo.Bitness?.Data);
            Assert.Equal("FLAC", model.Album.TechnicalInfo.DigitalFormat?.Data);
            Assert.Equal("LP 12'' 33RPM", model.Album.TechnicalInfo.SourceFormat?.Data);
            Assert.Equal(192, model.Album.TechnicalInfo.Sampling?.Data);
            Assert.Equal("Mint", model.Album.TechnicalInfo.VinylState?.Data);
        }

        // should return BadRequestResult if album/0
        [Fact]
        public async Task GET_Album_0()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.GetById(0);
            var viewResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, viewResult.StatusCode);
        }

        // should return NotFoundResult result
        [Fact]
        public async Task GET_Album_500()
        {
            var album = _albumRepoMock.Object.Albums.FirstOrDefault(x => x.Id == 500);
            _albumRepoMock.Setup(x => x.GetByIdAsync(500)).ReturnsAsync(album);
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.GetById(500);
            var viewResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, viewResult.StatusCode);
        }
        #endregion

        #region GET album/create
        // should return ViewResult
        [Fact]
        public void GET_Album_Create()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = controller.Create();
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AlbumCreateUpdateViewModel>(viewResult.ViewData.Model);
        }
        #endregion

        #region POST album/create
        
        // shouldn't create new album if not enter required data
        [Fact]
        public async Task POST_Album_Create_Model_Error()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            controller.ModelState.AddModelError("AlbumName", "Required");
            var result = await controller.Create(new AlbumCreateUpdateViewModel());
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AlbumCreateUpdateViewModel>(viewResult.ViewData.Model);
            Assert.Null(model.Adc);
            Assert.Null(model.AdcManufacturer);
            Assert.Null(model.Album);
            Assert.Null(model.AlbumCover);
            Assert.Equal(0, model.AlbumId);
            Assert.Null(model.Amplifier);
            Assert.Null(model.AmplifierManufacturer);
            Assert.Null(model.Artist);
            Assert.Null(model.Bitness);
            Assert.Null(model.Cartridge);
            Assert.Null(model.CartridgeManufacturer);
            Assert.Null(model.Country);
            Assert.Null(model.DigitalFormat);
            Assert.Null(model.Discogs);
            Assert.Null(model.Genre);
            Assert.Null(model.Label);
            Assert.Null(model.Player);
            Assert.Null(model.PlayerManufacturer);
            Assert.Null(model.Reissue);
            Assert.Null(model.Sampling);
            Assert.Null(model.Size);
            Assert.Null(model.Source);
            Assert.Null(model.SourceFormat);
            Assert.Null(model.Storage);
            Assert.Null(model.VinylState);
            Assert.Null(model.Wire);
            Assert.Null(model.WireManufacturer);
            Assert.Null(model.Year);
            Assert.Equal(ActionType.Create, model.Action);
        }

        // should create new album and redirect to it, image service shouldn't be called as no image
        [Fact]
        public async Task POST_Album_Create_No_Cover()
        {
            var album = Shared.GetFullAlbum();
            
            _albumRepoMock.Setup(x => x.CreateOrUpdateAlbumAsync(It.IsAny<AlbumCreateUpdateViewModel>())).Returns(Task.FromResult(album)).Verifiable();
            _techInfoRepoMock.Setup(x => x.CreateOrUpdateTechnicalInfoAsync(It.IsAny<Album>(), It.IsAny<AlbumCreateUpdateViewModel>())).Returns(Task.FromResult(album.TechnicalInfo)).Verifiable();
            _imageServiceMock.Setup(x => x.SaveCover(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>())).Verifiable();
            
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            
            var result = await controller.Create(new AlbumCreateUpdateViewModel
            {
                Album = album.Data,
                Artist = album.Artist.Data,
                Genre = album.Genre.Data,
                Year = album.Year.Data,
                Reissue = album.Reissue.Data,
                Country = album.Country.Data,
                Label = album.Label.Data,
                Source = album.Source,
                Size = album.Size,
                Storage = album.Storage.Data,
                Discogs = album.Discogs,
                Adc = album.TechnicalInfo.Adc.Data,
                AdcManufacturer = album.TechnicalInfo.Adc.Manufacturer.Data,
                Amplifier = album.TechnicalInfo.Amplifier.Data,
                AmplifierManufacturer = album.TechnicalInfo.Amplifier.Manufacturer.Data,
                Bitness = album.TechnicalInfo.Bitness.Data,
                Cartridge = album.TechnicalInfo.Cartridge.Data,
                CartridgeManufacturer = album.TechnicalInfo.Cartridge.Manufacturer.Data,
                DigitalFormat = album.TechnicalInfo.DigitalFormat.Data,
                Player = album.TechnicalInfo.Player.Data,
                PlayerManufacturer = album.TechnicalInfo.Player.Manufacturer.Data,
                SourceFormat = album.TechnicalInfo.SourceFormat.Data,
                Sampling = album.TechnicalInfo.Sampling.Data,
                VinylState = album.TechnicalInfo.VinylState.Data,
                Wire = album.TechnicalInfo.Wire.Data,
                WireManufacturer = album.TechnicalInfo.Wire.Manufacturer.Data
            });
            
            var viewResult = Assert.IsType<RedirectResult>(result);

            _albumRepoMock.Verify(x => x.CreateOrUpdateAlbumAsync(It.IsAny<AlbumCreateUpdateViewModel>()), Times.Once);
            _techInfoRepoMock.Verify(x => x.CreateOrUpdateTechnicalInfoAsync(It.IsAny<Album>(), It.IsAny<AlbumCreateUpdateViewModel>()), Times.Once);
            _imageServiceMock.Verify(x => x.SaveCover(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>()), Times.Never);
            
            Assert.Equal("123", viewResult.Url);
            
        }

        // should create new album and redirect to it, image service should be called
        [Fact]
        public async Task POST_Album_Create_With_Cover()
        {
            var album = Shared.GetFullAlbum();

            _albumRepoMock.Setup(x => x.CreateOrUpdateAlbumAsync(It.IsAny<AlbumCreateUpdateViewModel>())).Returns(Task.FromResult(album)).Verifiable();
            _techInfoRepoMock.Setup(x => x.CreateOrUpdateTechnicalInfoAsync(It.IsAny<Album>(), It.IsAny<AlbumCreateUpdateViewModel>())).Returns(Task.FromResult(album.TechnicalInfo)).Verifiable();
            _imageServiceMock.Setup(x => x.SaveCover(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>())).Verifiable();

            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);

            var result = await controller.Create(new AlbumCreateUpdateViewModel
            {
                AlbumCover = "file.jpg",
                Album = album.Data,
                Artist = album.Artist.Data,
                Genre = album.Genre.Data,
                Year = album.Year.Data,
                Reissue = album.Reissue.Data,
                Country = album.Country.Data,
                Label = album.Label.Data,
                Source = album.Source,
                Size = album.Size,
                Storage = album.Storage.Data,
                Discogs = album.Discogs,
                Adc = album.TechnicalInfo.Adc.Data,
                AdcManufacturer = album.TechnicalInfo.Adc.Manufacturer.Data,
                Amplifier = album.TechnicalInfo.Amplifier.Data,
                AmplifierManufacturer = album.TechnicalInfo.Amplifier.Manufacturer.Data,
                Bitness = album.TechnicalInfo.Bitness.Data,
                Cartridge = album.TechnicalInfo.Cartridge.Data,
                CartridgeManufacturer = album.TechnicalInfo.Cartridge.Manufacturer.Data,
                DigitalFormat = album.TechnicalInfo.DigitalFormat.Data,
                Player = album.TechnicalInfo.Player.Data,
                PlayerManufacturer = album.TechnicalInfo.Player.Manufacturer.Data,
                SourceFormat = album.TechnicalInfo.SourceFormat.Data,
                Sampling = album.TechnicalInfo.Sampling.Data,
                VinylState = album.TechnicalInfo.VinylState.Data,
                Wire = album.TechnicalInfo.Wire.Data,
                WireManufacturer = album.TechnicalInfo.Wire.Manufacturer.Data
            });

            var viewResult = Assert.IsType<RedirectResult>(result);

            _albumRepoMock.Verify(x => x.CreateOrUpdateAlbumAsync(It.IsAny<AlbumCreateUpdateViewModel>()), Times.Once);
            _techInfoRepoMock.Verify(x => x.CreateOrUpdateTechnicalInfoAsync(It.IsAny<Album>(), It.IsAny<AlbumCreateUpdateViewModel>()), Times.Once);
            _imageServiceMock.Verify(x => x.SaveCover(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>()), Times.Once);

            Assert.Equal("123", viewResult.Url);

        }
        #endregion
    }
}
