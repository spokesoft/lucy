namespace Lucy.Application.Interfaces;

/// <summary>
/// Interface for database migration operations.
/// </summary>
public interface IDatabaseMigrator
{
    /// <summary>
    /// Migrates the database to the latest version asynchronously.
    /// </summary>
    Task MigrateAsync(CancellationToken token = default);
}
