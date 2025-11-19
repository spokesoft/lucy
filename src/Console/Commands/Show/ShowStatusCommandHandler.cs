using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;
using Lucy.Application.Projects.Queries.GetProjectById;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Statuses.Queries.GetStatusById;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.ListTickets;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Handler for the <see cref="ShowStatusCommand"/> command.
/// </summary>
internal class ShowStatusCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IViewRenderer<(StatusDto, ProjectDto, List<TicketDto>)> viewRenderer,
    IMediator mediator) : ICommandHandler<ShowStatusCommand>
{
    /// <summary>
    /// The console instance for outputting information.
    /// </summary>
    private readonly IAnsiConsole _console = console;

    /// <summary>
    /// The localizer instance for localized strings.
    /// </summary>
    private readonly IStringLocalizer<Program> _localizer = localizer;

    /// <summary>
    /// The view renderer for rendering the status details.
    /// </summary>
    private readonly IViewRenderer<(StatusDto, ProjectDto, List<TicketDto>)> _view = viewRenderer;

    /// <summary>
    /// The mediator instance for sending commands and queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ShowStatusCommand command,
        CancellationToken token = default)
    {
        var statusId = command.StatusId;
        if (statusId is null)
        {
            // Handle ambiguous case: if StatusKey is null but ProjectKey has a value and ProjectId is set,
            // then ProjectKey is actually the status key (single positional argument with --project-id option)
            string? statusKey = command.StatusKey;
            string? projectKey = command.ProjectKey;
            long? projectId = command.ProjectId;

            if (statusKey is null && projectKey is not null && projectId.HasValue)
            {
                // Single positional argument with --project-id: treat as status key
                statusKey = projectKey;
                projectKey = null;
            }

            // Need to get status by key, which requires project ID
            if (projectId is null)
            {
                var projectQuery = new GetProjectIdByKeyQuery(projectKey!);
                projectId = await _mediator.Send(projectQuery, token);
            }

            var statusQuery = new GetStatusByKeyQuery(projectId!.Value, statusKey!);
            var statusByKey = await _mediator.Send(statusQuery, token);
            statusId = statusByKey?.Id;
        }

        if (statusId is null)
        {
            _console.MarkupLine($"[red]{_localizer["Error.Status.NotFound"]}[/]");
            return ExitCode.Error;
        }

        var query = new GetStatusByIdQuery(statusId.Value);
        var status = await _mediator.Send(query, token);

        if (status is null)
        {
            _console.MarkupLine($"[red]{_localizer["Error.Status.NotFound"]}[/]");
            return ExitCode.Error;
        }

        var getProjectQuery = new GetProjectByIdQuery(status.ProjectId);
        var project = await _mediator.Send(getProjectQuery, token);

        if (project is null)
        {
            _console.MarkupLine($"[red]{_localizer["Error.Project.NotFound"]}[/]");
            return ExitCode.Error;
        }

        var tickets = new List<TicketDto>();

        if (command.IncludeTickets)
        {
            var ticketsQuery = new ListTicketsQuery(project.Id, status.Id);
            tickets = await _mediator.Send(ticketsQuery, token);
        }

        await _view.RenderAsync((status, project, tickets), _console, _localizer, token);
        return ExitCode.Success;
    }
}
