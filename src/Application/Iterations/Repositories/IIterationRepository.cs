using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Iterations.Repositories;

/// <summary>
/// Repository interface for Iteration entities.
/// </summary>
public interface IIterationRepository : IRepository<Iteration, long>, IIterationReadOnlyRepository
{
}
