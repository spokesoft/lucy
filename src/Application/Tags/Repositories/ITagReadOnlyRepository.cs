using Lucy.Application.Common.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Tags.Repositories;

/// <summary>
/// Read-only repository interface for Tag entities.
/// </summary>
public interface ITagReadOnlyRepository : IReadOnlyRepository<Tag, long>
{
    /// <summary>
    /// Gets a tag by its key and project ID.
    /// </summary>
    Task<Tag?> GetByKeyAsync(long projectId, string key, CancellationToken token = default);

    /// <summary>
    /// Checks if a tag exists by its key and project ID.
    /// </summary>
    Task<bool> ExistsByKeyAsync(long projectId, string key, CancellationToken token = default);

    /// <summary>
    /// Gets all tags for a specific project.
    /// </summary>
    Task<List<Tag>> GetByProjectIdAsync(long projectId, CancellationToken token = default);
}
