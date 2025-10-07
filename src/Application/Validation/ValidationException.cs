namespace Lucy.Application.Validation;

/// <summary>
/// Exception thrown when validation errors occur.
/// </summary>
/// <param name="result"></param>
public class ValidationException(ValidationResult? result = null)
    : Exception("One or more validation errors occurred.")
{
    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public IEnumerable<ValidationError> Errors { get; } = result?.Errors ?? [];
}
