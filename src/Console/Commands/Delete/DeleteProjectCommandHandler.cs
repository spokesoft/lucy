using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Handler for the <see cref="DeleteProjectCommand"/> command.
/// </summary>
public class DeleteProjectCommandHandler : ICommandHandler<DeleteProjectCommand>
{
    /// <inheritdoc />
    public Task<ExitCode> HandleAsync(
        CommandContext context,
        DeleteProjectCommand command,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
