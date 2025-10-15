using Lucy.Console.Enums;
using Spectre.Console.Cli;

namespace Lucy.Console.Internal;

/// <summary>
/// A command pipeline that executes a series of middlewares.
/// </summary>
internal class CommandPipeline<TCommand>(CommandDelegate<TCommand> pipeline)
    where TCommand : CommandSettings
{
    private readonly CommandDelegate<TCommand> _pipeline = pipeline;

    /// <summary>
    /// Runs the command pipeline asynchronously.
    /// </summary>
    public async Task<int> RunAsync(CommandContext context, TCommand command, CancellationToken token = default)
        => (int)await _pipeline(context, command, token);
}
