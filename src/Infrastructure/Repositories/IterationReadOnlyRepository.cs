using Lucy.Application.Iterations.Repositories;
using Lucy.Application.Iterations.Queries;
using Lucy.Application.Common.Queries;
using Lucy.Domain.Entities;
using Lucy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Repositories;

/// <summary>
/// Iteration read-only repository implementation
/// </summary>
public class IterationReadOnlyRepository(
    LucyReadContext context) : ReadOnlyRepositoryBase<Iteration, long>(context), IIterationReadOnlyRepository
{
    /// <summary>
    /// Checks if an iteration exists by its key asynchronously.
    /// </summary>
    public Task<bool> ExistsByKeyAsync(string key, CancellationToken token = default)
        => _set.AnyAsync(iteration => iteration.Key.Equals(key), token);

    /// <summary>
    /// Gets an iteration by its key asynchronously.
    /// </summary>
    public Task<Iteration?> GetByKeyAsync(string key, CancellationToken token = default)
        => _set.FirstOrDefaultAsync(iteration => iteration.Key.Equals(key), token);

    /// <summary>
    /// Gets all iterations for a specific project.
    /// </summary>
    public Task<List<Iteration>> GetByProjectIdAsync(long projectId, IterationField sortBy, SortDirection sortDirection, CancellationToken token = default)
    {
        var query = _set.Where(iteration => iteration.ProjectId == projectId);

        query = sortBy switch
        {
            IterationField.Id => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.Id)
                : query.OrderByDescending(i => i.Id),
            IterationField.Key => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.Key)
                : query.OrderByDescending(i => i.Key),
            IterationField.Name => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.Name)
                : query.OrderByDescending(i => i.Name),
            IterationField.StartDate => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.StartDate)
                : query.OrderByDescending(i => i.StartDate),
            IterationField.EndDate => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.EndDate)
                : query.OrderByDescending(i => i.EndDate),
            IterationField.CreatedAt => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.CreatedAt)
                : query.OrderByDescending(i => i.CreatedAt),
            IterationField.UpdatedAt => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.UpdatedAt)
                : query.OrderByDescending(i => i.UpdatedAt),
            _ => query.OrderBy(i => i.Id)
        };

        return query.ToListAsync(token);
    }

    /// <summary>
    /// Gets all iterations with sorting.
    /// </summary>
    public Task<List<Iteration>> GetAllAsync(IterationField sortBy, SortDirection sortDirection, CancellationToken token = default)
    {
        var query = _set.AsQueryable();

        query = sortBy switch
        {
            IterationField.Id => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.Id)
                : query.OrderByDescending(i => i.Id),
            IterationField.Key => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.Key)
                : query.OrderByDescending(i => i.Key),
            IterationField.Name => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.Name)
                : query.OrderByDescending(i => i.Name),
            IterationField.StartDate => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.StartDate)
                : query.OrderByDescending(i => i.StartDate),
            IterationField.EndDate => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.EndDate)
                : query.OrderByDescending(i => i.EndDate),
            IterationField.CreatedAt => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.CreatedAt)
                : query.OrderByDescending(i => i.CreatedAt),
            IterationField.UpdatedAt => sortDirection == SortDirection.Ascending
                ? query.OrderBy(i => i.UpdatedAt)
                : query.OrderByDescending(i => i.UpdatedAt),
            _ => query.OrderBy(i => i.Id)
        };

        return query.ToListAsync(token);
    }
}
