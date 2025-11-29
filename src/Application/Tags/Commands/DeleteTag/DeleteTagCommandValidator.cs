using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Tags.Commands.DeleteTag;

/// <summary>
/// Validator for the DeleteTagCommand.
/// </summary>
public class DeleteTagCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<DeleteTagCommand>
{
    /// <summary>
    /// The unit of work for read-only operations.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Asynchronously validates the DeleteTagCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(DeleteTagCommand request, CancellationToken token = default)
    {
        if (!await _unitOfWork.Tags.ExistsByIdAsync(request.Id, token))
            return ValidationResult.Error(ValidationCode.TagNotFound, "Id", request.Id);
        return ValidationResult.Success;
    }
}
