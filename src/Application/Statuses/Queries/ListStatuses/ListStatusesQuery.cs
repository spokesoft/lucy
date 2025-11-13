using Lucy.Application.Interfaces;
using Lucy.Application.Statuses.DTOs;

namespace Lucy.Application.Statuses.Queries.ListStatuses;

/// <summary>
/// Query to list all statuses for a specific project.
/// </summary>
/// <param name="ProjectId">The ID of the project to list statuses for.</param>
public record ListStatusesQuery(long ProjectId) : IRequest<List<StatusDto>>;
