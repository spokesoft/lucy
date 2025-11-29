using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.TicketTags.Repositories;

/// <summary>
/// Read-only repository interface for TicketTag entity.
/// </summary>
public interface ITicketTagRepository : IRepository<TicketTag, long>, ITicketTagReadOnlyRepository
{
}
