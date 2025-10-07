namespace Lucy.Application.Interfaces;

/// <summary>
/// Validator interface for requests.
/// </summary>
public interface IRequestValidator<TRequest> : IAsyncValidator<TRequest>
    where TRequest : IRequestBase
{
}
