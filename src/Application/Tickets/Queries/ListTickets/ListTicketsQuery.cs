using Lucy.Application.Interfaces;
using Lucy.Application.Queries;
using Lucy.Application.Tickets.DTOs;

namespace Lucy.Application.Tickets.Queries.ListTickets;

/// <summary>
/// Query to list all tickets for a specific project.
/// </summary>
/// <param name="ProjectId">The ID of the project to list tickets for.</param>
/// <param name="StatusId">Optional status ID to filter tickets by.</param>
/// <param name="SortBy">The field to sort by.</param>
/// <param name="SortDirection">The direction to sort.</param>
/// <param name="TagId">Optional tag ID to filter tickets by.</param>
/// <param name="IterationId">Optional iteration ID to filter tickets by.</param>
public record ListTicketsQuery(
    long ProjectId,
    long? StatusId = null,
    TicketField SortBy = TicketField.Id,
    SortDirection SortDirection = SortDirection.Ascending,
    long? TagId = null,
    long? IterationId = null) : IRequest<List<TicketDto>>;
