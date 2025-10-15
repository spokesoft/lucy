namespace Lucy.Application.Interfaces;

/// <summary>
/// Interface for database migration operations.
/// </summary>
public interface IDatabaseMigrator
{
    /// <summary>
    /// The name of the database being migrated.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Migrates the database to the latest version asynchronously.
    /// </summary>
    Task MigrateAsync(CancellationToken token = default);

    /// <summary>
    /// Checks if a migration is required.
    /// </summary>
    Task<bool> IsMigrationRequiredAsync(CancellationToken token = default);
}
