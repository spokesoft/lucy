namespace Lucy.Application.Validation;

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public class ValidationResult(
    IEnumerable<ValidationError>? errors = null)
{
    /// <summary>
    /// A static instance representing a successful validation result.
    /// </summary>
    public static readonly ValidationResult Success = new();

    /// <summary>
    /// Internal list of validation errors.
    /// </summary>
    private readonly List<ValidationError> _errors = [.. errors ?? []];

    /// <summary>
    /// Indicates whether the validation was successful (i.e., no errors).
    /// </summary>
    public bool IsValid => _errors.Count == 0;

    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public IReadOnlyList<ValidationError> Errors => _errors;

    /// <summary>
    /// Adds a validation error to the result.
    /// </summary>
    public void AddError(ValidationError error) => _errors.Add(error);

    /// <summary>
    /// Adds multiple validation errors to the result.
    /// </summary>
    /// <param name="result"></param>
    public void AddResult(ValidationResult result)
    {
        if (result is null || result.IsValid) return;
        _errors.AddRange(result.Errors);
    }
}
