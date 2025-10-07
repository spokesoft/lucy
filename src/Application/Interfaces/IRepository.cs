using Lucy.Domain.Entities;

namespace Lucy.Application.Interfaces;

/// <summary>
/// Repository interface for generic entity operations.
/// </summary>
public interface IRepository<T, TKey> : IReadOnlyRepository<T, TKey>
    where T : DomainEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Asynchronously adds a new entity to the repository.
    /// </summary>
    Task AddAsync(T entity, CancellationToken token = default);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Deletes an entity from the repository.
    /// </summary>
    void Remove(T entity);
}
