using System.Reflection;
using Lucy.Application.Interfaces;
using Lucy.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Lucy.Infrastructure.Mediation;

/// <summary>
/// Extension methods for setting up mediation services.
/// </summary>
public static class MediationExtensions
{
    /// <summary>
    /// Adds mediation services to the service collection.
    /// </summary>
    public static IServiceCollection AddMediation(this IServiceCollection services)
    {
        var application = typeof(IRequest<>).Assembly;

        services
            .AddSingleton<IMediator, Mediator>()
            .AddRequestHandlersFromAssembly(application)
            .AddRequestValidatorsFromAssembly(application);

        return services;
    }

    /// <summary>
    /// Scans the specified assembly for IRequestHandler implementations and
    /// registers them with the service collection.
    /// </summary>
    public static IServiceCollection AddRequestHandlersFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
        => services.AddImplementingTypesFromAssembly(
            [
                typeof(IRequestHandler<>),
                typeof(IRequestHandler<,>)
            ],
            assembly,
            ServiceLifetime.Scoped);

    /// <summary>
    /// Scans the specified assembly for IRequestValidator implementations and
    /// registers them with the service collection.
    /// </summary>
    public static IServiceCollection AddRequestValidatorsFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
        => services.AddImplementingTypesFromAssembly(
            [typeof(IRequestValidator<>)],
            assembly,
            ServiceLifetime.Scoped);
}
