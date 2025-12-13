using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Queries.GetIterationIdByKey;
using Lucy.Application.Iterations.Queries.GetProjectIdFromIteration;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Application.Statuses.Queries.ListStatuses;
using Lucy.Application.Tags.Queries.GetTagIdByKey;
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
        // Resolve project ID
        long projectId;
        if (command.Id.HasValue)
        {
            projectId = command.Id.Value;
        }
        else if (!string.IsNullOrWhiteSpace(command.Key))
        {
            var id = await _mediator.Send(new GetProjectIdByKeyQuery(command.Key), token);
            projectId = id ?? throw new InvalidOperationException(_localizer["Error.Project.NotFound"]);
        }
        else
        {
            var projId = await _mediator.Send(
                new GetProjectIdFromIterationQuery(command.IterationId, command.IterationKey),
                token);

            if (!projId.HasValue)
            {
                throw new InvalidOperationException(_localizer["Error.Project.NotFound"]);
            }

            projectId = projId.Value;
        }

        // Resolve status ID from key if needed
        long? statusId = command.StatusId;
        if (statusId == null && !string.IsNullOrWhiteSpace(command.StatusKey))
        {
            var status = await _mediator.Send(
                new GetStatusByKeyQuery(projectId, command.StatusKey), token);
            statusId = status?.Id;
        }

        // Resolve tag ID from key if needed
        long? tagId = command.TagId;
        if (tagId == null && !string.IsNullOrWhiteSpace(command.TagKey))
        {
            tagId = await _mediator.Send(
                new GetTagIdByKeyQuery(projectId, command.TagKey), token);
        }

        // Resolve iteration ID from key if needed
        long? iterationId = command.IterationId;
        if (iterationId == null && !string.IsNullOrWhiteSpace(command.IterationKey))
        {
            iterationId = await _mediator.Send(
                new GetIterationIdByKeyQuery(command.IterationKey), token);
        }

        var ticketsQuery = new ListTicketsQuery(
            projectId,
            statusId,
            TagId: tagId,
            IterationId: iterationId);
        var tickets = await _mediator.Send(ticketsQuery, token);

        var statusesQuery = new ListStatusesQuery(projectId);
        var statuses = await _mediator.Send(statusesQuery, token);
        var statusLookup = statuses.ToDictionary(
            s => s.Id,
            s => (s.Key, s.Color.ToString().ToLowerInvariant()));

        await _view.RenderAsync((tickets, statusLookup), _console, _localizer, token);
        return ExitCode.Success;
    }
}
