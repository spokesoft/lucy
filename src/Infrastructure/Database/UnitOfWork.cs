using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Sequences.Repositories;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tickets.Repositories;
using Lucy.Infrastructure.Repositories;

namespace Lucy.Infrastructure.Database;

/// <summary>
/// Unit of Work implementation
/// </summary>
public class UnitOfWork(
    LucyWriteContext context) : IUnitOfWork
{
    /// <summary>
    /// The database context
    /// </summary>
    private readonly LucyWriteContext _context = context;

    /// <summary>
    /// Project repository
    /// </summary>
    public IProjectRepository Projects { get; } = new ProjectRepository(context);

    /// <summary>
    /// Status repository
    /// </summary>
    public IStatusRepository Statuses { get; } = new StatusRepository(context);

    /// <summary>
    /// Ticket repository
    /// </summary>
    public ITicketRepository Tickets { get; } = new TicketRepository(context);

    /// <summary>
    /// Sequence repository
    /// </summary>
    public ISequenceRepository Sequences { get; } = new SequenceRepository(context);

    /// <summary>
    /// Saves changes to the database asynchronously
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
