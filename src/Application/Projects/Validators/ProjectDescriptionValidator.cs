using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Projects.Validators;

/// <summary>
/// Validator for project descriptions.
/// </summary>
public class ProjectDescriptionValidator : IValidator<string?>
{
    /// <summary>
    /// Validates the given project description.
    /// </summary>
    public ValidationResult Validate(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return ValidationResult.Success; // Description is optional

        if (description.Length > 500)
            return ValidationResult.Error(
                ValidationCode.ProjectDescriptionLength,
                nameof(description),
                description.Length);

        return ValidationResult.Success;
    }
}
