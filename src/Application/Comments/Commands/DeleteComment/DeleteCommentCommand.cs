using Lucy.Application.Interfaces;

namespace Lucy.Application.Comments.Commands.DeleteComment;

/// <summary>
/// Command to delete a comment by its ID.
/// </summary>
/// <param name="Id">The unique identifier of the comment to be deleted.</param>
public record DeleteCommentCommand(long Id) : IRequest;
