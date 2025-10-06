using Lucy.Domain.Entities;

namespace Lucy.Application.Interfaces;

/// <summary>
/// Read-only Unit of Work interface for managing read-only repositories.
/// </summary>
public interface IReadOnlyUnitOfWork
{
    /// <summary>
    /// Read-only repository for Project entities.
    /// </summary>
    public IReadOnlyRepository<Project, long> Projects { get; }
}
