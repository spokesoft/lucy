using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;
using Lucy.Application.Projects.Queries.GetProjectById;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Handler for the <see cref="ShowProjectCommand"/> command.
/// </summary>
internal class ShowProjectCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IViewRenderer<ProjectDto> viewRenderer,
    IMediator mediator) : ICommandHandler<ShowProjectCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IViewRenderer<ProjectDto> _view = viewRenderer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ShowProjectCommand command,
        CancellationToken token = default)
    {
        var projectId = command.Id;
        if (projectId is null)
        {
            var getId = new GetProjectIdByKeyQuery(command.Key!);
            projectId = await _mediator.Send(getId, token);
        }
        var query = new GetProjectByIdQuery(projectId!.Value);
        var project = await _mediator.Send(query, token);

        if (project is null)
        {
            _console.MarkupLine($"[red]{_localizer["Error.Project.NotFound"]}[/]");
            return ExitCode.Error;
        }

        await _view.RenderAsync(project, _console, _localizer, token);
        return ExitCode.Success;
    }
}
