using Lucy.Application.Comments.Validators;
using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Comments.Commands.UpdateComment;

/// <summary>
/// Validator for the UpdateCommentCommand.
/// </summary>
public class UpdateCommentCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<UpdateCommentCommand>
{
    /// <summary>
    /// The unit of work for read-only operations.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Validator for comment content.
    /// </summary>
    private readonly CommentContentValidator _contentValidator = new();

    /// <summary>
    /// Asynchronously validates the given instance of UpdateCommentCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(UpdateCommentCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Validate comment exists
        if (!await _uow.Comments.ExistsByIdAsync(request.Id, token))
        {
            result.AddError(ValidationCode.CommentNotFound, "Id", request.Id);
            return result;
        }

        // Validate Content
        result.AddResult(_contentValidator.Validate(request.Content));

        return result;
    }
}
