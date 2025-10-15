using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Lucy.Infrastructure.Extensions;

/// <summary>
/// Extension methods for IServiceCollection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Scans the specified assembly for types implementing the given interface
    /// types and registers them with the service collection.
    /// </summary>
    public static IServiceCollection AddImplementingTypesFromAssembly(
        this IServiceCollection services,
        IEnumerable<Type> interfaceTypes,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var types = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
            .Where(i => i.IsGenericType && interfaceTypes.Contains(i.GetGenericTypeDefinition()))
            .Select(i => new { ImplementationType = t, InterfaceType = i }));

        foreach (var type in types)
        {
            services.Add(new ServiceDescriptor(type.InterfaceType, type.ImplementationType, lifetime));
        }

        return services;
    }

    /// <summary>
    /// Scans the specified assembly for all non-abstract, non-interface types
    /// that implement the given interface and registers them with the service
    /// collection.
    /// </summary>
    public static IServiceCollection AddTypesFromAssembly(
        this IServiceCollection services,
        Type interfaceType,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var types = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && t.GetInterfaces().Contains(interfaceType));

        foreach (var type in types)
        {
            services.Add(new ServiceDescriptor(type, type, lifetime));
        }

        return services;
    }
}
