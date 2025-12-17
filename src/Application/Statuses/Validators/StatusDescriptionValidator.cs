using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Statuses.Validators;

/// <summary>
/// Validator for status descriptions.
/// </summary>
public class StatusDescriptionValidator : IValidator<string?>
{
    public ValidationResult Validate(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return ValidationResult.Success;

        if (description.Length > 100)
            return ValidationResult.Error(
                ValidationCode.StatusDescriptionLength,
                "Description",
                description.Length);

        return ValidationResult.Success;
    }
}
