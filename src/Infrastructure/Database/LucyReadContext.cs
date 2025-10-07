using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Database;

/// <summary>
/// The read-only database context for Lucy.
/// </summary>
public class LucyReadContext(
    DbContextOptions<LucyDbContext> options) : LucyDbContext(options)
{
    /// <summary>
    /// The exception message for invalid operations.
    /// </summary>
    private const string InvalidOperationMessage = "This context is read-only and does not support save operations.";

    /// <summary>
    /// Configures the context to use no tracking for read-only operations.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        base.OnConfiguring(optionsBuilder);
    }

    /// <summary>
    /// Disables save changes to enforce read-only behavior.
    /// </summary>
    public override int SaveChanges()
        => throw new InvalidOperationException(InvalidOperationMessage);

    /// <summary>
    /// Disables async save changes to enforce read-only behavior.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => throw new InvalidOperationException(InvalidOperationMessage);
}
