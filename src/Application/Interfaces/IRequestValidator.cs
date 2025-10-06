namespace Lucy.Application.Interfaces;

/// <summary>
/// Validator interface for requests.
/// </summary>
public interface IRequestValidator<TRequest>
    where TRequest : IRequestBase
{
    /// <summary>
    /// Validates the request asynchronously.
    /// </summary>
    Task ValidateAsync(TRequest request, CancellationToken token = default);
}
