using DMA.Application.ReferenceData;
using MediatR;
using Web.Interfaces;

namespace Web.Services;

public class EntityFindOrCreateService(IMediator mediator) : IEntityFindOrCreateService
{
    public Task<Year> FindOrCreateYearAsync(int yearValue) =>
        mediator.Send(new FindOrCreateYearCommand(yearValue));

    public Task<Reissue> FindOrCreateReissueAsync(int reissueValue) =>
        mediator.Send(new FindOrCreateReissueCommand(reissueValue));

    public Task<Country> FindOrCreateCountryAsync(string countryName) =>
        mediator.Send(new FindOrCreateCountryCommand(countryName));

    public Task<Label> FindOrCreateLabelAsync(string labelName) =>
        mediator.Send(new FindOrCreateLabelCommand(labelName));

    public Task<Storage> FindOrCreateStorageAsync(string storageData) =>
        mediator.Send(new FindOrCreateStorageCommand(storageData));

    public Task<Bitness> FindOrCreateBitnessAsync(int bitnessValue) =>
        mediator.Send(new FindOrCreateBitnessCommand(bitnessValue));

    public Task<Sampling> FindOrCreateSamplingAsync(double samplingValue) =>
        mediator.Send(new FindOrCreateSamplingCommand(samplingValue));

    public Task<DigitalFormat> FindOrCreateDigitalFormatAsync(string formatName) =>
        mediator.Send(new FindOrCreateDigitalFormatCommand(formatName));

    public Task<SourceFormat> FindOrCreateSourceFormatAsync(string formatName) =>
        mediator.Send(new FindOrCreateSourceFormatCommand(formatName));

    public Task<VinylState> FindOrCreateVinylStateAsync(string stateName) =>
        mediator.Send(new FindOrCreateVinylStateCommand(stateName));

    public Task<Player> FindOrCreatePlayerAsync(string playerName, string? manufacturerName = null) =>
        mediator.Send(new FindOrCreatePlayerCommand(playerName, manufacturerName));

    public Task<Cartridge> FindOrCreateCartridgeAsync(string cartridgeName, string? manufacturerName = null) =>
        mediator.Send(new FindOrCreateCartridgeCommand(cartridgeName, manufacturerName));

    public Task<Amplifier> FindOrCreateAmplifierAsync(string amplifierName, string? manufacturerName = null) =>
        mediator.Send(new FindOrCreateAmplifierCommand(amplifierName, manufacturerName));

    public Task<Adc> FindOrCreateAdcAsync(string adcName, string? manufacturerName = null) =>
        mediator.Send(new FindOrCreateAdcCommand(adcName, manufacturerName));

    public Task<Wire> FindOrCreateWireAsync(string wireName, string? manufacturerName = null) =>
        mediator.Send(new FindOrCreateWireCommand(wireName, manufacturerName));

    public Task<Manufacturer?> FindOrCreateManufacturerAsync(string manufacturerName) =>
        mediator.Send(new FindOrCreateManufacturerCommand(manufacturerName));
}
