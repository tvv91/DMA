using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Db
{
    public class DbRepository : IAlbumRepository
    {
        private DMADbContext _context;
        public DbRepository(DMADbContext ctx)
        {
            _context = ctx;
        }
        public IQueryable<Album> Albums => _context.Albums;

        public IQueryable<Artist> Artists => _context.Artists;

        public IQueryable<Country> Countries => _context.Countries;

        public IQueryable<Genre> Genres => _context.Genres;

        public IQueryable<Reissue> Reissues => _context.Reissues;

        public IQueryable<Year> Years => _context.Years;

        public IQueryable<Label> Labels => _context.Labels;
    }
}
