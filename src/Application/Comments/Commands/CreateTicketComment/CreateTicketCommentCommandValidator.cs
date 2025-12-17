using Lucy.Application.Comments.Validators;
using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Comments.Commands.CreateTicketComment;

/// <summary>
/// Validator for the CreateTicketCommentCommand.
/// </summary>
public class CreateTicketCommentCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<CreateTicketCommentCommand>
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
    /// Asynchronously validates the given instance of CreateTicketCommentCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(CreateTicketCommentCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Validate TicketId exists
        if (!await _uow.Tickets.ExistsByIdAsync(request.TicketId, token))
        {
            result.AddError(ValidationCode.TicketNotFound, "TicketId", request.TicketId);
        }

        // Validate Content
        result.AddResult(_contentValidator.Validate(request.Content));

        return result;
    }
}
