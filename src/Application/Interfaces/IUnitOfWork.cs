using Lucy.Application.Projects.Repositories;

namespace Lucy.Application.Interfaces;

/// <summary>
/// Unit of Work interface for managing repositories and committing changes.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Repository for Project entities.
    /// </summary>
    public IProjectRepository Projects { get; }

    /// <summary>
    /// Saves all changes made in the context to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken token = default);
}
