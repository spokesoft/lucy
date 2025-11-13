using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Statuses.Repositories;

/// <summary>
/// Read-only repository interface for Status entities.
/// </summary>
public interface IStatusReadOnlyRepository : IReadOnlyRepository<Status, long>
{
    /// <summary>
    /// Gets a status by its key and project ID.
    /// </summary>
    Task<Status?> GetByKeyAsync(long projectId, string key, CancellationToken token = default);

    /// <summary>
    /// Checks if a status exists by its key and project ID.
    /// </summary>
    Task<bool> ExistsByKeyAsync(long projectId, string key, CancellationToken token = default);

    /// <summary>
    /// Gets all statuses for a specific project.
    /// </summary>
    Task<List<Status>> GetByProjectIdAsync(long projectId, CancellationToken token = default);
}
