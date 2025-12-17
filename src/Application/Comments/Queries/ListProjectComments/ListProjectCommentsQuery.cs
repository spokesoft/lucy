using Lucy.Application.Comments.DTOs;
using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Comments.Queries.ListProjectComments;

/// <summary>
/// Query to list all comments for a project.
/// </summary>
/// <param name="ProjectId">The ID of the project.</param>
public record ListProjectCommentsQuery(long ProjectId) : IRequest<List<ProjectCommentDto>>;
