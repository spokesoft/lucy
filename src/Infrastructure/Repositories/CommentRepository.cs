using Lucy.Application.Comments.DTOs;
using Lucy.Application.Comments.Repositories;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Lucy.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Repositories;

/// <summary>
/// Comment repository implementation
/// </summary>
public class CommentRepository(
    LucyWriteContext context) : RepositoryBase<Comment, long>(context), ICommentRepository
{
    /// <summary>
    /// Gets all comments for a project.
    /// </summary>
    public async Task<List<ProjectCommentDto>> GetProjectCommentsAsync(long projectId, CancellationToken token = default)
    {
        return await _set
            .OfType<ProjectComment>()
            .Where(c => c.ProjectId == projectId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new ProjectCommentDto
            {
                Id = c.Id,
                Content = c.Content,
                CommentType = CommentType.Project,
                ProjectId = c.ProjectId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(token);
    }

    /// <summary>
    /// Gets all comments for a ticket.
    /// </summary>
    public async Task<List<TicketCommentDto>> GetTicketCommentsAsync(long ticketId, CancellationToken token = default)
    {
        return await _set
            .OfType<TicketComment>()
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new TicketCommentDto
            {
                Id = c.Id,
                Content = c.Content,
                CommentType = CommentType.Ticket,
                TicketId = c.TicketId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .ToListAsync(token);
    }
}
