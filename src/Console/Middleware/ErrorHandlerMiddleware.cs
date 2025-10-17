using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Lucy.Console.Internal;
using Lucy.Console;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using Lucy.Console.Extensions;

namespace Lucy.Console.Middleware;

/// <summary>
/// A middleware that handles errors and exceptions.
/// </summary>
internal class ErrorHandlerMiddleware(
    ILogger<ErrorHandlerMiddleware> logger,
    IStringLocalizer<Program> localizer) : ICommandMiddleware
{
    private readonly ILogger<ErrorHandlerMiddleware> _logger = logger;
    private readonly IStringLocalizer<Program> _localizer = localizer;

    /// <inheritdoc/>
    public async Task<ExitCode> InvokeAsync<TCommand>(
        CommandContext context,
        TCommand command,
        CommandDelegate<TCommand> next,
        CancellationToken token = default)
        where TCommand : CommandSettings
    {
        try
        {
            _logger.LogDebug(
                _localizer,
                "Messages.CommandStarting",
                command.GetType().Name,
                DateTime.UtcNow,
                Environment.UserName);

            return await next(context, command, token);
        }
        catch (OperationCanceledException)
        {
            return ExitCode.Canceled;
        }
        catch (ValidationException)
        {
            return ExitCode.Invalid;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex);
            _logger.LogError(
                ex,
                _localizer,
                "Messages.CommandError",
                ex.Message);

            return ExitCode.Error;
        }
    }
}
