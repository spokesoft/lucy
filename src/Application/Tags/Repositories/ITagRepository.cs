using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Tags.Repositories;

/// <summary>
/// Repository interface for Tag entities.
/// </summary>
public interface ITagRepository : IRepository<Tag, long>, ITagReadOnlyRepository
{
}
