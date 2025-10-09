using Lucy.Console.Enums;
using Spectre.Console.Cli;

namespace Lucy.Console.Interfaces;

/// <summary>
/// A handler for a specific command.
/// </summary>
public interface ICommandHandler<TCommand>
    where TCommand : CommandSettings
{
    /// <summary>
    /// Handles the command asynchronously.
    /// </summary>
    Task<ExitCode> HandleAsync(CommandContext context, TCommand command, CancellationToken token = default);
}
