using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.Request;

namespace Web.Db.Implementation
{
    public class TechnicalnfoRepository : ITechInfoRepository
    {
        private readonly DMADbContext _context;

        public TechnicalnfoRepository(DMADbContext ctx) => _context = ctx;

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

        public async Task<TechnicalInfo> CreateNewTechnicallInfo(NewAlbumRequest request)
        {
            var tinfo = new TechnicalInfo();

            if (request.Adc != null)
            {
                var adc = await FindAdcAsync(request.Adc);
                if (adc != null)
                {
                    adc = await CreateAdcAsync(request.Adc);
                }
                tinfo.Adc = adc;
            }

            if (request.Amplifier != null)
            {
                var amp = await FindAmpAsync(request.Amplifier);
                if (amp != null)
                {
                    amp = await CreateAmpAsync(request.Amplifier);
                }
                tinfo.Amplifier = amp;
            }

            if (request.Bitness != null)
            {
                var bitness = await FindBitnessAsync(request.Bitness);
                if (bitness != null)
                {
                    bitness = await CreateBitnessAsync(request.Bitness);
                }
                tinfo.Bitness = bitness;
            }
            return tinfo;
        }

        private async Task<Amplifier> FindAmpAsync(string data)
        {
            return await Amplifiers.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Amplifier> CreateAmpAsync(string data)
        {
            var amp = new Amplifier { Data = data };
            await _context.Amplifiers.AddAsync(amp);
            await _context.SaveChangesAsync();
            return amp;
        }

        private async Task<Adc> FindAdcAsync(string data)
        {
            return await Adcs.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Adc> CreateAdcAsync(string data)
        {
            var adc = new Adc { Data = data };
            await _context.Adces.AddAsync(adc);
            await _context.SaveChangesAsync();
            return adc;
        }
    }
}
