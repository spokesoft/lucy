using Lucy.Console.Enums;
using Lucy.Console.Internal;
using Spectre.Console.Cli;

namespace Lucy.Console.Interfaces;

/// <summary>
/// A middleware for command execution.
/// </summary>
public interface ICommandMiddleware
{
    /// <summary>
    /// Executes the middleware.
    /// </summary>
    Task<ExitCode> InvokeAsync<TCommand>(
        CommandContext context,
        TCommand command,
        CommandDelegate<TCommand> next,
        CancellationToken token = default)
        where TCommand : CommandSettings;
}
