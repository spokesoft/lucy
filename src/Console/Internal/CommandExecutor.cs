using Lucy.Console.Interfaces;
using Lucy.Console.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

/// <summary>
/// Defines methods to execute commands.
/// </summary>
namespace Lucy.Console.Internal;

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
    public async Task<int> ExecuteAsync<TCommand>(CommandContext context, TCommand settings)
        where TCommand : CommandSettings
    {
        var scope = _provider.CreateScope();
        var cts = new CancellationTokenSource();

        void HandleCancel(object? sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            cts.Cancel();
        };

        System.Console.CancelKeyPress += HandleCancel;

        try
        {
            var pipeline = CommandPipelineBuilder<TCommand>.Create(scope.ServiceProvider)
                .Use<ErrorHandlerMiddleware>()
                .Use<MigrationsMiddleware>()
                .Use<LoggingMiddleware>()
                .Use<ValidationMiddleware>()
                .Use<HandlerMiddleware>()
                .Build();

            return await pipeline.RunAsync(context, settings, cts.Token);
        }
        finally
        {
            System.Console.CancelKeyPress -= HandleCancel;
            scope.Dispose();
            cts.Dispose();
        }
    }
}
