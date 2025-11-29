using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Tags.Validators;

public class TagLabelValidator : IValidator<string?>
{
    public ValidationResult Validate(string? label)
    {
        if (string.IsNullOrWhiteSpace(label))
            return ValidationResult.Success; // Label is optional

        if (label.Length > 50)
            return ValidationResult.Error(
                ValidationCode.TagNameLength,
                "Name",
                label.Length);

        return ValidationResult.Success;
    }
}
