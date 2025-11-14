using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Statuses.Commands.DeleteStatus;

/// <summary>
/// Validator for the DeleteStatusCommand.
/// </summary>
public class DeleteStatusCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<DeleteStatusCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously validates the DeleteStatusCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(DeleteStatusCommand request, CancellationToken token = default)
    {
        if (!await _uow.Statuses.ExistsByIdAsync(request.Id, token))
            return ValidationResult.Error(ValidationCode.StatusNotFound, "Id", request.Id);

        return ValidationResult.Success;
    }
}
