using Lucy.Application.Comments.Repositories;
using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Repositories;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Sequences.Repositories;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tickets.Repositories;
using Lucy.Application.TicketTags.Repositories;
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
    /// Tag read-only repository
    /// </summary>
    public ITagReadOnlyRepository Tags { get; } = new TagReadOnlyRepository(context);

    /// <summary>
    /// Ticket read-only repository
    /// </summary>
    public ITicketReadOnlyRepository Tickets { get; } = new TicketReadOnlyRepository(context);

    /// <summary>
    /// TicketTag read-only repository
    /// </summary>
    public ITicketTagReadOnlyRepository TicketTags { get; } = new TicketTagReadOnlyRepository(context);

    /// <summary>
    /// Sequence read-only repository
    /// </summary>
    public ISequenceReadOnlyRepository Sequences { get; } = new SequenceReadOnlyRepository(context);

    /// <summary>
    /// Comment read-only repository
    /// </summary>
    public ICommentReadOnlyRepository Comments { get; } = new CommentReadOnlyRepository(context);

    /// <summary>
    /// Iteration read-only repository
    /// </summary>
    public IIterationReadOnlyRepository Iterations { get; } = new IterationReadOnlyRepository(context);
}
