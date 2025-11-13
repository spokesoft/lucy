using Lucy.Application.Interfaces;
using Lucy.Application.Statuses.Validators;
using Lucy.Application.Validation;

namespace Lucy.Application.Statuses.Commands.CreateStatus;

/// <summary>
/// Validator for the CreateStatusCommand.
/// </summary>
public class CreateStatusCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<CreateStatusCommand>
{
    /// <summary>
    /// The unit of work for read-only operations.
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
    /// Asynchronously validates the given instance of CreateStatusCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(CreateStatusCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (!await _unitOfWork.Projects.ExistsByIdAsync(request.ProjectId, token))
        {
            result.AddError(ValidationCode.ProjectNotFound, "ProjectId", request.ProjectId);
            return result;
        }

        var existing = await _unitOfWork.Statuses.GetByProjectIdAsync(request.ProjectId, token);

        result.AddResult(_keyValidator.Validate((request.Key, existing)));
        result.AddResult(_nameValidator.Validate(request.Name));
        result.AddResult(_descriptionValidator.Validate(request.Description));
        result.AddResult(_orderValidator.Validate((request.Order, existing)));

        return result;
    }
}
