using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

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

        if (request.ReassignToId.HasValue
            && !await _uow.Statuses.ExistsByIdAsync(request.ReassignToId.Value, token))
        {
            return ValidationResult.Error(ValidationCode.ReassignStatusNotFound, "ReassignToId", request.ReassignToId);
        }

        if (!string.IsNullOrWhiteSpace(request.ReassignTo))
        {
            var status = await _uow.Statuses.GetByIdAsync(request.Id, token);
            var reassign = await _uow.Statuses.GetByKeyAsync(status!.ProjectId, request.ReassignTo, token);
            if (reassign == null)
            {
                return ValidationResult.Error(ValidationCode.ReassignStatusKeyNotFound, "ReassignTo", request.ReassignTo);
            }
        }

        return ValidationResult.Success;
    }
}
