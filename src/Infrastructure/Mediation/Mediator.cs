using Lucy.Application.Interfaces;

namespace Lucy.Infrastructure.Mediation;

/// <summary>
/// A simple Mediator implementation for sending requests to their respective handlers.
/// </summary>
public class Mediator(IServiceProvider provider) : IMediator
{
    /// <summary>
    /// The service provider used to resolve handlers and validators.
    /// </summary>
    private readonly IServiceProvider _provider = provider;

    /// <inheritdoc />
    public async Task Send<TRequest>(TRequest request, CancellationToken token = default)
        where TRequest : IRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        var handlerType = typeof(RequestHandlerAdapter<>).MakeGenericType(request.GetType());
        var handler = Activator.CreateInstance(handlerType)
            as RequestHandlerWrapper
            ?? throw new InvalidOperationException($"No handler registered for {request.GetType()}");

        await handler.HandleAsync(request, _provider, token);
    }

    /// <inheritdoc />
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var handlerType = typeof(RequestHandlerAdapter<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = Activator.CreateInstance(handlerType)
            as RequestHandlerWrapper<TResponse>
            ?? throw new InvalidOperationException($"No handler registered for {request.GetType()}");

        return await handler.HandleAsync(request, _provider, token);
    }
}
