using Lucy.Application.Common.Interfaces;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;

namespace Lucy.Application.Tickets.Commands.CreateTicket;

/// <summary>
/// Handler for the CreateTicketCommand.
/// </summary>
public class CreateTicketCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTicketCommand, long>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the CreateTicketCommand.
    /// </summary>
    public async Task<long> HandleAsync(CreateTicketCommand request, CancellationToken token = default)
    {
        var project = await _uow.Projects.GetByIdAsync(request.ProjectId, token)
            ?? throw new InvalidOperationException("Project should exist due to prior validation.");

        var status = await _uow.Statuses.GetByIdAsync(request.StatusId, token)
            ?? throw new InvalidOperationException("Status should exist due to prior validation.");

        // Get the ticket sequence for this project
        var sequence = await _uow.Sequences.GetByTypeAsync(request.ProjectId, SequenceType.Ticket, token)
            ?? throw new InvalidOperationException("Ticket sequence not found for project.");

        // Generate the ticket identifier
        var key = sequence.Next();

        var ticket = new Ticket(
            request.ProjectId,
            request.StatusId,
            key,
            sequence.Value,
            request.Title,
            request.Description);

        _uow.Sequences.Update(sequence);
        await _uow.Tickets.AddAsync(ticket, token);
        await _uow.SaveChangesAsync(token);

        return ticket.Id;
    }
}
