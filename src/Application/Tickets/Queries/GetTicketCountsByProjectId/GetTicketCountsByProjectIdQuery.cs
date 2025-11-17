using Lucy.Application.Interfaces;
using Lucy.Application.Tickets.DTOs;

namespace Lucy.Application.Tickets.Queries.GetTicketCountsByProjectId;

/// <summary>
/// Query to get ticket counts by status for a specific project.
/// </summary>
public record GetTicketCountsByProjectIdQuery(long ProjectId) : IRequest<IEnumerable<TicketCountByStatusDto>>;
