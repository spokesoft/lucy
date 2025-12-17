using Lucy.Application.Common.Interfaces;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;

namespace Lucy.Application.Sequences.Repositories;

/// <summary>
/// Read-only repository interface for Sequence entities.
/// </summary>
public interface ISequenceReadOnlyRepository : IReadOnlyRepository<Sequence, long>
{
    /// <summary>
    /// Gets a sequence by its type and project ID.
    /// </summary>
    Task<Sequence?> GetByTypeAsync(long projectId, SequenceType type, CancellationToken token = default);
}
