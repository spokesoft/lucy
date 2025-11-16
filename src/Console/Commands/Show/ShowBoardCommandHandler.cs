using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Statuses.Queries.ListStatuses;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.ListTickets;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Handler for the <see cref="ShowBoardCommand"/> command.
/// </summary>
internal class ShowBoardCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IViewRenderer<(IEnumerable<StatusDto>, Dictionary<long, List<TicketDto>>)> viewRenderer,
    IMediator mediator) : ICommandHandler<ShowBoardCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IViewRenderer<(IEnumerable<StatusDto>, Dictionary<long, List<TicketDto>>)> _view = viewRenderer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ShowBoardCommand command,
        CancellationToken token = default)
    {
        // Resolve project ID from key if needed
        var projectId = command.Id ?? await _mediator.Send(
            new GetProjectIdByKeyQuery(command.Key!), token);

        if (projectId is null)
        {
            _console.MarkupLine($"[red]{_localizer["Error.Project.NotFound"]}[/]");
            return ExitCode.Error;
        }

        // Get all statuses for the project
        var statusesQuery = new ListStatusesQuery(projectId.Value);
        var statuses = await _mediator.Send(statusesQuery, token);

        // Get all tickets for the project
        var ticketsQuery = new ListTicketsQuery(projectId.Value);
        var tickets = await _mediator.Send(ticketsQuery, token);

        // Group tickets by status
        var ticketsByStatus = tickets
            .GroupBy(t => t.StatusId)
            .ToDictionary(g => g.Key, g => g.ToList());

        await _view.RenderAsync((statuses, ticketsByStatus), _console, _localizer, token);
        return ExitCode.Success;
    }
}
