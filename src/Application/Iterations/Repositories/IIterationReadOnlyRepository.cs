using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Queries;
using Lucy.Application.Queries;
using Lucy.Domain.Entities;

namespace Lucy.Application.Iterations.Repositories;

/// <summary>
/// Read-only repository interface for Iteration entities.
/// </summary>
public interface IIterationReadOnlyRepository : IReadOnlyRepository<Iteration, long>
{
    /// <summary>
    /// Gets an iteration by its key.
    /// </summary>
    Task<Iteration?> GetByKeyAsync(string key, CancellationToken token = default);

    /// <summary>
    /// Checks if an iteration exists by its key.
    /// </summary>
    Task<bool> ExistsByKeyAsync(string key, CancellationToken token = default);

    /// <summary>
    /// Gets all iterations for a specific project.
    /// </summary>
    Task<List<Iteration>> GetByProjectIdAsync(long projectId, CancellationToken token = default);

    /// <summary>
    /// Gets all iterations with sorting.
    /// </summary>
    Task<List<Iteration>> GetAllAsync(IterationSortField sortBy, SortDirection sortDirection, CancellationToken token = default);
}
