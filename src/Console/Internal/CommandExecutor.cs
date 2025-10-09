using Lucy.Application.Interfaces;
using Lucy.Console.Interfaces;
using Lucy.Infrastructure.Logging.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

/// <summary>
/// Defines methods to execute commands.
/// </summary>
namespace Lucy.Console.Internal;

internal interface ICommandExecutor
{
    Task<int> ExecuteAsync<TCommand>(CommandContext context, TCommand settings)
        where TCommand : CommandSettings;
}

/// <summary>
/// Executes commands by resolving them from the service provider.
/// </summary>
internal class CommandExecutor(
    IServiceProvider provider) : ICommandExecutor
{
    /// <summary>
    /// The service provider to resolve commands from.
    /// </summary>
    private readonly IServiceProvider _provider = provider;

    /// <summary>
    /// Executes the specified command by resolving it from the service provider.
    /// </summary>
    public Task<int> ExecuteAsync<TCommand>(CommandContext context, TCommand settings)
        where TCommand : CommandSettings
    {
        using var scope = _provider.CreateScope();

        var migrators = scope.ServiceProvider.GetServices<IDatabaseMigrator>();
        var logging = scope.ServiceProvider.GetRequiredService<IDatabaseLoggingService>();

        var validators = scope.ServiceProvider.GetServices(
            typeof(ICommandValidator<>).MakeGenericType(typeof(TCommand)))
            as IEnumerable<ICommandValidator<TCommand>> ?? [];

        var handler = scope.ServiceProvider.GetRequiredService(
            typeof(ICommandHandler<>).MakeGenericType(typeof(TCommand)))
            as ICommandHandler<TCommand>
            ?? throw new InvalidOperationException(
                $"No handler registered for command type {typeof(TCommand).FullName}");

        // Build and run the command pipeline
        return CommandPipeline.Create(context, settings)
            .UseDatabaseMigration(migrators)
            .UseLogging(logging)
            .UseValidation(validators)
            .UseHandler(handler)
            .RunAsync();
    }
}
