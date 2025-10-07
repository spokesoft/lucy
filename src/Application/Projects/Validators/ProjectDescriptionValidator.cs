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
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(description))
            return result; // Description is optional

        if (description.Length > 500)
            result.AddError(new ValidationError(
                ValidationCode.ProjectDescriptionLength,
                nameof(description),
                [description.Length]));

        return result;
    }
}
