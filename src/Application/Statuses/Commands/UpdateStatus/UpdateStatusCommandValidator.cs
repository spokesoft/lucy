using Lucy.Application.Common.Interfaces;
using Lucy.Application.Statuses.Validators;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Statuses.Commands.UpdateStatus;

/// <summary>
/// Validator for the UpdateStatusCommand.
/// </summary>
public class UpdateStatusCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<UpdateStatusCommand>
{
    /// <summary>
    /// Read-only unit of work for querying repositories.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Validator for status keys.
    /// </summary>
    private readonly StatusKeyValidator _keyValidator = new();

    /// <summary>
    /// Validator for status names.
    /// </summary>
    private readonly StatusNameValidator _nameValidator = new();

    /// <summary>
    /// Validator for status descriptions.
    /// </summary>
    private readonly StatusDescriptionValidator _descriptionValidator = new();

    /// <summary>
    /// Validator for status order.
    /// </summary>
    private readonly StatusOrderValidator _orderValidator = new();

    /// <summary>
    /// Asynchronously validates the given instance of UpdateStatusCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(UpdateStatusCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        var status = await _unitOfWork.Statuses.GetByIdAsync(request.Id, token);
        if (status == null)
        {
            result.AddError(ValidationCode.StatusNotFound, "Id", request.Id);
            return result;
        }

        var existing = await _unitOfWork.Statuses.GetByProjectIdAsync(status.ProjectId, token);

        if (request.Key != null)
        {
            // Exclude the current status from the duplicate key check
            var otherStatuses = existing.Where(s => s.Id != request.Id);
            result.AddResult(_keyValidator.Validate((request.Key, otherStatuses)));
        }

        if (request.Name != null)
            result.AddResult(_nameValidator.Validate(request.Name));

        if (request.Description != null)
            result.AddResult(_descriptionValidator.Validate(request.Description));

        return result;
    }
}
