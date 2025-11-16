using Lucy.Application.Comments.DTOs;
using Lucy.Application.Interfaces;

namespace Lucy.Application.Comments.Queries.ListProjectComments;

/// <summary>
/// Handler for listing all comments for a project.
/// </summary>
public class ListProjectCommentsQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<ListProjectCommentsQuery, List<ProjectCommentDto>>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to list all comments for a project.
    /// </summary>
    public Task<List<ProjectCommentDto>> HandleAsync(ListProjectCommentsQuery request, CancellationToken token = default)
        => _uow.Comments.GetProjectCommentsAsync(request.ProjectId, token);
}
