using Lucy.Application.Interfaces;
using Lucy.Application.Tags.Validators;
using Lucy.Application.Validation;

namespace Lucy.Application.Tags.Commands.UpdateTag;

/// <summary>
/// Validator for the UpdateTagCommand.
/// </summary>
public class UpdateTagCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<UpdateTagCommand>
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
    /// Validator for tag labels.
    /// </summary>
    private readonly TagLabelValidator _labelValidator = new();
    /// <summary>
    /// Validator for tag descriptions.
    /// </summary>
    private readonly TagDescriptionValidator _descriptionValidator = new();

    /// <summary>
    /// Asynchronously validates the given instance of UpdateTagCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(UpdateTagCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        var tag = await _unitOfWork.Tags.GetByIdAsync(request.Id, token);
        if (tag == null)
        {
            result.AddError(ValidationCode.TagNotFound, "Id", request.Id);
            return result;
        }

        if (request.Key != null)
        {
            if (await _unitOfWork.Tags.ExistsByKeyAsync(tag.ProjectId, request.Key, token) && !string.Equals(tag.Key, request.Key, StringComparison.OrdinalIgnoreCase))
            {
                result.AddError(ValidationCode.TagKeyExists, "Key", request.Key);
                return result;
            }
            result.AddResult(_keyValidator.Validate(request.Key));
        }

        if (request.Label != null)
            result.AddResult(_labelValidator.Validate(request.Label));

        if (request.Description != null)
            result.AddResult(_descriptionValidator.Validate(request.Description));

        return result;
    }
}
