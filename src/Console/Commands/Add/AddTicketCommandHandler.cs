using Lucy.Application.Interfaces;
using Lucy.Application.Tickets.Commands.AssignTicketToIteration;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Add;

/// <summary>
/// Handler for the 'add ticket' command.
/// </summary>
internal class AddTicketCommandHandler(
    IMediator mediator,
    IUnitOfWork unitOfWork,
    IStringLocalizer<Program> localizer,
    IAnsiConsole console) : ICommandHandler<AddTicketCommand>
{
    private readonly IMediator _mediator = mediator;
    private readonly IUnitOfWork _uow = unitOfWork;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IAnsiConsole _console = console;

    public async Task<ExitCode> HandleAsync(CommandContext context, AddTicketCommand command, CancellationToken token = default)
    {
        var ticket = command.TicketId.HasValue
            ? await _uow.Tickets.GetByIdAsync(command.TicketId.Value, token)
            : await _uow.Tickets.GetByKeyAsync(command.TicketKey!, token);

        var iteration = command.IterationId.HasValue
            ? await _uow.Iterations.GetByIdAsync(command.IterationId.Value, token)
            : await _uow.Iterations.GetByKeyAsync(command.IterationKey!, token);

        // Validator ensures existence, but we check for null safety or race conditions
        if (ticket == null) throw new InvalidOperationException(_localizer["Error.Ticket.NotFound"]);
        if (iteration == null) throw new InvalidOperationException(_localizer["Error.Iteration.NotFound"]);

        await _mediator.Send(new AssignTicketToIterationCommand(ticket.Id, iteration.Id), token);

        _console.MarkupLine(_localizer["Messages.AddedTicketToIteration", ticket.Key, iteration.Key]);

        return ExitCode.Success;
    }
}
