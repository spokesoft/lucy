using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Comments.Commands.UpdateComment;

/// <summary>
/// Command to update a comment.
/// </summary>
/// <param name="Id">The unique identifier of the comment to be updated.</param>
/// <param name="Content">The new content of the comment.</param>
public record UpdateCommentCommand(
    long Id,
    string Content) : IRequest;
