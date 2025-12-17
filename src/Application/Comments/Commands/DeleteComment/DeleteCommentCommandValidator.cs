using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Comments.Commands.DeleteComment;

/// <summary>
/// Validator for the DeleteCommentCommand.
/// </summary>
public class DeleteCommentCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<DeleteCommentCommand>
{
    /// <summary>
    /// The unit of work for read-only operations.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously validates the DeleteCommentCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(DeleteCommentCommand request, CancellationToken token = default)
    {
        if (!await _uow.Comments.ExistsByIdAsync(request.Id, token))
            return ValidationResult.Error(ValidationCode.CommentNotFound, "Id", request.Id);

        return ValidationResult.Success;
    }
}
