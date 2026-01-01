using Web.Models;
using Web.Enums;

namespace Tests.Helpers
{
    public static class TestDataBuilder
    {
        public static Post CreatePost(int id = 1, string title = "Test Post", bool isDraft = false)
        {
            return new Post
            {
                Id = id,
                Title = title,
                Description = "Test Description",
                Content = "Test Content",
                IsDraft = isDraft,
                CreatedDate = DateTime.UtcNow
            };
        }

        public static Category CreateCategory(int id = 1, string title = "Test Category")
        {
            return new Category
            {
                Id = id,
                Title = title
            };
        }

        public static PostCategory CreatePostCategory(int postId, int categoryId)
        {
            return new PostCategory
            {
                PostId = postId,
                CategoryId = categoryId
            };
        }


        public static Artist CreateArtist(int id = 0, string name = "Test Artist")
        {
            var artist = new Artist
            {
                Name = name
            };
            if (id > 0)
            {
                artist.Id = id;
            }
            return artist;
        }

        public static Genre CreateGenre(int id = 0, string name = "Test Genre")
        {
            var genre = new Genre
            {
                Name = name
            };
            if (id > 0)
            {
                genre.Id = id;
            }
            return genre;
        }

        public static Album CreateAlbum(int id = 0, string title = "Test Album", string? artistName = null, string? genreName = null)
        {
            var artist = artistName != null ? CreateArtist(0, artistName) : CreateArtist(0);
            var genre = genreName != null ? CreateGenre(0, genreName) : CreateGenre(0);
            
            var album = new Album
            {
                Title = title,
                Artist = artist,
                Genre = genre,
                AddedDate = DateTime.UtcNow
            };
            
            if (id > 0)
            {
                album.Id = id;
            }
            
            return album;
        }

        public static Digitization CreateDigitization(int id = 0, int albumId = 1, string? source = null)
        {
            var digitization = new Digitization
            {
                AlbumId = albumId,
                Source = source ?? "Test Source",
                AddedDate = DateTime.UtcNow
            };
            if (id > 0)
            {
                digitization.Id = id;
            }
            return digitization;
        }

        public static Manufacturer CreateManufacturer(int id = 1, string name = "Test Manufacturer", Web.Enums.EntityType type = Web.Enums.EntityType.AdcManufacturer)
        {
            return new Manufacturer
            {
                Id = id,
                Name = name,
                Type = type
            };
        }

        public static Adc CreateAdc(int id = 1, string name = "Test ADC", Manufacturer? manufacturer = null)
        {
            return new Adc
            {
                Id = id,
                Name = name,
                Description = "Test Description",
                Manufacturer = manufacturer ?? CreateManufacturer()
            };
        }

        public static Player CreatePlayer(int id = 1, string name = "Test Player", Manufacturer? manufacturer = null)
        {
            return new Player
            {
                Id = id,
                Name = name,
                Description = "Test Description",
                Manufacturer = manufacturer ?? CreateManufacturer()
            };
        }

        public static Amplifier CreateAmplifier(int id = 1, string name = "Test Amplifier", Manufacturer? manufacturer = null)
        {
            return new Amplifier
            {
                Id = id,
                Name = name,
                Description = "Test Description",
                Manufacturer = manufacturer ?? CreateManufacturer()
            };
        }

        public static Cartridge CreateCartridge(int id = 1, string name = "Test Cartridge", Manufacturer? manufacturer = null)
        {
            return new Cartridge
            {
                Id = id,
                Name = name,
                Description = "Test Description",
                Manufacturer = manufacturer ?? CreateManufacturer()
            };
        }

        public static Wire CreateWire(int id = 1, string name = "Test Wire", Manufacturer? manufacturer = null)
        {
            return new Wire
            {
                Id = id,
                Name = name,
                Description = "Test Description",
                Manufacturer = manufacturer ?? CreateManufacturer()
            };
        }
    }
}

