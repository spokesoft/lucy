using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Handler for the <see cref="UpdateProjectCommand"/> command.
/// </summary>
public class UpdateProjectCommandHandler : ICommandHandler<UpdateProjectCommand>
{
    /// <inheritdoc />
    public Task<ExitCode> HandleAsync(
        CommandContext context,
        UpdateProjectCommand command,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
