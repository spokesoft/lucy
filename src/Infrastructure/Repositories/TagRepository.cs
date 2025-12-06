using Lucy.Application.Tags.Repositories;
using Lucy.Domain.Entities;
using Lucy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Repositories;

/// <summary>
/// Status repository implementation
/// </summary>
public class TagRepository(
    LucyWriteContext context) : RepositoryBase<Tag, long>(context), ITagRepository
{
    /// <inheritdoc />
    public Task<Tag?> GetByKeyAsync(long projectId, string key, CancellationToken token = default)
    {
        var normalizedKey = key.ToUpperInvariant();
        return _set.FirstOrDefaultAsync(tag => tag.ProjectId == projectId && tag.Key == normalizedKey, token);
    }

    /// <inheritdoc />
    public Task<bool> ExistsByKeyAsync(long projectId, string key, CancellationToken token = default)
    {
        var normalizedKey = key.ToUpperInvariant();
        return _set.AnyAsync(tag => tag.ProjectId == projectId && tag.Key == normalizedKey, token);
    }

    /// <inheritdoc />
    public Task<List<Tag>> GetByProjectIdAsync(long projectId, CancellationToken token = default)
        => _set.Where(tag => tag.ProjectId == projectId).ToListAsync(token);
}
