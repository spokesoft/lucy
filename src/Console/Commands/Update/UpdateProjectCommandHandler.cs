using AppUpdateProjectCommand = Lucy.Application.Projects.Commands.UpdateProject.UpdateProjectCommand;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;
using Spectre.Console;
using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Handler for the <see cref="UpdateProjectCommand"/> command.
/// </summary>
public class UpdateProjectCommandHandler(
    IAnsiConsole console,
    IMediator mediator) : ICommandHandler<UpdateProjectCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        UpdateProjectCommand command,
        CancellationToken token = default)
    {
        var projectId = command.Id;
        if (projectId is null)
        {
            var query = new GetProjectIdByKeyQuery(command.Key!);
            projectId = await _mediator.Send(query, token);
        }

        var request = new AppUpdateProjectCommand(
            projectId!.Value,
            command.Key,
            command.Name,
            command.Description);

        await _mediator.Send(request, token);

        if (command.Key is null)
            _console.MarkupLine("[green]:check_mark:[/] Project [blue]{0}[/] successfully updated", projectId);
        else
            _console.MarkupLine("[green]:check_mark:[/] Project [blue]{0}[/] successfully updated.", command.Key);

        return ExitCode.Success;
    }
}
