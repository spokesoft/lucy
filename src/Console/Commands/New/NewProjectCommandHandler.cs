using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Commands.CreateProject;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Handler for the <see cref="NewProjectCommand"/> command.
/// </summary>
public class NewProjectCommandHandler(
    IAnsiConsole console,
    IMediator mediator) : ICommandHandler<NewProjectCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        NewProjectCommand command,
        CancellationToken token = default)
    {
        var request = new CreateProjectCommand(
            command.Key,
            command.Name,
            command.Description);

        var id = await _mediator.Send(request, token);

        _console.MarkupLine("[green]✓[/] Created project [blue]{0}[/]. [gray]ID: {1}[/]", command.Key, id);

        return ExitCode.Success;
    }
}
