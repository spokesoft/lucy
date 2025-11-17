using Lucy.Application.Queries;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries;
using Lucy.Application.Tickets.Repositories;
using Lucy.Domain.Entities;
using Lucy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Repositories;

/// <summary>
/// Ticket read-only repository implementation
/// </summary>
public class TicketReadOnlyRepository(
    LucyReadContext context) : ReadOnlyRepositoryBase<Ticket, long>(context), ITicketReadOnlyRepository
{
    /// <summary>
    /// Gets a ticket by its key.
    /// </summary>
    public Task<Ticket?> GetByKeyAsync(string key, CancellationToken token = default)
        => _set.FirstOrDefaultAsync(ticket => ticket.Key.Equals(key), token);

    /// <summary>
    /// Checks if a ticket exists by its key.
    /// </summary>
    public Task<bool> ExistsByKeyAsync(string key, CancellationToken token = default)
        => _set.AnyAsync(ticket => ticket.Key.Equals(key), token);

    /// <summary>
    /// Gets all tickets for a specific project.
    /// </summary>
    public Task<List<Ticket>> GetByProjectIdAsync(long projectId, CancellationToken token = default)
        => _set.Where(ticket => ticket.ProjectId == projectId).ToListAsync(token);

    /// <summary>
    /// Gets all tickets for a specific project with sorting.
    /// </summary>
    public Task<List<Ticket>> GetByProjectIdAsync(long projectId, TicketSortField sortBy, SortDirection sortDirection, CancellationToken token = default)
    {
        var query = _set.Where(ticket => ticket.ProjectId == projectId);
        return ApplySort(query, sortBy, sortDirection).ToListAsync(token);
    }

    /// <summary>
    /// Gets all tickets for a specific project and status.
    /// </summary>
    public Task<List<Ticket>> GetByProjectIdAndStatusIdAsync(long projectId, long statusId, CancellationToken token = default)
        => _set.Where(ticket => ticket.ProjectId == projectId && ticket.StatusId == statusId).ToListAsync(token);

    /// <summary>
    /// Gets all tickets for a specific project and status with sorting.
    /// </summary>
    public Task<List<Ticket>> GetByProjectIdAndStatusIdAsync(long projectId, long statusId, TicketSortField sortBy, SortDirection sortDirection, CancellationToken token = default)
    {
        var query = _set.Where(ticket => ticket.ProjectId == projectId && ticket.StatusId == statusId);
        return ApplySort(query, sortBy, sortDirection).ToListAsync(token);
    }

    /// <summary>
    /// Gets all tickets for a specific status.
    /// </summary>
    public Task<List<Ticket>> GetByStatusIdAsync(long statusId, CancellationToken token = default)
        => _set.Where(ticket => ticket.StatusId == statusId).ToListAsync(token);

    /// <summary>
    /// Gets all tickets for a specific status with sorting.
    /// </summary>
    public Task<List<Ticket>> GetByStatusIdAsync(long statusId, TicketSortField sortBy, SortDirection sortDirection, CancellationToken token = default)
    {
        var query = _set.Where(ticket => ticket.StatusId == statusId);
        return ApplySort(query, sortBy, sortDirection).ToListAsync(token);
    }

    /// <summary>
    /// Gets all tickets with sorting.
    /// </summary>
    public Task<List<Ticket>> GetAllAsync(TicketSortField sortBy, SortDirection sortDirection, CancellationToken token = default)
    {
        var query = _set.AsQueryable();
        return ApplySort(query, sortBy, sortDirection).ToListAsync(token);
    }

    /// <summary>
    /// Gets ticket counts by status for a specific project.
    /// </summary>
    public async Task<IEnumerable<TicketCountByStatusDto>> GetTicketCountsByProjectIdAsync(long projectId, CancellationToken token = default)
    {
        return await _set
            .Where(t => t.ProjectId == projectId)
            .GroupBy(t => t.Status)
            .Select(g => new TicketCountByStatusDto
            {
                StatusId = g.Key.Id,
                StatusKey = g.Key.Key,
                StatusName = g.Key.Name,
                StatusColor = g.Key.Color.ToString(),
                Count = g.Count()
            })
            .OrderBy(x => x.StatusKey)
            .ToListAsync(token);
    }

    /// <summary>
    /// Applies sorting to a ticket query.
    /// </summary>
    private static IQueryable<Ticket> ApplySort(IQueryable<Ticket> query, TicketSortField sortBy, SortDirection sortDirection)
    {
        return sortBy switch
        {
            TicketSortField.Id => sortDirection == SortDirection.Ascending
                ? query.OrderBy(t => t.Id)
                : query.OrderByDescending(t => t.Id),
            TicketSortField.Key => sortDirection == SortDirection.Ascending
                ? query.OrderBy(t => t.Key)
                : query.OrderByDescending(t => t.Key),
            TicketSortField.Title => sortDirection == SortDirection.Ascending
                ? query.OrderBy(t => t.Title)
                : query.OrderByDescending(t => t.Title),
            TicketSortField.ProjectId => sortDirection == SortDirection.Ascending
                ? query.OrderBy(t => t.ProjectId)
                : query.OrderByDescending(t => t.ProjectId),
            TicketSortField.StatusId => sortDirection == SortDirection.Ascending
                ? query.OrderBy(t => t.StatusId)
                : query.OrderByDescending(t => t.StatusId),
            TicketSortField.CreatedAt => sortDirection == SortDirection.Ascending
                ? query.OrderBy(t => t.CreatedAt)
                : query.OrderByDescending(t => t.CreatedAt),
            TicketSortField.UpdatedAt => sortDirection == SortDirection.Ascending
                ? query.OrderBy(t => t.UpdatedAt)
                : query.OrderByDescending(t => t.UpdatedAt),
            _ => query.OrderBy(t => t.Id)
        };
    }
}
