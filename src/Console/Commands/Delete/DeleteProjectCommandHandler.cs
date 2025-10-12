using AppDeleteProjectCommand = Lucy.Application.Projects.Commands.DeleteProject.DeleteProjectCommand;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Interfaces;
using Spectre.Console;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Handler for the <see cref="DeleteProjectCommand"/> command.
/// </summary>
public class DeleteProjectCommandHandler(
    IAnsiConsole console,
    IMediator mediator) : ICommandHandler<DeleteProjectCommand>
{
    /// <summary>
    /// The console instance for outputting information.
    /// </summary>
    private readonly IAnsiConsole _console = console;

    /// <summary>
    /// The mediator instance for sending commands and queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        DeleteProjectCommand command,
        CancellationToken token = default)
    {
        var projectId = command.Id;
        if (projectId is null)
        {
            var query = new GetProjectIdByKeyQuery(command.Key!);
            projectId = await _mediator.Send(query, token);
        }

        var request = new AppDeleteProjectCommand(projectId!.Value);
        await _mediator.Send(request, token);

        if (command.Key is not null)
            _console.MarkupLine("[green]✓[/] Deleted project [yellow]{0}[/] [gray]ID={1}[/]", command.Key, projectId);
        else
            _console.MarkupLine("[green]✓[/] Deleted project [gray]ID={0}[/]", projectId);

        return ExitCode.Success;
    }
}
