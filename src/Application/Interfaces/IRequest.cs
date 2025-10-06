namespace Lucy.Application.Interfaces;

/// <summary>
/// An interface for requests without a response.
/// </summary>
public interface IRequest : IRequestBase
{
}

/// <summary>
/// An interface for requests with a response.
/// </summary>
public interface IRequest<out TResult> : IRequestBase
{
}
