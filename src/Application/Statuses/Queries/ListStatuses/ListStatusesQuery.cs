using Lucy.Application.Interfaces;
using Lucy.Application.Queries;
using Lucy.Application.Statuses.DTOs;

namespace Lucy.Application.Statuses.Queries.ListStatuses;

/// <summary>
/// Query to list all statuses for a specific project.
/// </summary>
/// <param name="ProjectId">The ID of the project to list statuses for.</param>
/// <param name="SortBy">The field to sort by.</param>
/// <param name="SortDirection">The direction to sort.</param>
public record ListStatusesQuery(
    long ProjectId,
    StatusSortField SortBy = StatusSortField.Order,
    SortDirection SortDirection = SortDirection.Ascending) : IRequest<List<StatusDto>>;
