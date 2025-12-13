using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.DTOs;
using Lucy.Application.Queries;

namespace Lucy.Application.Iterations.Queries.ListIterations;

/// <summary>
/// Query to list all iterations.
/// </summary>
/// <param name="ProjectId">The ID of the project to list iterations for.</param>
/// <param name="SortBy">The field to sort by.</param>
/// <param name="SortDirection">The direction to sort.</param>
public record ListIterationsQuery(
    long ProjectId,
    IterationSortField SortBy = IterationSortField.Id,
    SortDirection SortDirection = SortDirection.Ascending) : IRequest<List<IterationDto>>;
