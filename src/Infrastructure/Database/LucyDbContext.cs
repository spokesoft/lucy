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
    /// Configures the model by applying all entity configurations from the specified namespace.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromNamespace(
            typeof(LucyDbContext).Assembly,
            ConfigurationNamespace);

        base.OnModelCreating(modelBuilder);
    }
}
