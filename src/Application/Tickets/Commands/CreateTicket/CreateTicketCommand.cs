using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Tickets.Commands.CreateTicket;

/// <summary>
/// Command to create a new ticket.
/// </summary>
/// <param name="ProjectId">The ID of the project this ticket belongs to.</param>
/// <param name="StatusId">The ID of the status for this ticket.</param>
/// <param name="Title">The title of the ticket.</param>
/// <param name="Description">A detailed description of the ticket.</param>
public record CreateTicketCommand(
    long ProjectId,
    long StatusId,
    string Title,
    string? Description) : IRequest<long>;
