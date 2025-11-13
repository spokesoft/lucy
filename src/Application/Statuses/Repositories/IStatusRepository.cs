using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Statuses.Repositories;

/// <summary>
/// Repository interface for Status entities.
/// </summary>
public interface IStatusRepository : IRepository<Status, long>, IStatusReadOnlyRepository
{
}
