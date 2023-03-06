using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Db
{
    public class DbRepository : IDbRepository
    {
        private Context _context;
        public DbRepository(Context ctx)
        {
            _context = ctx;
        }
        public IQueryable<Adc> Adcs => _context.Adces;
        public IQueryable<Album> Albums => _context.Albums;

        public IQueryable<Amplifier> Amplifiers => _context.Amplifiers;

        public IQueryable<Artist> Artists => _context.Artists;

        public IQueryable<Bitness> Bitnesses => _context.Bitnesses;

        public IQueryable<Cartrige> Cartriges => _context.Cartriges;

        public IQueryable<Codec> Codecs => _context.Codecs;

        public IQueryable<Country> Countries => _context.Countries;

        public IQueryable<Device> Devices => _context.Devices;

        public IQueryable<Format> Formats => _context.Formats;

        public IQueryable<Genre> Genres => _context.Genres;

        public IQueryable<Label> Labels => _context.Labels;

        public IQueryable<Processing> Processings => _context.Processings;

        public IQueryable<Reissue> Reissues => _context.Reissues;

        public IQueryable<Sampling> Samplings => _context.Samplings;

        public IQueryable<State> States => _context.States;

        public IQueryable<Year> Years => _context.Years;
    }
}
