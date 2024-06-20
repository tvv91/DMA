using Web.Models;

namespace Web.Db.Implementation
{
    public class TechnicalInfoRepository : ITechInfoRepository
    {
        private readonly DMADbContext _context;

        public TechnicalInfoRepository(DMADbContext ctx) => _context = ctx;

        public IQueryable<Adc> Adcs => _context.Adces;

        public IQueryable<Amplifier> Amplifiers => _context.Amplifiers;

        public IQueryable<Bitness> Bitnesses => _context.Bitnesses;

        public IQueryable<Cartrige> Cartriges => _context.Cartriges;

        public IQueryable<Codec> Codecs => _context.Codecs;

        public IQueryable<Device> Devices => _context.Devices;

        public IQueryable<Format> Formats => _context.Formats;

        public IQueryable<Processing> Processings => _context.Processings;

        public IQueryable<Sampling> Samplings => _context.Samplings;

        public IQueryable<State> States => _context.States;

        public IQueryable<TechnicalInfo> TechInfos => _context.TechnicalInfos;
    }
}
