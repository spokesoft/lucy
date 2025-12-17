using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Tickets.Commands.DeleteTicket;

/// <summary>
/// Command to delete a ticket by its ID.
/// </summary>
/// <param name="Id">The unique identifier of the ticket to be deleted.</param>
public record DeleteTicketCommand(long Id) : IRequest;
