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
    /// Creates a validation result representing an error.
    /// </summary>
    public static ValidationResult Error(string message)
    {
        var result = new ValidationResult();
        result.AddError(message);
        return result;
    }

    /// <inheritdoc cref="Error" />
    public static ValidationResult Error<T>(T code)
        where T : Enum => Error(code.ToString());

    /// <inheritdoc cref="Error" />
    public static ValidationResult Error(string message, params object[] args)
    {
        var result = new ValidationResult();
        result.AddError(message, args);
        return result;
    }

    /// <inheritdoc cref="Error" />
    public static ValidationResult Error<T>(T code, params object[] args)
        where T : Enum => Error(code.ToString(), args);

    /// <inheritdoc cref="Error" />
    public static ValidationResult Error(string message, string propertyName)
    {
        var result = new ValidationResult();
        result.AddError(message, propertyName);
        return result;
    }

    /// <inheritdoc cref="Error" />
    public static ValidationResult Error<T>(T code, string propertyName)
        where T : Enum => Error(code.ToString(), propertyName);

    /// <inheritdoc cref="Error" />
    public static ValidationResult Error(string message, string propertyName, params object[] args)
    {
        var result = new ValidationResult();
        result.AddError(message, propertyName, args);
        return result;
    }

    /// <inheritdoc cref="Error" />
    public static ValidationResult Error<T>(T code, string propertyName, params object[] args)
        where T : Enum => Error(code.ToString(), propertyName, args);

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

    /// <inheritdoc cref="AddError" />
    public void AddError(string message)
        => AddError(new ValidationError(message));

    /// <inheritdoc cref="AddError" />
    public void AddError<T>(T code)
        where T : Enum => AddError(code.ToString());

    /// <inheritdoc cref="AddError" />
    public void AddError(string message, params object[] args)
        => AddError(new ValidationError(message, null, args));

    /// <inheritdoc cref="AddError" />
    public void AddError<T>(T code, params object[] args)
        where T : Enum => AddError(code.ToString(), args);

    /// <inheritdoc cref="AddError" />
    public void AddError(string message, string propertyName)
        => AddError(new ValidationError(message, propertyName));

    /// <inheritdoc cref="AddError" />
    public void AddError<T>(T code, string propertyName)
        where T : Enum => AddError(code.ToString(), propertyName);

    /// <inheritdoc cref="AddError" />
    public void AddError(string message, string propertyName, params object[] args)
        => AddError(new ValidationError(message, propertyName, args));

    /// <inheritdoc cref="AddError" />
    public void AddError<T>(T code, string propertyName, params object[] args)
        where T : Enum => AddError(code.ToString(), propertyName, args);

    /// <summary>
    /// Adds multiple validation errors to the result.
    /// </summary>
    public void AddErrors(IEnumerable<ValidationError> errors) => _errors.AddRange(errors);

    /// <summary>
    /// Merges another validation result into this one.
    /// </summary>
    public void AddResult(ValidationResult result)
        => _errors.AddRange(result.Errors);
}
