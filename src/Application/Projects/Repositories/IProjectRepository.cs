using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Projects.Repositories;

/// <summary>
/// Repository interface for Project entities.
/// </summary>
public interface IProjectRepository : IRepository<Project, long>, IProjectReadOnlyRepository
{
}
