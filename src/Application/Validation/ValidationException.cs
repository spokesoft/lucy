namespace Lucy.Application.Validation;

/// <summary>
/// Exception thrown when validation errors occur.
/// </summary>
public class ValidationException(ValidationResult result)
    : Exception("One or more validation errors occurred.")
{
    /// <summary>
    /// Gets the validation result containing the errors.
    /// </summary>
    public ValidationResult Result { get; } = result;

    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public IEnumerable<ValidationError> Errors => Result?.Errors ?? [];
}
