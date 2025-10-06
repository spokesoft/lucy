namespace Lucy.Application.Interfaces;

/// <summary>
/// Mediator interface for sending requests.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a command without expecting a response.
    /// </summary>
    Task Send<TRequest>(TRequest request, CancellationToken token = default)
        where TRequest : IRequest;

    /// <summary>
    /// Sends a request and expects a response.
    /// </summary>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken token = default);
}
