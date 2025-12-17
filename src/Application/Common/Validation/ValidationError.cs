namespace Lucy.Application.Common.Validation;

/// <summary>
/// Represents a validation error.
/// </summary>
public record ValidationError(
    string Message,
    string? PropertyName = null,
    object[]? Parameters = null);
