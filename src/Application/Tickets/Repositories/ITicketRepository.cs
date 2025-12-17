using Lucy.Application.Common.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Tickets.Repositories;

/// <summary>
/// Repository interface for Ticket entities.
/// </summary>
public interface ITicketRepository : IRepository<Ticket, long>, ITicketReadOnlyRepository
{
}
