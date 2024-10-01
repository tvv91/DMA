using Web.Models;
using Xunit;

namespace Tests
{
    /*
    public class TestDataSelfTests
    {
        private readonly ICollection<Album> albums;
        
        public TestDataSelfTests()
        {
            albums = new TestData().GetData();
        }

        [Fact]
        public void ShouldBeNotNull()
        {
            Assert.NotNull(albums);
        }

        [Fact]
        public void ShouldBe_25_Albums()
        {
            Assert.Equal(25, albums.Count());
        }

        [Fact]
        public void ShouldBe_3_Artists()
        {
            Assert.Equal(3, albums.Select(x => x.Artist).Distinct().Count());
        }

        [Fact]
        public void ShouldBe_5_Genres()
        {
            Assert.Equal(5, albums.Select(x => x.Genre).Distinct().Count());
        }

        [Fact]
        public void ShouldBe_5_Years()
        {
            Assert.Equal(5, albums.Select(x => x.Year).Distinct().Count());
        }

        [Fact]
        public void ShouldBe_3_Reissues()
        {
            Assert.Equal(3, albums.Where(x => x.Reissue != null).Select(x => x.Reissue).Distinct().Count());
        }

        [Fact]
        public void ShouldBe_3_Countries()
        {
            Assert.Equal(3, albums.Where(x => x.Country != null).Select(x => x.Country).Distinct().Count());
        }

        [Fact]
        public void ShouldBe_3_Labels()
        {
            Assert.Equal(3, albums.Where(x => x.Label != null).Select(x => x.Label).Distinct().Count());
        }

        [Fact]
        public void ShouldBe_5_TechnicalInfos()
        {
            Assert.Equal(5, albums.Where(x => x.TechnicalInfo != null).Select(x => x.TechnicalInfo).Distinct().Count());
        }
    }
    */
}
