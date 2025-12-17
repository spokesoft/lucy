using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Tickets.Commands.UpdateTicket;

/// <summary>
/// Update Ticket Command Handler
/// </summary>
public class UpdateTicketCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTicketCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the UpdateTicketCommand.
    /// </summary>
    public async Task HandleAsync(UpdateTicketCommand request, CancellationToken token = default)
    {
        var ticket = await _uow.Tickets.GetByIdAsync(request.Id, token)
            ?? throw new InvalidOperationException("Ticket should exist due to prior validation.");

        if (request.StatusId.HasValue)
            ticket.UpdateStatus(request.StatusId.Value);

        if (request.Title != null)
            ticket.UpdateTitle(request.Title);

        if (request.Description != null)
            ticket.UpdateDescription(request.Description);

        _uow.Tickets.Update(ticket);
        await _uow.SaveChangesAsync(token);
    }
}
