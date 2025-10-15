using Lucy.Console.Enums;
using Spectre.Console.Cli;

namespace Lucy.Console.Internal;

/// <summary>
/// A delegate that represents a command middleware execution.
/// </summary>
public delegate Task<ExitCode> CommandMiddlewareDelegate<TCommand>(
    CommandContext context,
    TCommand command,
    CommandDelegate<TCommand> next,
    CancellationToken token = default);
