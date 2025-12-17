using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Tags.Validators;

public class TagDescriptionValidator : IValidator<string?>
{
    public ValidationResult Validate(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return ValidationResult.Success; // Description is optional

        if (description.Length > 500)
            return ValidationResult.Error(
                ValidationCode.TagDescriptionLength,
                "Description",
                description.Length);

        return ValidationResult.Success;
    }
}
