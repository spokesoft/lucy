using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Comments.Commands.CreateTicketComment;

/// <summary>
/// Command to create a new ticket comment.
/// </summary>
/// <param name="TicketId">The ID of the ticket to comment on.</param>
/// <param name="Content">The content of the comment.</param>
public record CreateTicketCommentCommand(
    long TicketId,
    string Content) : IRequest<long>;
