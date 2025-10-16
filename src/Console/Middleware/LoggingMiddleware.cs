using System.Diagnostics;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Lucy.Console.Internal;
using Lucy.Infrastructure.Logging.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Lucy.Console.Middleware;

/// <summary>
/// A middleware that starts and stops the background logging service.
/// </summary>
public class LoggingMiddleware(
    IServiceProvider services,
    ILogger<LoggingMiddleware> logger) : ICommandMiddleware
{
    private readonly IServiceProvider _services = services;
    private readonly ILogger<LoggingMiddleware> _logger = logger;

    /// <inheritdoc/>
    public async Task<ExitCode> InvokeAsync<TCommand>(
        CommandContext context,
        TCommand command,
        CommandDelegate<TCommand> next,
        CancellationToken token = default)
        where TCommand : CommandSettings
    {
        var logging = _services.GetRequiredService<IDatabaseLoggingService>();
        var start = DateTime.UtcNow;
        var sw = Stopwatch.StartNew();
        logging.Start(token);

        _logger.LogDebug("Started background logging service.");

        try
        {
            _logger.LogDebug(
                "Command {command} execution started at {start:yyyy-MM-dd HH:mm:ss} UTC by {user}",
                command.GetType().Name,
                start,
                Environment.UserName);

            var result = await next(context, command, token);
            sw.Stop();

            _logger.LogDebug(
                "Command {command} completed with {exitcode} in {elapsed}ms",
                command.GetType().Name,
                result,
                sw.ElapsedMilliseconds);

            return result;
        }
        finally
        {
            await logging.StopAsync((count, elapsed)
                => $"Background logging service wrote {count} entries stopped after {elapsed}ms.");
        }
    }
}
