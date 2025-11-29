using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Tags.Validators;

public class TagKeyValidator : IValidator<string?>
{
    public ValidationResult Validate(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return ValidationResult.Error(ValidationCode.TagKeyRequired, "Key");

        if (!char.IsLetter(key[0]))
            return ValidationResult.Error(ValidationCode.TagKeyStartWithLetter, "Key", key);

        if (key.Length < 3 || key.Length > 50)
            return ValidationResult.Error(ValidationCode.TagKeyLength, "Key", key.Length);

        if (!key.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
            return ValidationResult.Error(ValidationCode.TagKeyInvalidCharacters, "Key", key);

        return ValidationResult.Success;
    }
}
