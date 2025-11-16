using Lucy.Application.Comments.DTOs;
using Lucy.Application.Interfaces;

namespace Lucy.Application.Comments.Queries.ListTicketComments;

/// <summary>
/// Query to list all comments for a ticket.
/// </summary>
/// <param name="TicketId">The ID of the ticket.</param>
public record ListTicketCommentsQuery(long TicketId) : IRequest<List<TicketCommentDto>>;
