using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Comments.Commands.CreateProjectComment;

/// <summary>
/// Command to create a new project comment.
/// </summary>
/// <param name="ProjectId">The ID of the project to comment on.</param>
/// <param name="Content">The content of the comment.</param>
public record CreateProjectCommentCommand(
    long ProjectId,
    string Content) : IRequest<long>;
