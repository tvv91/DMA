using DMA.Domain.Equipment;
using DMA.Domain.ReferenceData;
using MediatR;

namespace DMA.Application.ReferenceData;

public sealed record FindOrCreateYearCommand(int YearValue) : IRequest<Year>;
public sealed class FindOrCreateYearCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateYearCommand, Year>
{
    public Task<Year> Handle(FindOrCreateYearCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateYearAsync(request.YearValue, cancellationToken);
}

public sealed record FindOrCreateReissueCommand(int ReissueValue) : IRequest<Reissue>;
public sealed class FindOrCreateReissueCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateReissueCommand, Reissue>
{
    public Task<Reissue> Handle(FindOrCreateReissueCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateReissueAsync(request.ReissueValue, cancellationToken);
}

public sealed record FindOrCreateCountryCommand(string CountryName) : IRequest<Country>;
public sealed class FindOrCreateCountryCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateCountryCommand, Country>
{
    public Task<Country> Handle(FindOrCreateCountryCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateCountryAsync(request.CountryName, cancellationToken);
}

public sealed record FindOrCreateLabelCommand(string LabelName) : IRequest<Label>;
public sealed class FindOrCreateLabelCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateLabelCommand, Label>
{
    public Task<Label> Handle(FindOrCreateLabelCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateLabelAsync(request.LabelName, cancellationToken);
}

public sealed record FindOrCreateStorageCommand(string StorageData) : IRequest<Storage>;
public sealed class FindOrCreateStorageCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateStorageCommand, Storage>
{
    public Task<Storage> Handle(FindOrCreateStorageCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateStorageAsync(request.StorageData, cancellationToken);
}

public sealed record FindOrCreateBitnessCommand(int BitnessValue) : IRequest<Bitness>;
public sealed class FindOrCreateBitnessCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateBitnessCommand, Bitness>
{
    public Task<Bitness> Handle(FindOrCreateBitnessCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateBitnessAsync(request.BitnessValue, cancellationToken);
}

public sealed record FindOrCreateSamplingCommand(double SamplingValue) : IRequest<Sampling>;
public sealed class FindOrCreateSamplingCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateSamplingCommand, Sampling>
{
    public Task<Sampling> Handle(FindOrCreateSamplingCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateSamplingAsync(request.SamplingValue, cancellationToken);
}

public sealed record FindOrCreateDigitalFormatCommand(string FormatName) : IRequest<DigitalFormat>;
public sealed class FindOrCreateDigitalFormatCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateDigitalFormatCommand, DigitalFormat>
{
    public Task<DigitalFormat> Handle(FindOrCreateDigitalFormatCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateDigitalFormatAsync(request.FormatName, cancellationToken);
}

public sealed record FindOrCreateSourceFormatCommand(string FormatName) : IRequest<SourceFormat>;
public sealed class FindOrCreateSourceFormatCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateSourceFormatCommand, SourceFormat>
{
    public Task<SourceFormat> Handle(FindOrCreateSourceFormatCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateSourceFormatAsync(request.FormatName, cancellationToken);
}

public sealed record FindOrCreateVinylStateCommand(string StateName) : IRequest<VinylState>;
public sealed class FindOrCreateVinylStateCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateVinylStateCommand, VinylState>
{
    public Task<VinylState> Handle(FindOrCreateVinylStateCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateVinylStateAsync(request.StateName, cancellationToken);
}

public sealed record FindOrCreatePlayerCommand(string PlayerName, string? ManufacturerName = null) : IRequest<Player>;
public sealed class FindOrCreatePlayerCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreatePlayerCommand, Player>
{
    public Task<Player> Handle(FindOrCreatePlayerCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreatePlayerAsync(request.PlayerName, request.ManufacturerName, cancellationToken);
}

public sealed record FindOrCreateCartridgeCommand(string CartridgeName, string? ManufacturerName = null) : IRequest<Cartridge>;
public sealed class FindOrCreateCartridgeCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateCartridgeCommand, Cartridge>
{
    public Task<Cartridge> Handle(FindOrCreateCartridgeCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateCartridgeAsync(request.CartridgeName, request.ManufacturerName, cancellationToken);
}

public sealed record FindOrCreateAmplifierCommand(string AmplifierName, string? ManufacturerName = null) : IRequest<Amplifier>;
public sealed class FindOrCreateAmplifierCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateAmplifierCommand, Amplifier>
{
    public Task<Amplifier> Handle(FindOrCreateAmplifierCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateAmplifierAsync(request.AmplifierName, request.ManufacturerName, cancellationToken);
}

public sealed record FindOrCreateAdcCommand(string AdcName, string? ManufacturerName = null) : IRequest<Adc>;
public sealed class FindOrCreateAdcCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateAdcCommand, Adc>
{
    public Task<Adc> Handle(FindOrCreateAdcCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateAdcAsync(request.AdcName, request.ManufacturerName, cancellationToken);
}

public sealed record FindOrCreateWireCommand(string WireName, string? ManufacturerName = null) : IRequest<Wire>;
public sealed class FindOrCreateWireCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateWireCommand, Wire>
{
    public Task<Wire> Handle(FindOrCreateWireCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateWireAsync(request.WireName, request.ManufacturerName, cancellationToken);
}

public sealed record FindOrCreateManufacturerCommand(string ManufacturerName) : IRequest<Manufacturer?>;
public sealed class FindOrCreateManufacturerCommandHandler(IReferenceDataRepository repository) : IRequestHandler<FindOrCreateManufacturerCommand, Manufacturer?>
{
    public Task<Manufacturer?> Handle(FindOrCreateManufacturerCommand request, CancellationToken cancellationToken) =>
        repository.FindOrCreateManufacturerAsync(request.ManufacturerName, cancellationToken);
}
