using Lucy.Application.Common.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Comments.Repositories;

/// <summary>
/// Repository interface for Comment entities.
/// </summary>
public interface ICommentRepository : IRepository<Comment, long>, ICommentReadOnlyRepository
{
}
