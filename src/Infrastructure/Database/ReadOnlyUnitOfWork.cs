using Lucy.Application.Comments.Repositories;
using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Sequences.Repositories;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tickets.Repositories;
using Lucy.Infrastructure.Repositories;

namespace Lucy.Infrastructure.Database;

/// <summary>
/// Read-only Unit of Work implementation
/// </summary>
public class ReadOnlyUnitOfWork(
    LucyReadContext context) : IReadOnlyUnitOfWork
{
    /// <summary>
    /// Project read-only repository
    /// </summary>
    public IProjectReadOnlyRepository Projects { get; } = new ProjectReadOnlyRepository(context);

    /// <summary>
    /// Status read-only repository
    /// </summary>
    public IStatusReadOnlyRepository Statuses { get; } = new StatusReadOnlyRepository(context);

    /// <summary>
    /// Ticket read-only repository
    /// </summary>
    public ITicketReadOnlyRepository Tickets { get; } = new TicketReadOnlyRepository(context);

    /// <summary>
    /// Sequence read-only repository
    /// </summary>
    public ISequenceReadOnlyRepository Sequences { get; } = new SequenceReadOnlyRepository(context);

    /// <summary>
    /// Comment read-only repository
    /// </summary>
    public ICommentReadOnlyRepository Comments { get; } = new CommentReadOnlyRepository(context);
}
