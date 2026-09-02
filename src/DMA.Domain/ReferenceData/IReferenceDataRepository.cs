namespace DMA.Domain.ReferenceData;

public interface IReferenceDataRepository
{
    Task<Year> FindOrCreateYearAsync(int yearValue, CancellationToken cancellationToken = default);
    Task<Reissue> FindOrCreateReissueAsync(int reissueValue, CancellationToken cancellationToken = default);
    Task<Country> FindOrCreateCountryAsync(string countryName, CancellationToken cancellationToken = default);
    Task<Label> FindOrCreateLabelAsync(string labelName, CancellationToken cancellationToken = default);
    Task<Storage> FindOrCreateStorageAsync(string storageData, CancellationToken cancellationToken = default);
    Task<Bitness> FindOrCreateBitnessAsync(int bitnessValue, CancellationToken cancellationToken = default);
    Task<Sampling> FindOrCreateSamplingAsync(double samplingValue, CancellationToken cancellationToken = default);
    Task<DigitalFormat> FindOrCreateDigitalFormatAsync(string formatName, CancellationToken cancellationToken = default);
    Task<SourceFormat> FindOrCreateSourceFormatAsync(string formatName, CancellationToken cancellationToken = default);
    Task<VinylState> FindOrCreateVinylStateAsync(string stateName, CancellationToken cancellationToken = default);
    Task<Artist> FindOrCreateArtistAsync(string artistName, CancellationToken cancellationToken = default);
    Task<Genre> FindOrCreateGenreAsync(string genreName, CancellationToken cancellationToken = default);
    Task<Equipment.Player> FindOrCreatePlayerAsync(string playerName, string? manufacturerName = null, CancellationToken cancellationToken = default);
    Task<Equipment.Cartridge> FindOrCreateCartridgeAsync(string cartridgeName, string? manufacturerName = null, CancellationToken cancellationToken = default);
    Task<Equipment.Amplifier> FindOrCreateAmplifierAsync(string amplifierName, string? manufacturerName = null, CancellationToken cancellationToken = default);
    Task<Equipment.Adc> FindOrCreateAdcAsync(string adcName, string? manufacturerName = null, CancellationToken cancellationToken = default);
    Task<Equipment.Wire> FindOrCreateWireAsync(string wireName, string? manufacturerName = null, CancellationToken cancellationToken = default);
    Task<Equipment.Manufacturer?> FindOrCreateManufacturerAsync(string manufacturerName, CancellationToken cancellationToken = default);
}
