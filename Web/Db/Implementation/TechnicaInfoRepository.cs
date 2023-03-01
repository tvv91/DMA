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
        public IQueryable<AdcManufacturer> AdcManufacturers => _context.AdcManufacturers;
        public IQueryable<Amplifier> Amplifiers => _context.Amplifiers;
        public IQueryable<AmplifierManufacturer> AmplifierManufacturers => _context.AmplifierManufacturers;
        public IQueryable<Bitness> Bitnesses => _context.Bitnesses;
        public IQueryable<Cartrige> Cartriges => _context.Cartriges;
        public IQueryable<CartrigeManufacturer> CartrigeManufacturers => _context.CartrigeManufacturers;
        public IQueryable<DigitalFormat> DigitalFormats => _context.DigitalFormats;
        public IQueryable<Player> Players => _context.Players;
        public IQueryable<PlayerManufacturer> PlayerManufacturers => _context.PlayerManufacturers;
        public IQueryable<SourceFormat> SourceFormats => _context.SourceFormats;
        public IQueryable<Processing> Processings => _context.Processings;
        public IQueryable<Sampling> Samplings => _context.Samplings;
        public IQueryable<VinylState> VinylStates => _context.VinylStates;
        public IQueryable<Wire> Wires => _context.Wires;
        public IQueryable<WireManufacturer> WireManufacturers => _context.WireManufacturers;
        public IQueryable<TechnicalInfo> TechInfos => _context.TechnicalInfos;

        public async Task<TechnicalInfo> CreateNewTechnicalInfoAsync(AlbumDataRequest request)
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
                
                if (request.AdcManufacturer != null)
                {
                    var adcManufacturer = await FindAdcManufacturerAsync(request.AdcManufacturer);
                    if (adcManufacturer == null)
                    {
                        adcManufacturer = await CreateAdcManuFacturerAsync(request.AdcManufacturer);
                    }
                    adc.AdcManufacturer = adcManufacturer;
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

                if (request.AmplifierManufacturer != null)
                {
                    var ampManufacturer = await FindAmpManufacturerAsync(request.AmplifierManufacturer);
                    if (ampManufacturer == null)
                    {
                        ampManufacturer = await CreateAmpManufacturerAsync(request.AmplifierManufacturer);
                    }
                    amp.AmplifierManufacturer = ampManufacturer;
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

                if (request.CartridgeManufacturer != null)
                {
                    var cartridgeManufacturer = await FindCartridgeManufacturerAsync(request.CartridgeManufacturer);
                    if (cartridgeManufacturer == null)
                    {
                        cartridgeManufacturer = await CreateCartridgeManufacturerAsync(request.CartridgeManufacturer);
                    }
                    cartridge.CartrigeManufacturer= cartridgeManufacturer;
                }

                notNull = true;
                tinfo.Cartrige = cartridge;
            }

            if (request.DigitalFormat != null)
            {
                var codec = await FindDigitalFormatAsync(request.DigitalFormat);
                if (codec == null)
                {
                    codec = await CreateDigitalFormatAsync(request.DigitalFormat);
                }
                notNull = true;
                tinfo.DigitalFormat = codec;
            }

            if (request.Player != null)
            {
                var player = await FindPlayerAsync(request.Player);
                if (player == null)
                {
                    player = await CreatePlayerAsync(request.Player);
                }

                if (request.PlayerManufacturer != null)
                {
                    var manufacturer = await FindPlayerManufacturerAsync(request.PlayerManufacturer);
                    if (manufacturer == null)
                    {
                        manufacturer = await CreatePlayerManufacturerAsync(request.PlayerManufacturer);
                    }
                    player.PlayerManufacturer = manufacturer;
                }
                notNull = true;
                tinfo.Player = player;
            }

            if (request.SourceFormat != null)
            {
                var format = await FindSourceFormatAsync(request.SourceFormat);
                if (format == null)
                {
                    format = await CreateSourceFormatAsync(request.SourceFormat);
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

            if (request.VinylState != null)
            {
                var state = await FindVinylStateAsync(request.VinylState);
                if (state == null)
                {
                    state = await CreateVinylStateAsync(request.VinylState);
                }
                notNull = true;
                tinfo.VinylState = state;
            }

            if (request.Wire != null)
            {
                var wire = await FindWireAsync(request.Wire);
                if (wire == null)
                {
                    wire = await CreateWireAsync(request.Wire);
                }

                if (request.WireManufacturer != null)
                {
                    var wireManufacturer = await FindWireManufacturerAsync(request.WireManufacturer);
                    if (wireManufacturer == null)
                    {
                        wireManufacturer = await CreateWireManufacturerAsync(request.WireManufacturer);
                    }
                    wire.WireManufacturer = wireManufacturer;
                }

                notNull = true;
                tinfo.Wire = wire;
            }

            return notNull == true ? tinfo : null;
        }

        #region Create methods
        private async Task<PlayerManufacturer> CreatePlayerManufacturerAsync(string data)
        {
            var manufacturer = new PlayerManufacturer { Data = data };
            await _context.PlayerManufacturers.AddAsync(manufacturer);
            await _context.SaveChangesAsync();
            return manufacturer;
        }

        private async Task<WireManufacturer?> CreateWireManufacturerAsync(string data)
        {
            var manufacturer = new WireManufacturer { Data = data };
            await _context.WireManufacturers.AddAsync(manufacturer);
            await _context.SaveChangesAsync();
            return manufacturer;
        }

        private async Task<CartrigeManufacturer?> CreateCartridgeManufacturerAsync(string data)
        {
            var manufacturer = new CartrigeManufacturer { Data = data };
            await _context.CartrigeManufacturers.AddAsync(manufacturer);
            await _context.SaveChangesAsync();
            return manufacturer;
        }

        private async Task<AmplifierManufacturer?> CreateAmpManufacturerAsync(string data)
        {
            var manufacturer = new AmplifierManufacturer { Data = data };
            await _context.AmplifierManufacturers.AddAsync(manufacturer);
            await _context.SaveChangesAsync();
            return manufacturer;
        }

        private async Task<AdcManufacturer> CreateAdcManuFacturerAsync(string data)
        {
            var manufacturer = new AdcManufacturer { Data = data };
            await _context.AdcManufacturers.AddAsync(manufacturer);
            await _context.SaveChangesAsync();
            return manufacturer;
        }

        private async Task<DigitalFormat> CreateDigitalFormatAsync(string data)
        {
            var digitalFormat = new DigitalFormat { Data = data };
            await _context.DigitalFormats.AddAsync(digitalFormat);
            await _context.SaveChangesAsync();
            return digitalFormat;
        }

        private async Task<Player> CreatePlayerAsync(string data)
        {
            var device = new Player { Data = data };
            await _context.Players.AddAsync(device);
            await _context.SaveChangesAsync();
            return device;
        }
        private async Task<SourceFormat> CreateSourceFormatAsync(string data)
        {
            var sourceFormat = new SourceFormat { Data = data };
            await _context.SourceFormats.AddAsync(sourceFormat);
            await _context.SaveChangesAsync();
            return sourceFormat;
        }

        private async Task<Processing> CreateProcessingAsync(string data)
        {
            var processing = new Processing { Data = data };
            await _context.Processings.AddAsync(processing);
            await _context.SaveChangesAsync();
            return processing;
        }

        private async Task<Sampling> CreateSamplingAsync(double data)
        {
            var sampling = new Sampling { Data = data };
            await _context.Samplings.AddAsync(sampling);
            await _context.SaveChangesAsync();
            return sampling;
        }

        private async Task<VinylState> CreateVinylStateAsync(string data)
        {
            var vinylState = new VinylState { Data = data };
            await _context.VinylStates.AddAsync(vinylState);
            await _context.SaveChangesAsync();
            return vinylState;
        }

        private async Task<Cartrige> CreateCartridgeAsync(string data)
        {
            var cartridge = new Cartrige { Data = data };
            await _context.Cartriges.AddAsync(cartridge);
            await _context.SaveChangesAsync();
            return cartridge;
        }

        private async Task<Bitness> CreateBitnessAsync(int data)
        {
            var bitness = new Bitness { Data = data };
            await _context.Bitnesses.AddAsync(bitness);
            await _context.SaveChangesAsync();
            return bitness;
        }

        private async Task<Amplifier> CreateAmpAsync(string data)
        {
            var amp = new Amplifier { Data = data };
            await _context.Amplifiers.AddAsync(amp);
            await _context.SaveChangesAsync();
            return amp;
        }

        private async Task<Adc> CreateAdcAsync(string data)
        {
            var adc = new Adc { Data = data };
            await _context.Adces.AddAsync(adc);
            await _context.SaveChangesAsync();
            return adc;
        }

        private async Task<Wire> CreateWireAsync(string data)
        {
            var wire = new Wire { Data = data };
            await _context.Wires.AddAsync(wire);
            await _context.SaveChangesAsync();
            return wire;
        }
        #endregion

        #region Find methods
        private async Task<PlayerManufacturer> FindPlayerManufacturerAsync(string data)
        {
            return await _context.PlayerManufacturers.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<WireManufacturer> FindWireManufacturerAsync(string data)
        {
            return await _context.WireManufacturers.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<CartrigeManufacturer> FindCartridgeManufacturerAsync(string data)
        {
            return await _context.CartrigeManufacturers.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<AmplifierManufacturer> FindAmpManufacturerAsync(string data)
        {
            return await _context.AmplifierManufacturers.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<AdcManufacturer> FindAdcManufacturerAsync(string data)
        {
            return await _context.AdcManufacturers.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<DigitalFormat> FindDigitalFormatAsync(string data)
        {
            return await DigitalFormats.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Player> FindPlayerAsync(string data)
        {
            return await Players.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<SourceFormat> FindSourceFormatAsync(string data)
        {
            return await SourceFormats.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Processing> FindProcessingAsync(string data)
        {
            return await Processings.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Sampling> FindSamplingAsync(double data)
        {
            return await Samplings.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<VinylState> FindVinylStateAsync(string data)
        {
            return await VinylStates.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Cartrige> FindCartridgeAsync(string data)
        {
            return await Cartriges.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Bitness> FindBitnessAsync(int data)
        {
            return await Bitnesses.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Amplifier> FindAmpAsync(string data)
        {
            return await Amplifiers.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Adc> FindAdcAsync(string data)
        {
            return await Adcs.FirstOrDefaultAsync(x => x.Data == data);
        }

        private async Task<Wire> FindWireAsync(string data)
        {
            return await Wires.FirstOrDefaultAsync(x => x.Data == data);
        }
        #endregion        

        public async Task<TechnicalInfo> UpdateTechnicalInfoAsync(int id, AlbumDataRequest request)
        {
            var techInfo = await _context.TechnicalInfos
                .Include(x => x.VinylState)
                .Include(x => x.DigitalFormat)
                .Include(x => x.Bitness)
                .Include(x => x.Sampling)
                .Include(x => x.SourceFormat)
                .Include(x => x.Player)
                .ThenInclude(x => x.PlayerManufacturer)
                .Include(x => x.Cartrige)
                .ThenInclude(x => x.CartrigeManufacturer)
                .Include(x => x.Amplifier)
                .ThenInclude(x => x.AmplifierManufacturer)
                .Include(x => x.Adc)
                .ThenInclude(x => x.AdcManufacturer)
                .Include(x => x.Wire)
                .ThenInclude(x => x.WireManufacturer)
                .Include(x => x.Processing)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (request == null)
            {
                return await CreateNewTechnicalInfoAsync(request);
            }

            if (techInfo?.VinylState?.Data != request.VinylState)
            {
                var vinylState = await FindVinylStateAsync(request.VinylState);
                if (vinylState == null)
                {
                    vinylState = await CreateVinylStateAsync(request.VinylState);
                }
                techInfo.VinylState = vinylState;
            }

            if (techInfo?.DigitalFormat?.Data != request.DigitalFormat)
            {
                var digitalFormat = await FindDigitalFormatAsync(request.DigitalFormat);
                if (digitalFormat == null)
                {
                    digitalFormat = await CreateDigitalFormatAsync(request.DigitalFormat);
                }
                techInfo.DigitalFormat = digitalFormat;
            }

            if (techInfo?.Bitness?.Data != request.Bitness)
            {
                var bitness = await FindBitnessAsync(request.Bitness.Value);
                if (bitness == null)
                {
                    bitness = await CreateBitnessAsync(request.Bitness.Value);
                }
                techInfo.Bitness = bitness;
            }

            if (techInfo?.Sampling?.Data != request.Sampling)
            {
                var sampling = await FindSamplingAsync(request.Sampling.Value);
                if (sampling == null)
                {
                    sampling = await CreateSamplingAsync(request.Sampling.Value);
                }
                techInfo.Sampling = sampling;
            }

            if (techInfo?.SourceFormat?.Data != request.SourceFormat)
            {
                var sourceFormat = await FindSourceFormatAsync(request.SourceFormat);
                if (sourceFormat == null)
                {
                    sourceFormat = await CreateSourceFormatAsync(request.SourceFormat);
                }
                techInfo.SourceFormat = sourceFormat;
            }

            if (techInfo?.Processing?.Data != request.Processing)
            {
                var processing = await FindProcessingAsync(request.Processing);
                if (processing == null)
                {
                    processing = await CreateProcessingAsync(request.Processing);
                }
                techInfo.Processing = processing;
            }

            if (techInfo?.Player?.Data != request.Player)
            {
                var player = await FindPlayerAsync(request.Player);
                if (player == null)
                {
                    player = await CreatePlayerAsync(request.Player);
                }

                if (techInfo?.Player?.PlayerManufacturer?.Data != request.PlayerManufacturer)
                {
                    var manufacturer = await FindPlayerManufacturerAsync(request.PlayerManufacturer);
                    if (manufacturer == null)
                    {
                        manufacturer = await CreatePlayerManufacturerAsync(request.PlayerManufacturer);
                    }
                    player.PlayerManufacturer = manufacturer;
                }
                techInfo.Player = player;
            }

            if (techInfo?.Cartrige?.Data != request.Cartridge)
            {
                var cartridge = await FindCartridgeAsync(request.Cartridge);
                if (cartridge == null)
                {
                    cartridge = await CreateCartridgeAsync(request.Cartridge);
                }

                if (techInfo?.Cartrige?.CartrigeManufacturer?.Data != request.CartridgeManufacturer)
                {
                    var manufacturer = await FindCartridgeManufacturerAsync(request.CartridgeManufacturer);
                    if (manufacturer == null)
                    {
                        manufacturer = await CreateCartridgeManufacturerAsync(request.CartridgeManufacturer);
                    }
                    cartridge.CartrigeManufacturer = manufacturer;
                }
                techInfo.Cartrige = cartridge;
            }

            if (techInfo?.Amplifier?.Data != request.Amplifier)
            {
                var amplifier = await FindAmpAsync(request.Amplifier);
                if (amplifier == null)
                {
                    amplifier = await CreateAmpAsync(request.Amplifier);
                }

                if (techInfo?.Amplifier?.AmplifierManufacturer?.Data != request.AmplifierManufacturer)
                {
                    var manufacturer = await FindAmpManufacturerAsync(request.AmplifierManufacturer);
                    if (manufacturer == null)
                    {
                        manufacturer = await CreateAmpManufacturerAsync(request.AmplifierManufacturer);
                    }
                    amplifier.AmplifierManufacturer = manufacturer;
                }
                techInfo.Amplifier = amplifier;
            }

            if (techInfo?.Adc?.Data != request.Adc)
            {
                var adc = await FindAdcAsync(request.Adc);
                if (adc == null)
                {
                    adc = await CreateAdcAsync(request.Adc);
                }

                if (techInfo?.Adc?.AdcManufacturer?.Data != request.AdcManufacturer)
                {
                    var manufacturer = await FindAdcManufacturerAsync(request.AdcManufacturer);
                    if (manufacturer == null)
                    {
                        manufacturer = await CreateAdcManuFacturerAsync(request.AdcManufacturer);
                    }
                    adc.AdcManufacturer = manufacturer;
                }
                techInfo.Adc = adc;
            }

            if (techInfo?.Wire?.Data != request.Wire)
            {
                var wire = await FindWireAsync(request.Wire);
                if (wire == null)
                {
                    wire = await CreateWireAsync(request.Wire);
                }

                if (techInfo?.Wire?.WireManufacturer?.Data != request.WireManufacturer)
                {
                    var manufacturer = await FindWireManufacturerAsync(request.WireManufacturer);
                    if (manufacturer == null)
                    {
                        manufacturer = await CreateWireManufacturerAsync(request.WireManufacturer);
                    }
                    wire.WireManufacturer = manufacturer;
                }
                techInfo.Wire = wire;
            }

            await _context.SaveChangesAsync();
            return techInfo;
        }
    }
}
