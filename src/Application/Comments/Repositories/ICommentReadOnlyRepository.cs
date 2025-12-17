using Lucy.Application.Comments.DTOs;
using Lucy.Application.Common.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Comments.Repositories;

/// <summary>
/// Read-only repository interface for Comment entities.
/// </summary>
public interface ICommentReadOnlyRepository : IReadOnlyRepository<Comment, long>
{
    /// <summary>
    /// Gets all comments for a project.
    /// </summary>
    Task<List<ProjectCommentDto>> GetProjectCommentsAsync(long projectId, CancellationToken token = default);

    /// <summary>
    /// Gets all comments for a ticket.
    /// </summary>
    Task<List<TicketCommentDto>> GetTicketCommentsAsync(long ticketId, CancellationToken token = default);
}
