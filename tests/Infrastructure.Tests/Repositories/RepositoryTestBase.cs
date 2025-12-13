using Lucy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lucy.Infrastructure.Tests.Repositories;

/// <summary>
/// Base class for repository tests.
/// </summary>
public abstract class RepositoryTestBase
{
    protected readonly DbContextOptions<LucyDbContext> DbContextOptions;
    protected readonly DbContextOptions<LucyDbContext> _writeDbContextOptions;
    protected readonly DbContextOptions<LucyDbContext> _readDbContextOptions;

    protected RepositoryTestBase()
    {
        var databaseName = Guid.NewGuid().ToString();
        var databaseRoot = new InMemoryDatabaseRoot();

        DbContextOptions = new DbContextOptionsBuilder<LucyDbContext>()
            .UseInMemoryDatabase(databaseName, databaseRoot)
            .EnableServiceProviderCaching(false)
            .Options;

        _writeDbContextOptions = DbContextOptions;
        _readDbContextOptions = DbContextOptions;
    }
}
