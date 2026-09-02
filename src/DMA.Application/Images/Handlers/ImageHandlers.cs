using DMA.Application.Images;
using DMA.Domain.Common;
using MediatR;

namespace DMA.Application.Images.Handlers;

public sealed record GetImageUrlQuery(int Id, EntityType Entity) : IRequest<string>;
public sealed class GetImageUrlQueryHandler(IImageStorage imageStorage) : IRequestHandler<GetImageUrlQuery, string>
{
    public Task<string> Handle(GetImageUrlQuery request, CancellationToken cancellationToken) =>
        imageStorage.GetUrlAsync(request.Id, request.Entity, cancellationToken);
}

public sealed record SaveImageCommand(int Id, string Filename, EntityType Entity) : IRequest;
public sealed class SaveImageCommandHandler(IImageStorage imageStorage) : IRequestHandler<SaveImageCommand>
{
    public Task Handle(SaveImageCommand request, CancellationToken cancellationToken) =>
        imageStorage.SaveAsync(request.Id, request.Filename, request.Entity, cancellationToken);
}

public sealed record RemoveImageCommand(int Id, EntityType Entity) : IRequest;
public sealed class RemoveImageCommandHandler(IImageStorage imageStorage) : IRequestHandler<RemoveImageCommand>
{
    public Task Handle(RemoveImageCommand request, CancellationToken cancellationToken) =>
        imageStorage.RemoveAsync(request.Id, request.Entity, cancellationToken);
}

public sealed record GetResourceIconUrlQuery(int Id, EntityType Entity) : IRequest<string>;
public sealed class GetResourceIconUrlQueryHandler(IResourceIconService resourceIconService) : IRequestHandler<GetResourceIconUrlQuery, string>
{
    public Task<string> Handle(GetResourceIconUrlQuery request, CancellationToken cancellationToken) =>
        resourceIconService.GetIconUrlAsync(request.Id, request.Entity, cancellationToken);
}
