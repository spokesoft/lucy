using Lucy.Application.Common.Interfaces;
using Lucy.Application.Tags.Validators;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Tags.Commands.CreateTag;

/// <summary>
/// Validator for the CreateTagCommand.
/// </summary>
public class CreateTagCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<CreateTagCommand>
{
    /// <summary>
    /// The unit of work for read-only operations.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _unitOfWork = unitOfWork;
    /// <summary>
    /// Validator for tag keys.
    /// </summary>
    private readonly TagKeyValidator _keyValidator = new();
    /// <summary>
    /// Validator for tag names.
    /// </summary>
    private readonly TagLabelValidator _labelValidator = new();
    /// <summary>
    /// Validator for tag descriptions.
    /// </summary>
    private readonly TagDescriptionValidator _descriptionValidator = new();

    /// <summary>
    /// Asynchronously validates the given instance of CreateTagCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(CreateTagCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (!await _unitOfWork.Projects.ExistsByIdAsync(request.ProjectId, token))
        {
            result.AddError(ValidationCode.ProjectNotFound, "ProjectId", request.ProjectId);
            return result;
        }

        if (await _unitOfWork.Tags.ExistsByKeyAsync(request.ProjectId, request.Key, token))
        {
            result.AddError(ValidationCode.TagKeyExists, "Key", request.Key);
            return result;
        }

        result.AddResult(_keyValidator.Validate(request.Key));
        result.AddResult(_labelValidator.Validate(request.Label));
        result.AddResult(_descriptionValidator.Validate(request.Description));

        return result;
    }
}
