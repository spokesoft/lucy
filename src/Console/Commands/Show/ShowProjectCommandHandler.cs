using Lucy.Application.Comments.DTOs;
using Lucy.Application.Comments.Queries.ListProjectComments;
using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.DTOs;
using Lucy.Application.Projects.Queries.GetProjectById;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.GetTicketCountsByProjectId;
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
    IViewRenderer<(ProjectDto, IEnumerable<CommentDto>, IEnumerable<TicketCountByStatusDto>)> viewRenderer,
    IMediator mediator) : ICommandHandler<ShowProjectCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IViewRenderer<(ProjectDto, IEnumerable<CommentDto>, IEnumerable<TicketCountByStatusDto>)> _view = viewRenderer;
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

        var commentsQuery = new ListProjectCommentsQuery(project.Id);
        var comments = await _mediator.Send(commentsQuery, token);

        var ticketCountsQuery = new GetTicketCountsByProjectIdQuery(project.Id);
        var ticketCounts = await _mediator.Send(ticketCountsQuery, token);

        await _view.RenderAsync((project, comments, ticketCounts), _console, _localizer, token);
        return ExitCode.Success;
    }
}
