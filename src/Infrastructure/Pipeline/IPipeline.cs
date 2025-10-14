namespace Lucy.Infrastructure.Pipeline;

/// <summary>
/// A pipeline that takes an input of type TInput and produces an output of
/// type TOutput.
/// </summary>
public interface IPipeline<TInput, TOutput>
{
    /// <summary>
    /// Runs the pipeline with the given input and cancellation token.
    /// </summary>
    Task<TOutput> RunAsync(TInput input, CancellationToken token = default);
}
