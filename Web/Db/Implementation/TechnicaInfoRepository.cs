﻿using Microsoft.EntityFrameworkCore;
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

        public IQueryable<DigitalFormat> Codecs => _context.DigitalFormats;

        public IQueryable<Player> Devices => _context.Players;

        public IQueryable<SourceFormat> Formats => _context.SourceFormats;

        public IQueryable<Processing> Processings => _context.Processings;

        public IQueryable<Sampling> Samplings => _context.Samplings;

        public IQueryable<VinylState> States => _context.VinylStates;
        
        public IQueryable<Wire> Wires => _context.Wires;

        public IQueryable<TechnicalInfo> TechInfos => _context.TechnicalInfos;

        public async Task<TechnicalInfo> CreateNewTechnicallInfoAsync(AlbumDataRequest request)
        {
            var tinfo = new TechnicalInfo();
            // set true if at lease one property not null, else return null and not create empty record in db
            bool notNull = false;
            if (request.Adc != null)
            {
                var adc = await FindAdcAsync(request.Adc);
                if (adc == null)
                {
                    adc = await CreateAdcAsync(request.Adc);
                }
                notNull = true;
                tinfo.Adc = adc;
            }

            if (request.Amplifier != null)
            {
                var amp = await FindAmpAsync(request.Amplifier);
                if (amp == null)
                {
                    amp = await CreateAmpAsync(request.Amplifier);
                }
                notNull = true;
                tinfo.Amplifier = amp;
            }

            if (request.Bitness != null)
            {
                var bitness = await FindBitnessAsync(request.Bitness.Value);
                if (bitness == null)
                {
                    bitness = await CreateBitnessAsync(request.Bitness.Value);
                }
                notNull = true;
                tinfo.Bitness = bitness;
            }

            if (request.Cartridge != null)
            {
                var cartridge = await FindCartridgeAsync(request.Cartridge);
                if (cartridge == null)
                {
                    cartridge = await CreateCartridgeAsync(request.Cartridge);
                }
                notNull = true;
                tinfo.Cartrige = cartridge;
            }

            if (request.Codec != null)
            {
                var codec = await FindCodecAsync(request.Codec);
                if (codec == null)
                {
                    codec = await CreateCodecAsync(request.Codec);
                }
                notNull = true;
                tinfo.DigitalFormat = codec;
            }

            if (request.Device != null)
            {
                var device = await FindDeviceAsync(request.Device);
                if (device == null)
                {
                    device = await CreateDeviceAsync(request.Device);
                }
                notNull = true;
                tinfo.Player = device;
            }

            if (request.Format != null)
            {
                var format = await FindFormatAsync(request.Format);
                if (format == null)
                {
                    format = await CreateFormatAsync(request.Format);
                }
                notNull = true;
                tinfo.SourceFormat = format;
            }

            if (request.Processing != null)
            {
                var processing = await FindProcessingAsync(request.Processing);
                if (processing == null)
                {
                    processing = await CreateProcessingAsync(request.Processing);
                }
                notNull = true;
                tinfo.Processing = processing;
            }

            if (request.Sampling != null)
            {
                var sampling = await FindSamplingAsync(request.Sampling.Value);
                if (sampling == null)
                {
                    sampling = await CreateSamplingAsync(request.Sampling.Value);
                }
                notNull = true;
                tinfo.Sampling = sampling;
            }

            if (request.State != null)
            {
                var state = await FindStateAsync(request.State);
                if (state == null)
                {
                    state = await CreateStateAsync(request.State);
                }
                notNull = true;
                tinfo.VinylState = state;
            }

            return notNull == true ? tinfo : null;
        }

        private async Task<DigitalFormat> FindCodecAsync(string data)
        {
            return await Codecs.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<DigitalFormat> CreateCodecAsync(string data)
        {
            var codec = new DigitalFormat { Data = data };
            await _context.DigitalFormats.AddAsync(codec);
            await _context.SaveChangesAsync();
            return codec;
        }

        private async Task<Player> FindDeviceAsync(string data)
        {
            return await Devices.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Player> CreateDeviceAsync(string data)
        {
            var device = new Player { Data = data };
            await _context.Players.AddAsync(device);
            await _context.SaveChangesAsync();
            return device;
        }

        private async Task<SourceFormat> FindFormatAsync(string data)
        {
            return await Formats.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<SourceFormat> CreateFormatAsync(string data)
        {
            var format = new SourceFormat { Data = data };
            await _context.SourceFormats.AddAsync(format);
            await _context.SaveChangesAsync();
            return format;
        }

        private async Task<Processing> FindProcessingAsync(string data)
        {
            return await Processings.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Processing> CreateProcessingAsync(string data)
        {
            var processing = new Processing { Data = data };
            await _context.Processings.AddAsync(processing);
            await _context.SaveChangesAsync();
            return processing;
        }

        private async Task<Sampling> FindSamplingAsync(double data)
        {
            return await Samplings.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Sampling> CreateSamplingAsync(double data)
        {
            var sampling = new Sampling { Data = data };
            await _context.Samplings.AddAsync(sampling);
            await _context.SaveChangesAsync();
            return sampling;
        }

        private async Task<VinylState> FindStateAsync(string data)
        {
            return await States.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<VinylState> CreateStateAsync(string data)
        {
            var state = new VinylState { Data = data };
            await _context.VinylStates.AddAsync(state);
            await _context.SaveChangesAsync();
            return state;
        }

        private async Task<Cartrige> FindCartridgeAsync(string data)
        {
            return await Cartriges.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Cartrige> CreateCartridgeAsync(string data)
        {
            var cartridge = new Cartrige { Data = data };
            await _context.Cartriges.AddAsync(cartridge);
            await _context.SaveChangesAsync();
            return cartridge;
        }

        private async Task<Bitness> FindBitnessAsync(int data)
        {
            return await Bitnesses.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Bitness> CreateBitnessAsync(int data)
        {
            var bitness = new Bitness { Data = data };
            await _context.Bitnesses.AddAsync(bitness);
            await _context.SaveChangesAsync();
            return bitness;
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

        private async Task<Wire> FindWireAsync(string data)
        {
            return await Wires.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Wire> CreateWireAsync(string data)
        {
            var wire = new Wire { Data = data };
            await _context.Wires.AddAsync(wire);
            await _context.SaveChangesAsync();
            return wire;
        }
    }
}
