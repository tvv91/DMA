using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using Moq;
using Web.Controllers;
using Web.Db;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.Services;
using Web.ViewModels;
using Xunit;

namespace Tests.Controllers
{
    public class AlbumControllerTests
    {
        private readonly Mock<IAlbumRepository> _albumRepoMock = new Mock<IAlbumRepository>();
        private readonly Mock<IDigitizationRepository> _techInfoRepoMock = new Mock<IDigitizationRepository>();
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
            var model = Assert.IsAssignableFrom<AlbumIndexViewModel>(viewResult.ViewData.Model);
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
            var model = Assert.IsAssignableFrom<AlbumIndexViewModel>(viewResult.ViewData.Model);
            Assert.Equal(7, model.PageCount);
            Assert.Equal(15, model.Albums.Count());
        }

        [Fact]
        public async Task GET_Index_Page_500()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Index(500);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AlbumIndexViewModel>(viewResult.ViewData.Model);
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
                var model = Assert.IsAssignableFrom<AlbumIndexViewModel>(viewResult.ViewData.Model);
                foreach (var album in model.Albums)
                {
                    Assert.Equal("Artist 1", album.Artist.Data);
                    Assert.Equal($"Album {albumIndex++}", album.Title);
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
            Assert.Equal("Heavy Metal", model.Album.Genre?.Name);
            Assert.Equal(2005, model.Album.Year?.YearValue);
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
            Assert.Null(model.Title);
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
                Title = album.Title,
                Artist = album.Artist.Data,
                Genre = album.Genre.Name,
                Year = album.Year.YearValue,
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
                Title = album.Title,
                Artist = album.Artist.Data,
                Genre = album.Genre.Name,
                Year = album.Year.YearValue,
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

        #region GET album/edit/{albumId}

        // should return BadRequest
        [Fact]
        public async Task GET_Album_Edit_Invalid_Album_Id()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Edit(0);
            var actionResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
        }

