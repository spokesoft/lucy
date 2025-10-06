using Lucy.Domain.Entities;

namespace Lucy.Application.Interfaces;

/// <summary>
/// Unit of Work interface for managing repositories and committing changes.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Repository for Project entities.
    /// </summary>
    public IRepository<Project, long> Projects { get; }

    /// <summary>
    /// Saves all changes made in the context to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken token = default);
}
