using Lucy.Application.Common.Validation;

namespace Lucy.Application.Common.Interfaces;

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
