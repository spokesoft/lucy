using Lucy.Application.Projects.Repositories;
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
}
