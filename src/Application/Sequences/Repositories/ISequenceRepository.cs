using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Sequences.Repositories;

/// <summary>
/// Repository interface for Sequence entities.
/// </summary>
public interface ISequenceRepository : IRepository<Sequence, long>, ISequenceReadOnlyRepository
{
}
