using Lucy.Application.Comments.Validators;
using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Comments.Commands.CreateProjectComment;

/// <summary>
/// Validator for the CreateProjectCommentCommand.
/// </summary>
public class CreateProjectCommentCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<CreateProjectCommentCommand>
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
    /// Asynchronously validates the given instance of CreateProjectCommentCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(CreateProjectCommentCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Validate ProjectId exists
        if (!await _uow.Projects.ExistsByIdAsync(request.ProjectId, token))
        {
            result.AddError(ValidationCode.ProjectNotFound, "ProjectId", request.ProjectId);
        }

        // Validate Content
        result.AddResult(_contentValidator.Validate(request.Content));

        return result;
    }
}
