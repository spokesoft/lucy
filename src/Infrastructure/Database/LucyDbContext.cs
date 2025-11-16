using Lucy.Domain.Entities;
using Lucy.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Database;

/// <summary>
/// The database context for Lucy.
/// </summary>
public class LucyDbContext(DbContextOptions<LucyDbContext> options) : DbContext(options)
{
    /// <summary>
    /// The namespace where the entity configurations are located.
    /// </summary>
    private const string ConfigurationNamespace = "Lucy.Infrastructure.Database.Configurations";

    /// <summary>
    /// Database set of projects.
    /// </summary>
    public DbSet<Project> Projects { get; set; }

    /// <summary>
    /// Database set of statuses.
    /// </summary>
    public DbSet<Status> Statuses { get; set; }

    /// <summary>
    /// Database set of tickets.
    /// </summary>
    public DbSet<Ticket> Tickets { get; set; }

    /// <summary>
    /// Database set of sequences.
    /// </summary>
    public DbSet<Sequence> Sequences { get; set; }

    /// <summary>
    /// Database set of comments (TPH: includes ProjectComment and TicketComment).
    /// </summary>
    public DbSet<Comment> Comments { get; set; }

    /// <summary>
    /// Configures the model by applying all entity configurations from the specified namespace.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromNamespace(
            typeof(LucyDbContext).Assembly,
            ConfigurationNamespace);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Overrides SaveChanges to automatically set timestamps on entities.
    /// </summary>
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        TimestampEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <summary>
    /// Overrides SaveChangesAsync to automatically set timestamps on entities.
    /// </summary>
    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken token = default)
    {
        TimestampEntities();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, token);
    }

    /// <summary>
    /// Sets the CreatedAt and UpdatedAt timestamps on entities that are being added or modified.
    /// </summary>
    private void TimestampEntities()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is DomainEntity &&
                (e.State == EntityState.Added || e.State == EntityState.Modified));

        var utcNow = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            var entity = (DomainEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = utcNow;
            }

            entity.UpdatedAt = utcNow;
        }
    }
}
