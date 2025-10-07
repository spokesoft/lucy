using Lucy.Application.Interfaces;

namespace Lucy.Infrastructure.Mediation;

/// <summary>
/// Base class for request handlers to implement dynamic handling.
/// </summary>
public abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
{
    /// <summary>
    /// Handles the given request using the provided service provider and returns a response.
    /// </summary>
    public abstract Task<TResponse> HandleAsync(
        IRequest<TResponse> request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

/// <summary>
/// Base class for request handlers to implement dynamic handling for requests without a response.
/// </summary>
public abstract class RequestHandlerWrapper : RequestHandlerBase
{
    /// <summary>
    /// Handles the given request using the provided service provider.
    /// </summary>
    public abstract Task<VoidResponse> HandleAsync(
        IRequest request,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}
