namespace Lucy.Domain.Entities;

/// <summary>
/// Base class for entities.
/// </summary>
public abstract class DomainEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Base class for entities with a typed Id.
/// </summary>
public abstract class DomainEntity<T> : DomainEntity
    where T : IEquatable<T>
{
    public T Id { get; set; } = default!;
}
