using Lucy.Application.Statuses.Repositories;
using Lucy.Domain.Entities;
using Lucy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Repositories;

/// <summary>
/// Status repository implementation
/// </summary>
public class StatusRepository(
    LucyWriteContext context) : RepositoryBase<Status, long>(context), IStatusRepository
{
    /// <summary>
    /// Gets a status by its key and project ID.
    /// </summary>
    public Task<Status?> GetByKeyAsync(long projectId, string key, CancellationToken token = default)
        => _set.FirstOrDefaultAsync(status => status.ProjectId == projectId && status.Key.Equals(key), token);

    /// <summary>
    /// Checks if a status exists by its key and project ID.
    /// </summary>
    public Task<bool> ExistsByKeyAsync(long projectId, string key, CancellationToken token = default)
        => _set.AnyAsync(status => status.ProjectId == projectId && status.Key.Equals(key), token);

    /// <summary>
    /// Gets all statuses for a specific project.
    /// </summary>
    public Task<List<Status>> GetByProjectIdAsync(long projectId, CancellationToken token = default)
        => _set.Where(status => status.ProjectId == projectId).ToListAsync(token);
}
