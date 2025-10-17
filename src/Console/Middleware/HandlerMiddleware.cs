using System.Diagnostics;
using Lucy.Console.Enums;
using Lucy.Console.Extensions;
using Lucy.Console.Interfaces;
using Lucy.Console.Internal;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Lucy.Console.Middleware;

/// <summary>
/// A middleware that handles command execution.
/// </summary>
internal class HandlerMiddleware(
    IServiceProvider services,
    ILogger<HandlerMiddleware> logger,
    IStringLocalizer<Program> localizer) : ICommandMiddleware
{
    private readonly IServiceProvider _services = services;
    private readonly ILogger<HandlerMiddleware> _logger = logger;
    private readonly IStringLocalizer<Program> _localizer = localizer;

    /// <inheritdoc/>
    public async Task<ExitCode> InvokeAsync<TCommand>(
        CommandContext context,
        TCommand command,
        CommandDelegate<TCommand> next,
        CancellationToken token = default)
        where TCommand : CommandSettings
    {
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = _services.GetService(handlerType)
            as ICommandHandler<TCommand>
            ?? throw new InvalidOperationException($"No handler found for command type {command.GetType().Name}");

        var sw = Stopwatch.StartNew();
        var handlerName = handler.GetType().Name;
        var commandName = command.GetType().Name;

        var result = await handler.HandleAsync(context, command, token);

        sw.Stop();
        _logger.LogDebug(
            _localizer,
            "Messages.CommandHandled",
            commandName,
            handlerName,
            sw.ElapsedMilliseconds);

        return result;
    }
}
