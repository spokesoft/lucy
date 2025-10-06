using Lucy.Infrastructure.Logging;
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
        services.AddLogging(configuration);
        return services;
    }
}
