using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Statuses.Validators;

/// <summary>
/// Validator for status names.
/// </summary>
public class StatusNameValidator : IValidator<string?>
{
    /// <summary>
    /// Validates the given status name.
    /// </summary>
    public ValidationResult Validate(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ValidationResult.Success; // Name is optional

        if (name.Length > 50)
            return ValidationResult.Error(
                ValidationCode.StatusNameLength,
                "Name",
                name.Length);

        return ValidationResult.Success;
    }
}
