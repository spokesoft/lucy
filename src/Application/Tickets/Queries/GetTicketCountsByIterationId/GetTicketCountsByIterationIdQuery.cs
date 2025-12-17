using Lucy.Application.Common.Interfaces;
using Lucy.Application.Tickets.DTOs;

namespace Lucy.Application.Tickets.Queries.GetTicketCountsByIterationId;

/// <summary>
/// Query to get ticket counts by status for a specific iteration.
/// </summary>
public record GetTicketCountsByIterationIdQuery(long IterationId) : IRequest<IEnumerable<TicketCountByStatusDto>>;
