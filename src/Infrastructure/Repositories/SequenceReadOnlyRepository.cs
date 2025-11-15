using Lucy.Application.Sequences.Repositories;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Lucy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Repositories;

/// <summary>
/// Sequence read-only repository implementation
/// </summary>
public class SequenceReadOnlyRepository(
    LucyReadContext context) : ReadOnlyRepositoryBase<Sequence, long>(context), ISequenceReadOnlyRepository
{
    /// <summary>
    /// Gets a sequence by its type and project ID.
    /// </summary>
    public Task<Sequence?> GetByTypeAsync(long projectId, SequenceType type, CancellationToken token = default)
        => _set.FirstOrDefaultAsync(sequence => sequence.ProjectId == projectId && sequence.Type == type, token);
}
