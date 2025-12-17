using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Comments.Commands.DeleteComment;

/// <summary>
/// Handler for the DeleteCommentCommand.
/// </summary>
public class DeleteCommentCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCommentCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the DeleteCommentCommand.
    /// </summary>
    public async Task HandleAsync(DeleteCommentCommand request, CancellationToken token = default)
    {
        var comment = await _uow.Comments.GetByIdAsync(request.Id, token)
            ?? throw new InvalidOperationException("Comment not found, cannot delete.");

        _uow.Comments.Remove(comment);
        await _uow.SaveChangesAsync(token);
    }
}
