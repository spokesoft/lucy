using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lucy.Infrastructure.Database;

/// <summary>
/// Factory for creating LucyDbContext instances (at design time).
/// </summary>
/// <remarks>
/// This is used by EF tools for creating migrations. This connection string
/// does not need to match the runtime configuration. It is only used to
/// create a context for design-time operations.
/// </remarks>
public class LucyDbContextFactory : IDesignTimeDbContextFactory<LucyDbContext>
{
    public LucyDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<LucyDbContext>();
        builder.UseSqlite("Data Source=lucy.db; Mode=ReadWriteCreate;");

        return new LucyDbContext(builder.Options);
    }
}
