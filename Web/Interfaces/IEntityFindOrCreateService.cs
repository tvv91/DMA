using Web.Models;

namespace Web.Interfaces
{
    public interface IEntityFindOrCreateService
    {
        Task<Year> FindOrCreateYearAsync(int yearValue);
        Task<Reissue> FindOrCreateReissueAsync(int reissueValue);
        Task<Country> FindOrCreateCountryAsync(string countryName);
        Task<Label> FindOrCreateLabelAsync(string labelName);
        Task<Storage> FindOrCreateStorageAsync(string storageData);
        Task<Bitness> FindOrCreateBitnessAsync(int bitnessValue);
        Task<Sampling> FindOrCreateSamplingAsync(double samplingValue);
        Task<DigitalFormat> FindOrCreateDigitalFormatAsync(string formatName);
        Task<SourceFormat> FindOrCreateSourceFormatAsync(string formatName);
        Task<VinylState> FindOrCreateVinylStateAsync(string stateName);
        Task<Player> FindOrCreatePlayerAsync(string playerName, string? manufacturerName = null);
        Task<Cartridge> FindOrCreateCartridgeAsync(string cartridgeName, string? manufacturerName = null);
        Task<Amplifier> FindOrCreateAmplifierAsync(string amplifierName, string? manufacturerName = null);
        Task<Adc> FindOrCreateAdcAsync(string adcName, string? manufacturerName = null);
        Task<Wire> FindOrCreateWireAsync(string wireName, string? manufacturerName = null);
        Task<Manufacturer?> FindOrCreateManufacturerAsync(string manufacturerName);
    }
}
