using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Statuses.Queries.StatusExistsByKey;

/// <summary>
/// Query to check if a status exists by its key and project ID.
/// </summary>
/// <param name="ProjectId">The ID of the project.</param>
/// <param name="Key">The key of the status to check for.</param>
public record StatusExistsByKeyQuery(long ProjectId, string Key) : IRequest<bool>;
