using Lucy.Application.Interfaces;
using Lucy.Application.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Lucy.Infrastructure.Mediation;

/// <summary>
/// Adapter to bridge between the generic request handler interface and the
/// non-generic base class for requests without a response.
/// </summary>
public class RequestHandlerAdapter<TRequest> : RequestHandlerWrapper
    where TRequest : IRequest
{
    /// <inheritdoc />
    public override async Task<object?> HandleAsync(
        object request,
        IServiceProvider provider,
        CancellationToken token) =>
            await HandleAsync((TRequest)request, provider, token).ConfigureAwait(false);

    /// <inheritdoc />
    public override async Task<VoidResponse> HandleAsync(
        IRequest request,
        IServiceProvider provider,
        CancellationToken token)
    {
        var validation = await ValidateAsync(request, provider, token);

        if (!validation.IsValid)
            throw new ValidationException(validation);

        var handler = provider.GetService<IRequestHandler<TRequest>>()
            ?? throw new InvalidOperationException($"No handler registered for {typeof(TRequest)}");

        await handler.HandleAsync((TRequest)request, token);
        return VoidResponse.Value;
    }
}

/// <summary>
/// Adapter to bridge between the generic request handler interface and the
/// non-generic base class.
/// </summary>
public class RequestHandlerAdapter<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc />
    public override async Task<object?> HandleAsync(
        object request,
        IServiceProvider provider,
        CancellationToken token) =>
            await HandleAsync((TRequest)request, provider, token).ConfigureAwait(false);

    /// <inheritdoc />
    public override async Task<TResponse> HandleAsync(
        IRequest<TResponse> request,
        IServiceProvider provider,
        CancellationToken token)
    {
        var validation = await ValidateAsync(request, provider, token);

        if (!validation.IsValid)
            throw new ValidationException(validation);

        var handler = provider.GetService<IRequestHandler<TRequest, TResponse>>()
            ?? throw new InvalidOperationException($"No handler registered for {typeof(TRequest)}");

        return await handler.HandleAsync((TRequest)request, token);
    }
}
