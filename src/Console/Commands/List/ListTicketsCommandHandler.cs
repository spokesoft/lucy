using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.Queries.ListStatuses;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.ListTickets;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Handler for the <see cref="ListTicketsCommand"/> command.
/// </summary>
internal class ListTicketsCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IViewRenderer<(IEnumerable<TicketDto>, Dictionary<long, (string Key, string Color)>)> viewRenderer,
    IMediator mediator) : ICommandHandler<ListTicketsCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IViewRenderer<(IEnumerable<TicketDto>, Dictionary<long, (string Key, string Color)>)> _view = viewRenderer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ListTicketsCommand command,
        CancellationToken token = default)
    {
        // Resolve project ID from key if needed
        var projectId = command.Id ?? await _mediator.Send(
            new GetProjectIdByKeyQuery(command.Key!), token);

        var ticketsQuery = new ListTicketsQuery(projectId.Value);
        var tickets = await _mediator.Send(ticketsQuery, token);

        var statusesQuery = new ListStatusesQuery(projectId.Value);
        var statuses = await _mediator.Send(statusesQuery, token);
        var statusLookup = statuses.ToDictionary(
            s => s.Id,
            s => (s.Key, s.Color.ToString().ToLowerInvariant()));

        await _view.RenderAsync((tickets, statusLookup), _console, _localizer, token);
        return ExitCode.Success;
    }
}
