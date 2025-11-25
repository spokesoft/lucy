using AppUpdateProjectCommand = Lucy.Application.Projects.Commands.UpdateProject.UpdateProjectCommand;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;
using Spectre.Console;
using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Microsoft.Extensions.Localization;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Handler for the <see cref="UpdateProjectCommand"/> command.
/// </summary>
internal class UpdateProjectCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<UpdateProjectCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
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
            command.NewKey,
            command.Name,
            command.Description,
            command.CascadeRename);

        await _mediator.Send(request, token);


        var updatedKey = command.NewKey ?? command.Key;
        var keyOrId = updatedKey ?? projectId?.ToString() ?? "";
        var message = _localizer["Messages.UpdatedProject", keyOrId];
        _console.MarkupLine("[green]:check_mark:[/] " + message);

        return ExitCode.Success;
    }
}
