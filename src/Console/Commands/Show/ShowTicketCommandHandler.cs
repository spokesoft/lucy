using Lucy.Application.Interfaces;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.GetTicketById;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Handler for the <see cref="ShowTicketCommand"/> command.
/// </summary>
internal class ShowTicketCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IViewRenderer<TicketDto> viewRenderer,
    IMediator mediator) : ICommandHandler<ShowTicketCommand>
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
    /// The view renderer for rendering the ticket details.
    /// </summary>
    private readonly IViewRenderer<TicketDto> _view = viewRenderer;

    /// <summary>
    /// The mediator instance for sending commands and queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ShowTicketCommand command,
        CancellationToken token = default)
    {
        var query = new GetTicketByIdQuery(command.TicketId);
        var ticket = await _mediator.Send(query, token);

        if (ticket is null)
        {
            _console.MarkupLine($"[red]{_localizer["Error.Ticket.NotFound"]}[/]");
            return ExitCode.Error;
        }

        await _view.RenderAsync(ticket, _console, _localizer, token);
        return ExitCode.Success;
    }
}
