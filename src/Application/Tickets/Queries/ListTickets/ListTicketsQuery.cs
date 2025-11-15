using Lucy.Application.Interfaces;
using Lucy.Application.Queries;
using Lucy.Application.Tickets.DTOs;

namespace Lucy.Application.Tickets.Queries.ListTickets;

/// <summary>
/// Query to list all tickets for a specific project.
/// </summary>
/// <param name="ProjectId">The ID of the project to list tickets for.</param>
/// <param name="SortBy">The field to sort by.</param>
/// <param name="SortDirection">The direction to sort.</param>
public record ListTicketsQuery(
    long ProjectId,
    TicketSortField SortBy = TicketSortField.Id,
    SortDirection SortDirection = SortDirection.Ascending) : IRequest<List<TicketDto>>;
