using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Lucy.Console.Internal;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace Lucy.Console.Middleware;

/// <summary>
/// A middleware that handles errors and exceptions.
/// </summary>
public class ErrorHandlerMiddleware(
    ILogger<ErrorHandlerMiddleware> logger) : ICommandMiddleware
{
    private readonly ILogger<ErrorHandlerMiddleware> _logger = logger;

    /// <inheritdoc/>
    public async Task<ExitCode> InvokeAsync<TCommand>(
        CommandContext context,
        TCommand command,
        CommandDelegate<TCommand> next,
        CancellationToken token = default)
        where TCommand : CommandSettings
    {
        var name = command.GetType().Name;
        try
        {
            return await next(context, command, token);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Command execution was canceled.");
            return ExitCode.Canceled;
        }
        catch (ValidationException)
        {
            _logger.LogWarning("Command was invalid.");
            return ExitCode.Invalid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while executing the command {command}", name);
            return ExitCode.Error;
        }
    }
}
