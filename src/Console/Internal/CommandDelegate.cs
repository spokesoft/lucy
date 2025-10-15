using Lucy.Console.Enums;
using Spectre.Console.Cli;

namespace Lucy.Console.Internal;

/// <summary>
/// A delegate that represents a command execution.
/// </summary>
public delegate Task<ExitCode> CommandDelegate<TCommand>(
    CommandContext context,
    TCommand command,
    CancellationToken token = default);
