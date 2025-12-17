using Lucy.Application.Projects.Repositories;
using Lucy.Application.Projects.Queries;
using Lucy.Application.Common.Queries;
using Lucy.Domain.Entities;
using Lucy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Repositories;

/// <summary>
/// Project read-only repository implementation
/// </summary>
public class ProjectReadOnlyRepository(
    LucyReadContext context) : ReadOnlyRepositoryBase<Project, long>(context), IProjectReadOnlyRepository
{
    /// <summary>
    /// Checks if a project exists by its key asynchronously.
    /// </summary>
    public Task<bool> ExistsByKeyAsync(string key, CancellationToken token = default)
        => _set.AnyAsync(project => project.Key.Equals(key), token);

    /// <summary>
    /// Gets a project by its key asynchronously.
    /// </summary>
    public Task<Project?> GetByKeyAsync(string key, CancellationToken token = default)
        => _set.FirstOrDefaultAsync(project => project.Key.Equals(key), token);

    /// <summary>
    /// Gets all projects with sorting.
    /// </summary>
    public Task<List<Project>> GetAllAsync(ProjectField sortBy, SortDirection sortDirection, CancellationToken token = default)
    {
        var query = _set.AsQueryable();

        query = sortBy switch
        {
            ProjectField.Id => sortDirection == SortDirection.Ascending
                ? query.OrderBy(p => p.Id)
                : query.OrderByDescending(p => p.Id),
            ProjectField.Key => sortDirection == SortDirection.Ascending
                ? query.OrderBy(p => p.Key)
                : query.OrderByDescending(p => p.Key),
            ProjectField.Name => sortDirection == SortDirection.Ascending
                ? query.OrderBy(p => p.Name)
                : query.OrderByDescending(p => p.Name),
            ProjectField.CreatedAt => sortDirection == SortDirection.Ascending
                ? query.OrderBy(p => p.CreatedAt)
                : query.OrderByDescending(p => p.CreatedAt),
            ProjectField.UpdatedAt => sortDirection == SortDirection.Ascending
                ? query.OrderBy(p => p.UpdatedAt)
                : query.OrderByDescending(p => p.UpdatedAt),
            _ => query.OrderBy(p => p.Id)
        };

        return query.ToListAsync(token);
    }
}
