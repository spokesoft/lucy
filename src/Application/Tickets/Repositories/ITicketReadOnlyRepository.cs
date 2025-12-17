using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Queries;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries;
using Lucy.Domain.Entities;

namespace Lucy.Application.Tickets.Repositories;

/// <summary>
/// Read-only repository interface for Ticket entities.
/// </summary>
public interface ITicketReadOnlyRepository : IReadOnlyRepository<Ticket, long>
{
    /// <summary>
    /// Gets a ticket by its key.
    /// </summary>
    Task<Ticket?> GetByKeyAsync(string key, CancellationToken token = default);

    /// <summary>
    /// Checks if a ticket exists by its key.
    /// </summary>
    Task<bool> ExistsByKeyAsync(string key, CancellationToken token = default);

    /// <summary>
    /// Searches for tickets with various filters.
    /// </summary>
    Task<List<Ticket>> SearchAsync(
        long projectId,
        long? statusId = null,
        long? tagId = null,
        long? iterationId = null,
        TicketField sortBy = TicketField.Id,
        SortDirection sortDirection = SortDirection.Ascending,
        CancellationToken token = default);

    /// <summary>
    /// Gets all tickets for a specific project.
    /// </summary>
    Task<List<Ticket>> GetByProjectIdAsync(long projectId, CancellationToken token = default);

    /// <summary>
    /// Gets all tickets for a specific project with sorting.
    /// </summary>
    Task<List<Ticket>> GetByProjectIdAsync(long projectId, TicketField sortBy, SortDirection sortDirection, CancellationToken token = default);

    /// <summary>
    /// Gets all tickets for a specific project and status.
    /// </summary>
    Task<List<Ticket>> GetByProjectIdAndStatusIdAsync(long projectId, long statusId, CancellationToken token = default);

    /// <summary>
    /// Gets all tickets for a specific project and status with sorting.
    /// </summary>
    Task<List<Ticket>> GetByProjectIdAndStatusIdAsync(long projectId, long statusId, TicketField sortBy, SortDirection sortDirection, CancellationToken token = default);

    /// <summary>
    /// Gets all tickets for a specific project and tag with sorting.
    /// </summary>
    Task<List<Ticket>> GetByProjectIdAndTagIdAsync(long projectId, long tagId, TicketField sortBy, SortDirection sortDirection, CancellationToken token = default);

    /// <summary>
    /// Gets all tickets for a specific project, status, and tag with sorting.
    /// </summary>
    Task<List<Ticket>> GetByProjectIdStatusIdAndTagIdAsync(long projectId, long statusId, long tagId, TicketField sortBy, SortDirection sortDirection, CancellationToken token = default);

    /// <summary>
    /// Gets all tickets for a specific status.
    /// </summary>
    Task<List<Ticket>> GetByStatusIdAsync(long statusId, CancellationToken token = default);

    /// <summary>
    /// Gets all tickets for a specific status with sorting.
    /// </summary>
    Task<List<Ticket>> GetByStatusIdAsync(long statusId, TicketField sortBy, SortDirection sortDirection, CancellationToken token = default);

    /// <summary>
    /// Gets all tickets with sorting.
    /// </summary>
    Task<List<Ticket>> GetAllAsync(TicketField sortBy, SortDirection sortDirection, CancellationToken token = default);

    /// <summary>
    /// Gets ticket counts by status for a specific project.
    /// </summary>
    Task<IEnumerable<TicketCountByStatusDto>> GetTicketCountsByProjectIdAsync(long projectId, CancellationToken token = default);

    /// <summary>
    /// Gets ticket counts by status for a specific iteration.
    /// </summary>
    Task<IEnumerable<TicketCountByStatusDto>> GetTicketCountsByIterationIdAsync(long iterationId, CancellationToken token = default);
}
