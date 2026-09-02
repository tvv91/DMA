using DMA.Application;
using DMA.Domain.Albums;
using DMA.Domain.Common;
using DMA.Domain.Equipment;
using DMA.Domain.Posts;
using DMA.Domain.ReferenceData;
using DMA.Domain.Statistics;
using DMA.Infrastructure.Persistence;
using DMA.Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Web.Db;

namespace Web.Tests.Helpers;

internal sealed class TestMediatorContext : IMediator, IDisposable
{
    private readonly ServiceProvider _provider;

    public TestMediatorContext(Context context, TimeProvider? timeProvider = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton<DmaDbContext>(context);
        services.AddSingleton(timeProvider ?? TimeProvider.System);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IStatisticRepository, StatisticRepository>();
        services.AddScoped<ISearchRepository, SearchRepository>();
        services.AddScoped<IReferenceDataRepository, ReferenceDataRepository>();
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<IReleaseRepository, ReleaseRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();

        services.AddApplication();

        _provider = services.BuildServiceProvider();
    }

    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        using var scope = _provider.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request, cancellationToken);
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        using var scope = _provider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request, cancellationToken);
    }

    public async Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        using var scope = _provider.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<IMediator>().Send(request, cancellationToken);
    }

    public async IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequest<TResponse> request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var scope = _provider.CreateScope();
        await foreach (var item in scope.ServiceProvider
                           .GetRequiredService<IMediator>()
                           .CreateStream(request, cancellationToken))
        {
            yield return item;
        }
    }

    public async IAsyncEnumerable<object?> CreateStream(
        object request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var scope = _provider.CreateScope();
        await foreach (var item in scope.ServiceProvider
                           .GetRequiredService<IMediator>()
                           .CreateStream(request, cancellationToken))
        {
            yield return item;
        }
    }

    public async Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        using var scope = _provider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IMediator>().Publish(notification, cancellationToken);
    }

    public async Task Publish<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        using var scope = _provider.CreateScope();
        await scope.ServiceProvider.GetRequiredService<IMediator>().Publish(notification, cancellationToken);
    }

    public void Dispose() => _provider.Dispose();
}

internal static class MediatorTestHelper
{
    public static TestMediatorContext Create(Context context, TimeProvider? timeProvider = null) =>
        new(context, timeProvider);
}
