using Lucy.Application.Comments.DTOs;
using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Comments.Queries.ListTicketComments;

/// <summary>
/// Handler for listing all comments for a ticket.
/// </summary>
public class ListTicketCommentsQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<ListTicketCommentsQuery, List<TicketCommentDto>>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to list all comments for a ticket.
    /// </summary>
    public Task<List<TicketCommentDto>> HandleAsync(ListTicketCommentsQuery request, CancellationToken token = default)
        => _uow.Comments.GetTicketCommentsAsync(request.TicketId, token);
}
