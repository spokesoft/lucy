using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Infrastructure.Mediation;

/// <summary>
/// Base class for request handlers to implement dynamic handling.
/// </summary>
public abstract class RequestHandlerBase
{
    /// <summary>
    /// Handles the given request using the provided service provider.
    /// </summary>
    public abstract Task<object?> HandleAsync(
        object request,
        IServiceProvider provider,
        CancellationToken token = default);

    /// <summary>
    /// Validates the given request using a registered validator, if available.
    /// </summary>
    public static async Task<ValidationResult> ValidateAsync<TRequest>(
        TRequest request,
        IServiceProvider provider,
        CancellationToken token = default)
        where TRequest : IRequestBase
    {
        var validatorType = typeof(IRequestValidator<>).MakeGenericType(typeof(TRequest));

        if (provider.GetService(validatorType) is not IRequestValidator<TRequest> validator)
            return ValidationResult.Success;

        return await validator.ValidateAsync(request, token);
    }
}
