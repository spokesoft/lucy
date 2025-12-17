using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Tickets.Validators;

/// <summary>
/// Validator for ticket titles.
/// </summary>
public class TicketTitleValidator : IValidator<string?>
{
    /// <summary>
    /// Validates the given ticket title.
    /// </summary>
    public ValidationResult Validate(string? value)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(value))
            return ValidationResult.Error(ValidationCode.TicketTitleRequired, "Title");

        if (value.Length > 200)
            result.AddError(ValidationCode.TicketTitleLength, "Title", value.Length);

        return result;
    }
}
