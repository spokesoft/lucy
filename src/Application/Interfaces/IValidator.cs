using Lucy.Application.Validation;

namespace Lucy.Application.Interfaces;

/// <summary>
/// Generic validator interface.
/// </summary>
public interface IValidator<T>
{
    /// <summary>
    /// Validates the given state.
    /// </summary>
    ValidationResult Validate(T state);
}
