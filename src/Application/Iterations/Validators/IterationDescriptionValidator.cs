using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Iterations.Validators;

/// <summary>
/// Validator for iteration descriptions.
/// </summary>
public class IterationDescriptionValidator : IValidator<string?>
{
    /// <summary>
    /// Validates the given iteration description.
    /// </summary>
    public ValidationResult Validate(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return ValidationResult.Success; // Description is optional

        if (description.Length > 500)
            return ValidationResult.Error(
                ValidationCode.IterationDescriptionLength,
                "Description",
                description.Length);

        return ValidationResult.Success;
    }
}
