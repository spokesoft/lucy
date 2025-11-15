using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Tickets.Validators;

/// <summary>
/// Validator for ticket descriptions.
/// </summary>
public class TicketDescriptionValidator : IValidator<string?>
{
    /// <summary>
    /// Validates the given ticket description.
    /// </summary>
    public ValidationResult Validate(string? value)
    {
        var result = new ValidationResult();

        if (value is not null && value.Length > 5000)
            result.AddError(ValidationCode.TicketDescriptionLength, "Description", value.Length);

        return result;
    }
}
