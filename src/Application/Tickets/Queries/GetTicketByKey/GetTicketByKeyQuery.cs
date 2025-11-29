using Lucy.Application.Interfaces;
using Lucy.Application.Tickets.DTOs;

namespace Lucy.Application.Tickets.Queries.GetTicketByKey;

/// <summary>
/// Query to get a ticket by its key.
/// </summary>
/// <param name="Key">The key of the ticket to retrieve.</param>
public record GetTicketByKeyQuery(string Key) : IRequest<TicketDto?>;
