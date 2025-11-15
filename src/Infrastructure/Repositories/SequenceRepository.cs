using Lucy.Application.Sequences.Repositories;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Lucy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Repositories;

/// <summary>
/// Sequence repository implementation
/// </summary>
public class SequenceRepository(
    LucyWriteContext context) : RepositoryBase<Sequence, long>(context), ISequenceRepository
{
    /// <summary>
    /// Gets a sequence by its type and project ID.
    /// </summary>
    public Task<Sequence?> GetByTypeAsync(long projectId, SequenceType type, CancellationToken token = default)
        => _set.FirstOrDefaultAsync(sequence => sequence.ProjectId == projectId && sequence.Type == type, token);
}
