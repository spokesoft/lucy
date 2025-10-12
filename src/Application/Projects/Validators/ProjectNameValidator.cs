using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Projects.Validators;

/// <summary>
/// Validator for project names.
/// </summary>
public class ProjectNameValidator : IValidator<string?>
{
    /// <summary>
    /// Validates the given project name.
    /// </summary>
    public ValidationResult Validate(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ValidationResult.Success; // Name is optional

        if (name.Length > 500)
            return ValidationResult.Error(
                ValidationCode.ProjectNameLength,
                nameof(name),
                name.Length);

        return ValidationResult.Success;
    }
}
