using Lucy.Application.Comments.Repositories;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Sequences.Repositories;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tickets.Repositories;
using Lucy.Application.TicketTags.Repositories;

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
    /// Read-only repository for Tag entities.
    /// </summary>
    public ITagReadOnlyRepository Tags { get; }

    /// <summary>
    /// Read-only repository for Ticket entities.
    /// </summary>
    public ITicketReadOnlyRepository Tickets { get; }

    /// <summary>
    /// Read-only repository for TicketTag entities.
    /// </summary>
    public ITicketTagReadOnlyRepository TicketTags { get; }

    /// <summary>
    /// Read-only repository for Sequence entities.
    /// </summary>
    public ISequenceReadOnlyRepository Sequences { get; }

    /// <summary>
    /// Read-only repository for Comment entities.
    /// </summary>
    public ICommentReadOnlyRepository Comments { get; }
}
