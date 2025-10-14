namespace Lucy.Infrastructure.Pipeline;

/// <summary>
/// A pipeline that does nothing and takes no input or output.
/// </summary>
public class Pipeline<TInput, TOutput>(
    Func<TInput, CancellationToken, Task<TOutput>> app) : IPipeline<TInput, TOutput>
{
    /// <summary>
    /// The function that represents the pipeline.
    /// </summary>
    private readonly Func<TInput, CancellationToken, Task<TOutput>> _app = app
        ?? throw new ArgumentNullException(nameof(app));

    /// <inheritdoc/>
    public Task<TOutput> RunAsync(TInput input, CancellationToken token = default)
        => _app(input, token);
}
