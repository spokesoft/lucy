using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Comments.Commands.UpdateComment;

/// <summary>
/// Handler for the UpdateCommentCommand.
/// </summary>
public class UpdateCommentCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCommentCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the UpdateCommentCommand.
    /// </summary>
    public async Task HandleAsync(UpdateCommentCommand request, CancellationToken token = default)
    {
        var comment = await _uow.Comments.GetByIdAsync(request.Id, token)
            ?? throw new InvalidOperationException("Comment not found, cannot update.");

        comment.UpdateContent(request.Content);

        _uow.Comments.Update(comment);
        await _uow.SaveChangesAsync(token);
    }
}
