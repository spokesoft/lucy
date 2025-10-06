namespace Lucy.Application.Interfaces;

/// <summary>
/// Marker interface for request handlers.
/// </summary>
public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Handles the request asynchronously.
    /// </summary>
    Task HandleAsync(TRequest request, CancellationToken token = default);
}

/// <summary>
/// Marker interface for request handlers with a response type.
/// </summary>
public interface IRequestHandler<in TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    /// <summary>
    /// Handles the request asynchronously and returns a result.
    /// </summary>
    Task<TResult> HandleAsync(TRequest request, CancellationToken token = default);
}
