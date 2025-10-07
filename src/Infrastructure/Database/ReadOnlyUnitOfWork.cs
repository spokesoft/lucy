using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Repositories;
using Lucy.Infrastructure.Repositories;

namespace Lucy.Infrastructure.Database;

/// <summary>
/// Read-only Unit of Work implementation
/// </summary>
public class ReadOnlyUnitOfWork(
    LucyReadContext context) : IReadOnlyUnitOfWork
{
    /// <summary>
    /// Project read-only repository
    /// </summary>
    public IProjectReadOnlyRepository Projects { get; } = new ProjectReadOnlyRepository(context);
}