        // should return NotFoundResult
        [Fact]
        public async Task GET_Album_Edit_Album_Not_Found()
        {
            // suggest album with this id not found in db
            _albumRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult<Album>(null));

            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Edit(1);
            var actionResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, actionResult.StatusCode);

        }

        // should return CreateOrUpdate view with AlbumCreateUpdateViewModel
        [Fact]
        public async Task GET_Album_Edit_Album_Found()
        {
            var album = Shared.GetFullAlbum();

            _albumRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(album)).Verifiable();
            _techInfoRepoMock.Setup(x => x.GetByIdAsync(It.IsAny<int>())).Returns(Task.FromResult(album.TechnicalInfo)).Verifiable();
            _imageServiceMock.Setup(x => x.GetImageUrl(It.IsAny<int>(), EntityType.AlbumCover)).Returns("cover.jpg").Verifiable();

            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Edit(1);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AlbumCreateUpdateViewModel>(viewResult.ViewData.Model);
            
            Assert.Equal(123, model.AlbumId);
            Assert.Equal("Some album", model.Title);
            Assert.Equal(5.4, model.Size);
            Assert.Equal("https://somesource.com", model.Source);
            Assert.Equal("https://discogs.com", model.Discogs);
            Assert.Equal("Some artist", model.Artist);
            Assert.Equal("Heavy Metal", model.Genre);
            Assert.Equal(2010, model.Year);
            Assert.Equal(2020, model.Reissue);
            Assert.Equal("USA", model.Country);
            Assert.Equal("Some label", model.Label);
            Assert.Equal("Some storage", model.Storage);
            Assert.Equal("Some Adc Model", model.Adc);
            Assert.Equal("Some Adc Manufacturer", model.AdcManufacturer);
            Assert.Equal("Some Amplifier Model", model.Amplifier);
            Assert.Equal("Some Amplifier Manufacturer", model.AmplifierManufacturer);
            Assert.Equal("Some Cartridge Model", model.Cartridge);
            Assert.Equal("Some Cartridge Manufacturer", model.CartridgeManufacturer);
            Assert.Equal("Some Player Model", model.Player);
            Assert.Equal("Some Player Manufacturer", model.PlayerManufacturer);
            Assert.Equal("Some Wire Model", model.Wire);
            Assert.Equal("Some Wire Manufacturer", model.WireManufacturer);
            Assert.Equal("LP 12'' 33RPM", model.SourceFormat);
            Assert.Equal(192, model.Sampling);
            Assert.Equal("Mint", model.VinylState);
            Assert.Equal(24, model.Bitness);
            Assert.Equal("FLAC", model.DigitalFormat);
            Assert.Equal("cover.jpg", model.AlbumCover);

            _albumRepoMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _techInfoRepoMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Once);
            _imageServiceMock.Verify(x => x.GetImageUrl(It.IsAny<int>(), EntityType.AlbumCover), Times.Once);

        }
        #endregion

        #region POST album/update
        
        // should return bad request
        [Fact]
        public async Task POST_Album_Update_Empty_Request()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Update(new AlbumCreateUpdateViewModel { });
            var actionResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
        }

        // should return bad request
        [Fact]
        public async Task POST_Album_Update_Invalid_Album_Id()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Update(new AlbumCreateUpdateViewModel { AlbumId = 0 });
            var actionResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
        }

        // model data not changed when model not valid
        [Fact]
        public async Task POST_Album_Update_Model_Error()
        {
            var album = Shared.GetFullAlbum();
            var request = new AlbumCreateUpdateViewModel
            {
                AlbumId = 1,
                AlbumCover = "cover.jpg",
                Title = album.Title,
                Artist = album.Artist.Data,
                Genre = album.Genre.Name,
                Year = album.Year.YearValue,
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
            };

            _albumRepoMock.Setup(x => x.CreateOrUpdateAlbumAsync(It.IsAny<AlbumCreateUpdateViewModel>())).Returns(Task.FromResult(album)).Verifiable();
            _techInfoRepoMock.Setup(x => x.CreateOrUpdateTechnicalInfoAsync(It.IsAny<Album>(), It.IsAny<AlbumCreateUpdateViewModel>())).Returns(Task.FromResult(album.TechnicalInfo)).Verifiable();
            _imageServiceMock.Setup(x => x.SaveCover(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>())).Verifiable();
            _imageServiceMock.Setup(x => x.RemoveCover(It.IsAny<int>(), It.IsAny<EntityType>())).Verifiable();

            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            controller.ModelState.AddModelError("AlbumName", "Required");
            var result = await controller.Update(request);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<AlbumCreateUpdateViewModel>(viewResult.ViewData.Model);

            Assert.Equal(1, model.AlbumId);
            Assert.Equal("Some album", model.Title);
            Assert.Equal(5.4, model.Size);
            Assert.Equal("https://somesource.com", model.Source);
            Assert.Equal("https://discogs.com", model.Discogs);
            Assert.Equal("Some artist", model.Artist);
            Assert.Equal("Heavy Metal", model.Genre);
            Assert.Equal(2010, model.Year);
            Assert.Equal(2020, model.Reissue);
            Assert.Equal("USA", model.Country);
            Assert.Equal("Some label", model.Label);
            Assert.Equal("Some storage", model.Storage);
            Assert.Equal("Some Adc Model", model.Adc);
            Assert.Equal("Some Adc Manufacturer", model.AdcManufacturer);
            Assert.Equal("Some Amplifier Model", model.Amplifier);
            Assert.Equal("Some Amplifier Manufacturer", model.AmplifierManufacturer);
            Assert.Equal("Some Cartridge Model", model.Cartridge);
            Assert.Equal("Some Cartridge Manufacturer", model.CartridgeManufacturer);
            Assert.Equal("Some Player Model", model.Player);
            Assert.Equal("Some Player Manufacturer", model.PlayerManufacturer);
            Assert.Equal("Some Wire Model", model.Wire);
            Assert.Equal("Some Wire Manufacturer", model.WireManufacturer);
            Assert.Equal("LP 12'' 33RPM", model.SourceFormat);
            Assert.Equal(192, model.Sampling);
            Assert.Equal("Mint", model.VinylState);
            Assert.Equal(24, model.Bitness);
            Assert.Equal("FLAC", model.DigitalFormat);
            Assert.Equal("cover.jpg", model.AlbumCover);

            _albumRepoMock.Verify(x => x.CreateOrUpdateAlbumAsync(It.IsAny<AlbumCreateUpdateViewModel>()), Times.Never);
            _techInfoRepoMock.Verify(x => x.CreateOrUpdateTechnicalInfoAsync(It.IsAny<Album>(), It.IsAny<AlbumCreateUpdateViewModel>()), Times.Never);
            _imageServiceMock.Verify(x => x.SaveCover(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>()), Times.Never);
            _imageServiceMock.Verify(x => x.RemoveCover(It.IsAny<int>(), It.IsAny<EntityType>()), Times.Never);
        }

        // album updated, image service SaveCover called if image is set
        [Fact]
        public async Task POST_Album_Update_Album_With_Image()
        {
            var album = Shared.GetFullAlbum();
            var request = new AlbumCreateUpdateViewModel
            {
                AlbumId = 1,
                AlbumCover = "cover.jpg",
                Title = album.Title,
                Artist = album.Artist.Data,
                Genre = album.Genre.Name,
                Year = album.Year.YearValue,
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
            };

            _albumRepoMock.Setup(x => x.CreateOrUpdateAlbumAsync(It.IsAny<AlbumCreateUpdateViewModel>())).Returns(Task.FromResult(album)).Verifiable();
            _techInfoRepoMock.Setup(x => x.CreateOrUpdateTechnicalInfoAsync(It.IsAny<Album>(), It.IsAny<AlbumCreateUpdateViewModel>())).Returns(Task.FromResult(album.TechnicalInfo)).Verifiable();
            _imageServiceMock.Setup(x => x.SaveCover(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>())).Verifiable();
            _imageServiceMock.Setup(x => x.RemoveCover(It.IsAny<int>(), It.IsAny<EntityType>())).Verifiable();

            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Update(request);
            var viewResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/album/1", viewResult.Url);

            _albumRepoMock.Verify(x => x.CreateOrUpdateAlbumAsync(It.IsAny<AlbumCreateUpdateViewModel>()), Times.Once);
            _techInfoRepoMock.Verify(x => x.CreateOrUpdateTechnicalInfoAsync(It.IsAny<Album>(), It.IsAny<AlbumCreateUpdateViewModel>()), Times.Once);
            _imageServiceMock.Verify(x => x.SaveCover(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>()), Times.Once);
            _imageServiceMock.Verify(x => x.RemoveCover(It.IsAny<int>(), It.IsAny<EntityType>()), Times.Never);
        }

        // album updated, image service RemoveCover called if image is set
        [Fact]
        public async Task POST_Album_Update_Album_No_Image()
        {
            var album = Shared.GetFullAlbum();
            var request = new AlbumCreateUpdateViewModel
            {
                AlbumId = 1,
                //AlbumCover = "cover.jpg",
                Title = album.Title,
                Artist = album.Artist.Data,
                Genre = album.Genre.Name,
                Year = album.Year.YearValue,
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
            };

            _albumRepoMock.Setup(x => x.CreateOrUpdateAlbumAsync(It.IsAny<AlbumCreateUpdateViewModel>())).Returns(Task.FromResult(album)).Verifiable();
            _techInfoRepoMock.Setup(x => x.CreateOrUpdateTechnicalInfoAsync(It.IsAny<Album>(), It.IsAny<AlbumCreateUpdateViewModel>())).Returns(Task.FromResult(album.TechnicalInfo)).Verifiable();
            _imageServiceMock.Setup(x => x.SaveCover(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>())).Verifiable();
            _imageServiceMock.Setup(x => x.RemoveCover(It.IsAny<int>(), It.IsAny<EntityType>())).Verifiable();

            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Update(request);
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal("/album/1", redirectResult.Url);

            _albumRepoMock.Verify(x => x.CreateOrUpdateAlbumAsync(It.IsAny<AlbumCreateUpdateViewModel>()), Times.Once);
            _techInfoRepoMock.Verify(x => x.CreateOrUpdateTechnicalInfoAsync(It.IsAny<Album>(), It.IsAny<AlbumCreateUpdateViewModel>()), Times.Once);
            _imageServiceMock.Verify(x => x.SaveCover(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<EntityType>()), Times.Never);
            _imageServiceMock.Verify(x => x.RemoveCover(It.IsAny<int>(), It.IsAny<EntityType>()), Times.Once);
        }
        #endregion

        #region DELETE album/delete
        
        [Fact]
        public async Task DELETE_Album_Return_Bad_Request()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Delete(0);
            var actionResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, actionResult.StatusCode);
        }

        [Fact]
        public async Task DELETE_Album_No_Album_Return_Not_Found()
        {
            _albumRepoMock.Setup(x => x.Albums).Returns(new List<Album>().BuildMock());
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Delete(1);
            var actionResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, actionResult.StatusCode);
        }

        [Fact]
        public async Task DELETE_Album_Return_OK()
        {
            var controller = new AlbumController(_albumRepoMock.Object, _imageServiceMock.Object, _techInfoRepoMock.Object);
            var result = await controller.Delete(1);
            var actionResult = Assert.IsType<OkResult>(result);
        }
        #endregion
    }
}
