using System.Diagnostics;
using Lucy.Console.Enums;
using Lucy.Console.Extensions;
using Lucy.Console.Interfaces;
using Lucy.Console.Internal;
using Lucy.Infrastructure.Logging.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Lucy.Console.Middleware;

/// <summary>
/// A middleware that starts and stops the background logging service.
/// </summary>
internal class LoggingMiddleware(
    IServiceProvider services,
    ILogger<LoggingMiddleware> logger,
    IStringLocalizer<Program> localizer) : ICommandMiddleware
{
    private readonly IServiceProvider _services = services;
    private readonly ILogger<LoggingMiddleware> _logger = logger;
    private readonly IStringLocalizer<Program> _localizer = localizer;

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

        _logger.LogDebug(_localizer, "Messages.LoggingServiceStarted");

        try
        {
            var result = await next(context, command, token);
            sw.Stop();

            _logger.LogDebug(
                _localizer,
                "Messages.CommandExecuted",
                command.GetType().Name,
                result.ToString(),
                sw.ElapsedMilliseconds);

            return result;
        }
        finally
        {
            await logging.StopAsync((count, elapsed)
                => _localizer.GetString("Messages.LoggingServiceStopped", count, elapsed));
        }
    }
}
