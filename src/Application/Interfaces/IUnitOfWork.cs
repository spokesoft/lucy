using Lucy.Application.Comments.Repositories;
using Lucy.Application.Iterations.Repositories;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Sequences.Repositories;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tickets.Repositories;
using Lucy.Application.TicketTags.Repositories;

namespace Lucy.Application.Interfaces;

/// <summary>
/// Unit of Work interface for managing repositories and committing changes.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Repository for Project entities.
    /// </summary>
    public IProjectRepository Projects { get; }

    /// <summary>
    /// Repository for Status entities.
    /// </summary>
    public IStatusRepository Statuses { get; }

    /// <summary>
    /// Repository for Tag entities.
    /// </summary>
    public ITagRepository Tags { get; }

    /// <summary>
    /// Repository for Ticket entities.
    /// </summary>
    public ITicketRepository Tickets { get; }

    /// <summary>
    /// Repository for TicketTag entities.
    /// </summary>
    public ITicketTagRepository TicketTags { get; }

    /// <summary>
    /// Repository for Sequence entities.
    /// </summary>
    public ISequenceRepository Sequences { get; }

    /// <summary>
    /// Repository for Comment entities.
    /// </summary>
    public ICommentRepository Comments { get; }

    /// <summary>
    /// Repository for Iteration entities.
    /// </summary>
    public IIterationRepository Iterations { get; }

    /// <summary>
    /// Saves all changes made in the context to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken token = default);
}
