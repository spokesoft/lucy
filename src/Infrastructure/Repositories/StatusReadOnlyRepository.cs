using Lucy.Application.Common.Queries;
using Lucy.Application.Statuses.Queries;
using Lucy.Application.Statuses.Repositories;
using Lucy.Domain.Entities;
using Lucy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Repositories;

/// <summary>
/// Status read-only repository implementation
/// </summary>
public class StatusReadOnlyRepository(
    LucyReadContext context) : ReadOnlyRepositoryBase<Status, long>(context), IStatusReadOnlyRepository
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

    /// <summary>
    /// Gets all statuses for a specific project with sorting.
    /// </summary>
    public Task<List<Status>> GetByProjectIdAsync(long projectId, StatusField sortBy, SortDirection sortDirection, CancellationToken token = default)
    {
        var query = _set.Where(status => status.ProjectId == projectId);

        query = sortBy switch
        {
            StatusField.Id => sortDirection == SortDirection.Ascending
                ? query.OrderBy(s => s.Id)
                : query.OrderByDescending(s => s.Id),
            StatusField.Order => sortDirection == SortDirection.Ascending
                ? query.OrderBy(s => s.Order)
                : query.OrderByDescending(s => s.Order),
            StatusField.Key => sortDirection == SortDirection.Ascending
                ? query.OrderBy(s => s.Key)
                : query.OrderByDescending(s => s.Key),
            StatusField.Name => sortDirection == SortDirection.Ascending
                ? query.OrderBy(s => s.Name)
                : query.OrderByDescending(s => s.Name),
            StatusField.CreatedAt => sortDirection == SortDirection.Ascending
                ? query.OrderBy(s => s.CreatedAt)
                : query.OrderByDescending(s => s.CreatedAt),
            StatusField.UpdatedAt => sortDirection == SortDirection.Ascending
                ? query.OrderBy(s => s.UpdatedAt)
                : query.OrderByDescending(s => s.UpdatedAt),
            _ => query.OrderBy(s => s.Id)
        };

        return query.ToListAsync(token);
    }
}
