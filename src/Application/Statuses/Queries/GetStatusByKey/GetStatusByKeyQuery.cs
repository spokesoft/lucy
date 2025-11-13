using Lucy.Application.Interfaces;
using Lucy.Application.Statuses.DTOs;

namespace Lucy.Application.Statuses.Queries.GetStatusByKey;

/// <summary>
/// Query to get a status by its key and project ID.
/// </summary>
/// <param name="ProjectId">The ID of the project.</param>
/// <param name="Key">The key of the status to retrieve.</param>
public record GetStatusByKeyQuery(long ProjectId, string Key) : IRequest<StatusDto?>;
