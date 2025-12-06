using Lucy.Application.TicketTags.Repositories;
using Lucy.Domain.Entities;
using Lucy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Repositories;

/// <summary>
/// Read-only repository for TicketTag entities.
/// </summary>
public class TicketTagReadOnlyRepository(
    LucyReadContext context) : ReadOnlyRepositoryBase<TicketTag, long>(context), ITicketTagReadOnlyRepository
{
    /// <inheritdoc />
    public Task<TicketTag?> GetByTicketAndTagAsync(long ticketId, long tagId, CancellationToken token = default)
        => _set.FirstOrDefaultAsync(tt => tt.TicketId == ticketId && tt.TagId == tagId, token);

    /// <inheritdoc />
    public Task<List<Tag>> GetTagsByTicketIdAsync(long ticketId, CancellationToken token = default)
        => _set
            .AsNoTracking()
            .Include(tt => tt.Tag)
            .Where(tt => tt.TicketId == ticketId)
            .Select(tt => tt.Tag)
            .ToListAsync(token);
}
