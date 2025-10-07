using Lucy.Application.Validation;

namespace Lucy.Application.Interfaces;

/// <summary>
/// Generic validator interface.
/// </summary>
public interface IAsyncValidator<T>
{
    /// <summary>
    /// Validates the given state asynchronously.
    /// </summary>
    Task<ValidationResult> ValidateAsync(T state, CancellationToken token = default);
}
