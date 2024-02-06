using Web.Models;

namespace Web.Db
{
    public interface IAlbumRepository
    {
        IQueryable<Album> Albums { get; }
        IQueryable<Artist> Artists { get; }
        IQueryable<Country> Countries { get; }
        IQueryable<Genre> Genres { get; }
        IQueryable<Year> Years { get; }
        IQueryable<Label> Labels { get; }
        IQueryable<Reissue> Reissues { get; }
    }
}
