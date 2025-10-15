using Spectre.Console.Cli;

/// <summary>
/// Defines methods to execute commands.
/// </summary>
namespace Lucy.Console.Interfaces;

internal interface ICommandExecutor
{
    Task<int> ExecuteAsync<TCommand>(CommandContext context, TCommand settings)
        where TCommand : CommandSettings;
}
