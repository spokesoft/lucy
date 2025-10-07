using Lucy.Domain.Entities;

namespace Lucy.Application.Interfaces;

/// <summary>
/// Read-only repository interface.
/// </summary>
public interface IReadOnlyRepository<T, TKey>
    where T : DomainEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    Task<T?> GetByIdAsync(TKey id, CancellationToken token = default);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken token = default);

    /// <summary>
    /// Checks if an entity exists by its identifier.
    /// </summary>
    Task<bool> ExistsByIdAsync(TKey id, CancellationToken token = default);
}
