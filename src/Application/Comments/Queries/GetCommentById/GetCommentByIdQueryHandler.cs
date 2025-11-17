using Lucy.Application.Comments.DTOs;
using Lucy.Application.Interfaces;

namespace Lucy.Application.Comments.Queries.GetCommentById;

/// <summary>
/// Handler for the GetCommentByIdQuery.
/// </summary>
public class GetCommentByIdQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<GetCommentByIdQuery, CommentDto?>
{
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the GetCommentByIdQuery.
    /// </summary>
    public async Task<CommentDto?> HandleAsync(GetCommentByIdQuery request, CancellationToken token = default)
    {
        var comment = await _uow.Comments.GetByIdAsync(request.Id, token);

        if (comment is null)
            return null;

        return comment switch
        {
            Domain.Entities.ProjectComment pc => new ProjectCommentDto
            {
                Id = pc.Id,
                Content = pc.Content,
                CreatedAt = pc.CreatedAt,
                UpdatedAt = pc.UpdatedAt,
                ProjectId = pc.ProjectId
            },
            Domain.Entities.TicketComment tc => new TicketCommentDto
            {
                Id = tc.Id,
                Content = tc.Content,
                CreatedAt = tc.CreatedAt,
                UpdatedAt = tc.UpdatedAt,
                TicketId = tc.TicketId
            },
            _ => throw new InvalidOperationException("Unknown comment type.")
        };
    }
}
