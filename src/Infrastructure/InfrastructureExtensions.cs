using Lucy.Infrastructure.Database;
using Lucy.Infrastructure.Logging;
using Lucy.Infrastructure.Mediation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lucy.Infrastructure;

/// <summary>
/// Extension methods for setting up the infrastructure services.
/// </summary>
public static class InfrastructureExtensions
{
    /// <summary>
    /// Adds the infrastructure services.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDatabase(configuration)
            .AddLogging(configuration)
            .AddMediation();

        return services;
    }
}
