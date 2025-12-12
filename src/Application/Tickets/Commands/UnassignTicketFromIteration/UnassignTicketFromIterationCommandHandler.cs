using Lucy.Application.Interfaces;

namespace Lucy.Application.Tickets.Commands.UnassignTicketFromIteration;

/// <summary>
/// Handler for UnassignTicketFromIterationCommand.
/// </summary>
public class UnassignTicketFromIterationCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UnassignTicketFromIterationCommand>
{
    private readonly IUnitOfWork _uow = unitOfWork;

    public async Task HandleAsync(UnassignTicketFromIterationCommand request, CancellationToken token = default)
    {
        var ticket = await _uow.Tickets.GetByIdAsync(request.TicketId, token)
            ?? throw new InvalidOperationException("Ticket not found.");

        // Verify the ticket is actually assigned to the specified iteration
        if (ticket.IterationId != request.IterationId)
        {
            throw new InvalidOperationException("Ticket is not assigned to the specified iteration.");
        }

        ticket.UnsetIteration();
        _uow.Tickets.Update(ticket);
        await _uow.SaveChangesAsync(token);
    }
}
