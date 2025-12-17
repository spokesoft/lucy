using AppDeleteTicketCommand = Lucy.Application.Tickets.Commands.DeleteTicket.DeleteTicketCommand;
using Lucy.Application.Common.Interfaces;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Handler for the <see cref="DeleteTicketCommand"/> command.
/// </summary>
internal class DeleteTicketCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IMediator mediator) : ICommandHandler<DeleteTicketCommand>
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
        DeleteTicketCommand command,
        CancellationToken token = default)
    {
        // Resolve ticket ID from key if needed
        long ticketId;
        if (command.Id.HasValue)
        {
            ticketId = command.Id.Value;
        }
        else
        {
            var ticketQuery = new GetTicketByKeyQuery(command.Key!);
            var ticket = await _mediator.Send(ticketQuery, token);

            if (ticket is null)
            {
                throw new InvalidOperationException($"Ticket with key '{command.Key}' not found.");
            }

            ticketId = ticket.Id;
        }

        var request = new AppDeleteTicketCommand(ticketId);
        await _mediator.Send(request, token);

        _console.MarkupLine("[green]:check_mark:[/] " + _localizer["Messages.DeletedTicket", ticketId]);

        return ExitCode.Success;
    }
}
