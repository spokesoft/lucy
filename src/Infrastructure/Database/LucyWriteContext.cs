using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Database;

/// <summary>
/// The write database context for Lucy.
/// </summary>
public class LucyWriteContext(
    DbContextOptions<LucyDbContext> options) : LucyDbContext(options)
{
    // No additional implementation needed for write context.
}
