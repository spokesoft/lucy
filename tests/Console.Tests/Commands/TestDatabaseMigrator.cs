using Lucy.Application.Interfaces;

namespace Lucy.Console.Tests.Commands;

/// <summary>
/// Test database migrator for unit testing purposes.
/// </summary>
public class TestDatabaseMigrator : IDatabaseMigrator
{
    public string Name { get; set; } = "TestDatabase";
    public bool WasCalled { get; private set; }
    public bool MigrationRequired { get; set; } = false;
    public bool MigrateWasCalled { get; private set; }
    public Exception? ExceptionToThrow { get; set; }
    public TimeSpan Delay { get; set; } = TimeSpan.Zero;

    public async Task<bool> IsMigrationRequiredAsync(CancellationToken token = default)
    {
        WasCalled = true;

        if (Delay > TimeSpan.Zero)
        {
            await Task.Delay(Delay, token);
        }

        if (ExceptionToThrow != null)
        {
            throw ExceptionToThrow;
        }

        return MigrationRequired;
    }

    public async Task MigrateAsync(CancellationToken token = default)
    {
        MigrateWasCalled = true;

        if (Delay > TimeSpan.Zero)
        {
            await Task.Delay(Delay, token);
        }

        if (ExceptionToThrow != null)
        {
            throw ExceptionToThrow;
        }
    }

    public void Reset()
    {
        WasCalled = false;
        MigrationRequired = false;
        MigrateWasCalled = false;
        ExceptionToThrow = null;
        Delay = TimeSpan.Zero;
    }
}
