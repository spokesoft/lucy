using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Tickets.Commands.AssignTicketToIteration;

/// <summary>
/// Handler for AssignTicketToIterationCommand.
/// </summary>
public class AssignTicketToIterationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AssignTicketToIterationCommand>
{
    private readonly IUnitOfWork _uow = unitOfWork;

    public async Task HandleAsync(AssignTicketToIterationCommand request, CancellationToken token = default)
    {
        var ticket = await _uow.Tickets.GetByIdAsync(request.TicketId, token)
            ?? throw new InvalidOperationException("Ticket not found.");

        var iteration = await _uow.Iterations.GetByIdAsync(request.IterationId, token)
            ?? throw new InvalidOperationException("Iteration not found.");

        if (ticket.ProjectId != iteration.ProjectId)
        {
             throw new InvalidOperationException("Ticket and Iteration must belong to the same project.");
        }

        ticket.SetIteration(iteration.Id);
        _uow.Tickets.Update(ticket);
        await _uow.SaveChangesAsync(token);
    }
}
