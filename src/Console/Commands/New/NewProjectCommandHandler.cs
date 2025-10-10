using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Handler for the <see cref="NewProjectCommand"/> command.
/// </summary>
public class NewProjectCommandHandler : ICommandHandler<NewProjectCommand>
{
    /// <inheritdoc />
    public Task<ExitCode> HandleAsync(
        CommandContext context,
        NewProjectCommand command,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
