using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Tickets.Commands.DeleteTicket;

/// <summary>
/// Handler for the DeleteTicketCommand.
/// </summary>
public class DeleteTicketCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteTicketCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the DeleteTicketCommand.
    /// </summary>
    public async Task HandleAsync(DeleteTicketCommand request, CancellationToken token = default)
    {
        var ticket = await _uow.Tickets.GetByIdAsync(request.Id, token)
            ?? throw new InvalidOperationException("Ticket should exist due to prior validation.");

        _uow.Tickets.Remove(ticket);
        await _uow.SaveChangesAsync(token);
    }
}
