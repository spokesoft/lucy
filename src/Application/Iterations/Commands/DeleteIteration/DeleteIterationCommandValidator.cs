using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Iterations.Commands.DeleteIteration;

/// <summary>
/// Validator for the DeleteIterationCommand.
/// </summary>
public class DeleteIterationCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<DeleteIterationCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously validates the DeleteIterationCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(DeleteIterationCommand request, CancellationToken token = default)
    {
        if (!await _uow.Iterations.ExistsByIdAsync(request.Id, token))
            return ValidationResult.Error(ValidationCode.IterationNotFound, "Id", request.Id);

        return ValidationResult.Success;
    }
}
