namespace Lucy.Infrastructure.Pipeline;

/// <summary>
/// A pipeline step.
/// </summary>
public interface IMiddleware<TInput, TOutput>
{
    /// <summary>
    /// Executes the middleware step.
    /// </summary>
    Task<TOutput> ExecuteAsync(TInput input, CancellationToken token = default);
}
