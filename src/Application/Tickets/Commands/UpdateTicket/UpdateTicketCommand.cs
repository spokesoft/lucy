using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Tickets.Commands.UpdateTicket;

/// <summary>
/// Update Ticket Command
/// </summary>
/// <param name="Id">The unique identifier of the ticket to be updated.</param>
/// <param name="StatusId">The ID of the status for this ticket.</param>
/// <param name="Title">The title of the ticket.</param>
/// <param name="Description">The description of the ticket.</param>
public record UpdateTicketCommand(
    long Id,
    long? StatusId,
    string? Title,
    string? Description) : IRequest;
