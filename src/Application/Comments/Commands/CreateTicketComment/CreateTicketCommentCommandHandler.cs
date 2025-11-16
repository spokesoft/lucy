using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Comments.Commands.CreateTicketComment;

/// <summary>
/// Handler for the CreateTicketCommentCommand.
/// </summary>
public class CreateTicketCommentCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTicketCommentCommand, long>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the CreateTicketCommentCommand.
    /// </summary>
    public async Task<long> HandleAsync(CreateTicketCommentCommand request, CancellationToken token = default)
    {
        var comment = new TicketComment(
            request.TicketId,
            request.Content);

        await _uow.Comments.AddAsync(comment, token);
        await _uow.SaveChangesAsync(token);
        return comment.Id;
    }
}
