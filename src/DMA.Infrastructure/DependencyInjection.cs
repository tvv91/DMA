using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DMA.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Stage 3: DbContext, repositories, file storage, SignalR notifiers
        _ = configuration;
        return services;
    }
}
