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
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(name))
            return result; // Name is optional

        if (name.Length > 100)
            result.AddError(new ValidationError(
                ValidationCode.ProjectNameLength,
                nameof(name),
                [name.Length]));

        return result;
    }
}
