using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;
using Lucy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Repositories;

/// <summary>
/// Base read-only repository implementation
/// </summary>
public abstract class ReadOnlyRepositoryBase<TEntity, TKey>(
    LucyReadContext context) : IReadOnlyRepository<TEntity, TKey>
    where TEntity : DomainEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Database set for the entity
    /// </summary>
    protected readonly DbSet<TEntity> _set = context.Set<TEntity>();

    /// <summary>
    /// Gets all entities asynchronously.
    /// </summary>
    public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken token = default)
        => _set.ToListAsync(token).ContinueWith(t => (IEnumerable<TEntity>)t.Result, token);

    /// <summary>
    /// Gets an entity by its id asynchronously.
    /// </summary>
    public Task<TEntity?> GetByIdAsync(TKey id, CancellationToken token = default)
        => _set.FirstOrDefaultAsync(e => e.Id.Equals(id), token);

    /// <summary>
    /// Checks if an entity exists by its id asynchronously.
    /// </summary>
    public Task<bool> ExistsByIdAsync(TKey id, CancellationToken token = default)
        => _set.AnyAsync(e => e.Id.Equals(id), token);
}
