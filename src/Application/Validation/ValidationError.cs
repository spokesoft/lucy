namespace Lucy.Application.Validation;

/// <summary>
/// Represents a validation error.
/// </summary>
/// <param name="Code">Validation code representing the error type.</param>
/// <param name="PropertyName">Name of the property associated with the error.</param>
/// <param name="Parameters">Additional parameters related to the error.</param>
public record ValidationError(
    ValidationCode Code,
    string? PropertyName = null,
    object[]? Parameters = null);
