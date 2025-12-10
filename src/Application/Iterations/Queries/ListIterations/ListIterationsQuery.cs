using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.DTOs;
using Lucy.Application.Queries;

namespace Lucy.Application.Iterations.Queries.ListIterations;

/// <summary>
/// Query to list all iterations.
/// </summary>
/// <param name="SortBy">The field to sort by.</param>
/// <param name="SortDirection">The direction to sort.</param>
public record ListIterationsQuery(
    IterationSortField SortBy = IterationSortField.Id,
    SortDirection SortDirection = SortDirection.Ascending) : IRequest<List<IterationDto>>;
