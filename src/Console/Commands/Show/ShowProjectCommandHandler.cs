using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Handler for the <see cref="ShowProjectCommand"/> command.
/// </summary>
public class ShowProjectCommandHandler : ICommandHandler<ShowProjectCommand>
{
    /// <inheritdoc />
    public Task<ExitCode> HandleAsync(
        CommandContext context,
        ShowProjectCommand command,
        CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
