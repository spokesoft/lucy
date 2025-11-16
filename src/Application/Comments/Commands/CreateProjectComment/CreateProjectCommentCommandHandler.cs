using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Comments.Commands.CreateProjectComment;

/// <summary>
/// Handler for the CreateProjectCommentCommand.
/// </summary>
public class CreateProjectCommentCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProjectCommentCommand, long>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the CreateProjectCommentCommand.
    /// </summary>
    public async Task<long> HandleAsync(CreateProjectCommentCommand request, CancellationToken token = default)
    {
        var comment = new ProjectComment(
            request.ProjectId,
            request.Content);

        await _uow.Comments.AddAsync(comment, token);
        await _uow.SaveChangesAsync(token);
        return comment.Id;
    }
}
