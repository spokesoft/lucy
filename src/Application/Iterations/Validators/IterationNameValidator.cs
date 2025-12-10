using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Iterations.Validators;

/// <summary>
/// Validator for iteration names.
/// </summary>
public class IterationNameValidator : IValidator<string?>
{
    /// <summary>
    /// Validates the given iteration name.
    /// </summary>
    public ValidationResult Validate(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ValidationResult.Success; // Name is optional

        if (name.Length > 100)
            return ValidationResult.Error(
                ValidationCode.IterationNameLength,
                "Name",
                name.Length);

        return ValidationResult.Success;
    }
}
