using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.TicketTags.Repositories;

/// <summary>
/// Read-only repository interface for TicketTag entity.
/// </summary>
public interface ITicketTagReadOnlyRepository : IReadOnlyRepository<TicketTag, long>
{
    /// <summary>
    /// Gets a TicketTag by ticket ID and tag ID.
    /// </summary>
    public Task<TicketTag?> GetByTicketAndTagAsync(long ticketId, long tagId, CancellationToken token = default);
}
