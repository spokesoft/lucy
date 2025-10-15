using System.Reflection;
using Lucy.Console.Interfaces;
using Lucy.Console.Internal;
using Lucy.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Lucy.Console.Extensions;

/// <summary>
/// Extension methods for registering commands.
/// </summary>
public static class CommandExtensions
{
    /// <summary>
    /// Registers all commands handlers and validators.
    /// </summary>
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services
            .AddSingleton<ICommandExecutor, CommandExecutor>()
            .AddCommandHandlersFromAssembly(assembly)
            .AddCommandValidatorsFromAssembly(assembly)
            .AddCommandMiddlewareFromAssembly(assembly);

        return services;
    }

    /// <summary>
    /// Registers all command handlers from the specified assembly.
    /// </summary>
    public static IServiceCollection AddCommandHandlersFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
        => services.AddImplementingTypesFromAssembly(
            [
                typeof(ICommandHandler<>),
            ],
            assembly,
            ServiceLifetime.Scoped);

    /// <summary>
    /// Registers all command validators from the specified assembly.
    /// </summary>
    public static IServiceCollection AddCommandValidatorsFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
        => services.AddImplementingTypesFromAssembly(
            [
                typeof(ICommandValidator<>),
            ],
            assembly,
            ServiceLifetime.Scoped);

    /// <summary>
    /// Registers all command validators from the specified assembly.
    /// </summary>
    public static IServiceCollection AddCommandMiddlewareFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
        => services.AddTypesFromAssembly(
            typeof(ICommandMiddleware),
            assembly,
            ServiceLifetime.Scoped);

}
