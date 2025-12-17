using Lucy.Application.Common.Interfaces;
using Lucy.Application.Tickets.DTOs;

namespace Lucy.Application.Tickets.Queries.GetTicketById;

/// <summary>
/// Query to get a ticket by its ID.
/// </summary>
/// <param name="Id">The ID of the ticket to retrieve.</param>
public record GetTicketByIdQuery(long Id) : IRequest<TicketDto?>;
