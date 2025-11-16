using AppUpdateTicketCommand = Lucy.Application.Tickets.Commands.UpdateTicket.UpdateTicketCommand;
using Lucy.Application.Interfaces;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Application.Tickets.Queries.GetTicketById;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Handler for the <see cref="UpdateTicketCommand"/> command.
/// </summary>
internal class UpdateTicketCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<UpdateTicketCommand>
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
    /// The mediator instance for sending commands and queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        UpdateTicketCommand command,
        CancellationToken token = default)
    {
        // Resolve StatusId if StatusKey is provided
        long? statusId = command.StatusId;
        if (statusId is null && command.StatusKey is not null)
        {
            // Get the ticket to find its project
            var ticketQuery = new GetTicketByIdQuery(command.Id);
            var ticket = await _mediator.Send(ticketQuery, token);

            if (ticket is null)
            {
                throw new InvalidOperationException($"Ticket with ID {command.Id} not found.");
            }

            var statusQuery = new GetStatusByKeyQuery(ticket.ProjectId, command.StatusKey);
            var status = await _mediator.Send(statusQuery, token);

            if (status is null)
            {
                throw new InvalidOperationException($"Status with key '{command.StatusKey}' not found in project.");
            }

            statusId = status.Id;
        }

        var request = new AppUpdateTicketCommand(
            command.Id,
            statusId,
            command.Title,
            command.Description);

        await _mediator.Send(request, token);

        _console.MarkupLine(_localizer["Messages.UpdatedTicket", command.Id]);

        return ExitCode.Success;
    }
}
