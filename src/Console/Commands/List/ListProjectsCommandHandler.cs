using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;
using Lucy.Application.Projects.Queries.ListProjects;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Handler for the <see cref="ListProjectsCommand"/> command.
/// </summary>
internal class ListProjectsCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IViewRenderer<IEnumerable<ProjectDto>> viewRenderer,
    IMediator mediator) : ICommandHandler<ListProjectsCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IViewRenderer<IEnumerable<ProjectDto>> _view = viewRenderer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ListProjectsCommand command,
        CancellationToken token = default)
    {
        var query = new ListProjectsQuery();
        var projects = await _mediator.Send(query, token);

        await _view.RenderAsync(projects, _console, _localizer, token);
        return ExitCode.Success;
    }
}
