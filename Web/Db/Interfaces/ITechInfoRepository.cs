using Web.Enums;
using Web.Models;
using Web.ViewModels;

namespace Web.Db
{
    public interface ITechInfoRepository
    {
        IQueryable<TechnicalInfo> TechInfos { get; }
        IQueryable<Adc> Adcs { get; }
        IQueryable<AdcManufacturer> AdcManufacturers { get; }
        IQueryable<Amplifier> Amplifiers { get; }
        IQueryable<AmplifierManufacturer> AmplifierManufacturers { get; }
        IQueryable<Bitness> Bitnesses { get; }
        IQueryable<Cartridge> Cartridges { get; }
        IQueryable<CartridgeManufacturer> CartridgeManufacturers { get; }
        IQueryable<DigitalFormat> DigitalFormats { get; }
        IQueryable<Player> Players { get; }
        IQueryable<PlayerManufacturer> PlayerManufacturers { get; }
        IQueryable<SourceFormat> SourceFormats { get; }
        IQueryable<Sampling> Samplings { get; }
        IQueryable<VinylState> VinylStates { get; }
        IQueryable<Wire> Wires { get; }
        IQueryable<WireManufacturer> WireManufacturers { get; }
        Task<TechnicalInfo> CreateOrUpdateTechnicalInfoAsync(Album album, AlbumCreateUpdateViewModel request);
        Task<TechnicalInfo?> GetByIdAsync(int id);
        Task<int> CreateOrUpdateEquipmentAsync(EquipmentViewModel request);
        Task<bool> DeleteEquipmentAsync(int id, EntityType categorye);
        Task<EquipmentViewModel?> GetEquipmentByIdAsync(EntityType type, int id);
    }
}
