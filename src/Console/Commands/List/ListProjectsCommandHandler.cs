using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Handler for the <see cref="ListProjectsCommand"/> command.
/// </summary>
public class ListProjectsCommandHandler : ICommandHandler<ListProjectsCommand>
{
    /// <inheritdoc />
    public Task<ExitCode> HandleAsync(
        CommandContext context,
        ListProjectsCommand command,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
