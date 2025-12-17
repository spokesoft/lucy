using Lucy.Application.Common.Interfaces;
using Lucy.Application.Tickets.Commands.UnassignTicketFromIteration;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Remove;

/// <summary>
/// Handler for the 'remove ticket' command.
/// </summary>
internal class RemoveTicketCommandHandler(
    IMediator mediator,
    IUnitOfWork unitOfWork,
    IStringLocalizer<Program> localizer,
    IAnsiConsole console) : ICommandHandler<RemoveTicketCommand>
{
    private readonly IMediator _mediator = mediator;
    private readonly IUnitOfWork _uow = unitOfWork;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IAnsiConsole _console = console;

    public async Task<ExitCode> HandleAsync(CommandContext context, RemoveTicketCommand command, CancellationToken token = default)
    {
        var ticket = command.TicketId.HasValue
            ? await _uow.Tickets.GetByIdAsync(command.TicketId.Value, token)
            : await _uow.Tickets.GetByKeyAsync(command.TicketKey!, token);

        var iteration = command.IterationId.HasValue
            ? await _uow.Iterations.GetByIdAsync(command.IterationId.Value, token)
            : await _uow.Iterations.GetByKeyAsync(command.IterationKey!, token);

        if (ticket == null) throw new InvalidOperationException(_localizer["Error.Ticket.NotFound"]);
        if (iteration == null) throw new InvalidOperationException(_localizer["Error.Iteration.NotFound"]);

        await _mediator.Send(new UnassignTicketFromIterationCommand(ticket.Id, iteration.Id), token);

        _console.MarkupLine(_localizer["Messages.RemovedTicketFromIteration", ticket.Key, iteration.Key]);

        return ExitCode.Success;
    }
}
