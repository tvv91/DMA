using DMA.Application.Images;
using DMA.Domain.Albums;
using DMA.Domain.Common;
using DMA.Domain.Equipment;
using DMA.Domain.Posts;
using DMA.Domain.ReferenceData;
using DMA.Domain.Statistics;
using DMA.Infrastructure.Images;
using DMA.Infrastructure.Persistence;
using DMA.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DMA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:DbConnectionDev"];
        services.AddDbContext<DmaDbContext>(opts =>
        {
            opts.UseSqlServer(connectionString, sqlServerOptionsAction =>
            {
                sqlServerOptionsAction.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
            });
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IStatisticRepository, StatisticRepository>();
        services.AddScoped<ISearchRepository, SearchRepository>();
        services.AddScoped<IReferenceDataRepository, ReferenceDataRepository>();
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<IReleaseRepository, ReleaseRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IEquipmentRepository, EquipmentRepository>();
        services.AddScoped<IImageStorage, LocalStorageImageService>();
        services.AddScoped<IResourceIconService, LocalResourceIconService>();

        return services;
    }
}
