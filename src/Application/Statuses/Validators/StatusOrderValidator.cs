using Lucy.Application.Interfaces;
using Lucy.Application.Validation;
using Lucy.Domain.Entities;

namespace Lucy.Application.Statuses.Validators;

/// <summary>
/// Validator for status order.
/// </summary>
public class StatusOrderValidator : IValidator<(int? Order, IEnumerable<Status> Existing)>
{
    /// <summary>
    /// Validates the given status order.
    /// </summary>
    public ValidationResult Validate((int? Order, IEnumerable<Status> Existing) value)
    {
        var (order, existing) = value;

        if (order == null)
            return ValidationResult.Success;

        if (order < 0)
            return ValidationResult.Error(
                ValidationCode.StatusOrderInvalid,
                "Order",
                order);

        if (order > existing.Count())
            return ValidationResult.Error(
                ValidationCode.StatusOrderInvalid,
                "Order",
                order);

        return ValidationResult.Success;
    }
}
