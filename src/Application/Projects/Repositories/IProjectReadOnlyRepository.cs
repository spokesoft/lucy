using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries;
using Lucy.Application.Queries;
using Lucy.Domain.Entities;

namespace Lucy.Application.Projects.Repositories;

/// <summary>
/// Read-only repository interface for Project entities.
/// </summary>
public interface IProjectReadOnlyRepository : IReadOnlyRepository<Project, long>
{
    /// <summary>
    /// Gets a project by its key.
    /// </summary>
    Task<Project?> GetByKeyAsync(string key, CancellationToken token = default);

    /// <summary>
    /// Checks if a project exists by its key.
    /// </summary>
    Task<bool> ExistsByKeyAsync(string key, CancellationToken token = default);

    /// <summary>
    /// Gets all projects with sorting.
    /// </summary>
    Task<List<Project>> GetAllAsync(ProjectSortField sortBy, SortDirection sortDirection, CancellationToken token = default);
}
