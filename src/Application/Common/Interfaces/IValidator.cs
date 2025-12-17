using Lucy.Application.Common.Validation;

namespace Lucy.Application.Common.Interfaces;

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
