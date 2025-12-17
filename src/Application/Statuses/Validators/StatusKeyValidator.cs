using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;
using Lucy.Domain.Entities;

namespace Lucy.Application.Statuses.Validators;

/// <summary>
/// Validator for status keys.
/// </summary>
public class StatusKeyValidator : IValidator<(string Key, IEnumerable<Status> Existing)>
{
    /// <summary>
    /// Asynchronously validates the given status key.
    /// </summary>
    public ValidationResult Validate((string Key, IEnumerable<Status> Existing) value)
    {
        var (key, existing) = value;
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(key))
            return ValidationResult.Error(ValidationCode.StatusKeyRequired, "Key");

        if (!char.IsLetter(key[0]))
            result.AddError(ValidationCode.StatusKeyStartWithLetter, "Key", key);

        if (key.Length > 15)
            result.AddError(ValidationCode.StatusKeyLength, "Key", key.Length);

        if (!key.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
            result.AddError(ValidationCode.StatusKeyInvalidCharacters, "Key", key);

        if (result.IsValid && existing.Any(s => s.Key.Equals(key, StringComparison.OrdinalIgnoreCase)))
            result.AddError(ValidationCode.StatusKeyExists, "Key", key);

        return result;
    }
}
