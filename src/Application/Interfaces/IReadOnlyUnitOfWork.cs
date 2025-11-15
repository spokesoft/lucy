using Lucy.Application.Projects.Repositories;
using Lucy.Application.Sequences.Repositories;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tickets.Repositories;

namespace Lucy.Application.Interfaces;

/// <summary>
/// Read-only Unit of Work interface for managing read-only repositories.
/// </summary>
public interface IReadOnlyUnitOfWork
{
    /// <summary>
    /// Read-only repository for Project entities.
    /// </summary>
    public IProjectReadOnlyRepository Projects { get; }

    /// <summary>
    /// Read-only repository for Status entities.
    /// </summary>
    public IStatusReadOnlyRepository Statuses { get; }

    /// <summary>
    /// Read-only repository for Ticket entities.
    /// </summary>
    public ITicketReadOnlyRepository Tickets { get; }

    /// <summary>
    /// Read-only repository for Sequence entities.
    /// </summary>
    public ISequenceReadOnlyRepository Sequences { get; }
}
