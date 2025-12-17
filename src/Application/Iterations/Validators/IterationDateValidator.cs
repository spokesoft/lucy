using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Iterations.Validators;

/// <summary>
/// Validator for iteration date ranges.
/// </summary>
public class IterationDateValidator : IValidator<(DateTime? StartDate, DateTime? EndDate)>
{
    /// <summary>
    /// Validates the given iteration date range.
    /// </summary>
    public ValidationResult Validate((DateTime? StartDate, DateTime? EndDate) dates)
    {
        if (dates.StartDate.HasValue && dates.EndDate.HasValue && dates.StartDate > dates.EndDate)
            return ValidationResult.Error(
                ValidationCode.IterationStartDateAfterEndDate,
                "StartDate");

        return ValidationResult.Success;
    }
}
