namespace Lucy.Infrastructure.Pipeline;

public static class PipelineExtensions
{
    /// <summary>
    /// Chains two pipelines together.
    /// </summary>
    public static IPipeline<TInput, TFinal> Then<TInput, TMiddle, TFinal>(
        this IPipeline<TInput, TMiddle> first,
        IPipeline<TMiddle, TFinal> second)
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);

        return new Pipeline<TInput, TFinal>(async (input, token) =>
            {
                var middle = await first.RunAsync(input, token);
                return await second.RunAsync(middle, token);
            });
    }

    /// <summary>
    /// Wraps a pipeline with error handling.
    /// </summary>
    public static IPipeline<TInput, TOutput> WithErrorHandler<TInput, TOutput>(
        this IPipeline<TInput, TOutput> pipeline,
        Func<Exception, TInput, Task<TOutput>> errorHandler)
    {
        ArgumentNullException.ThrowIfNull(pipeline);
        ArgumentNullException.ThrowIfNull(errorHandler);

        return new Pipeline<TInput, TOutput>(async (input, token) =>
        {
            try
            {
                return await pipeline.RunAsync(input, token);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return await errorHandler(ex, input);
            }
        });
    }

    /// <summary>
    /// Adds retry logic to a pipeline.
    /// </summary>
    public static IPipeline<TIn, TOut> WithRetry<TIn, TOut>(
        this IPipeline<TIn, TOut> pipeline,
        int maxAttempts = 3,
        TimeSpan? delay = null)
    {
        ArgumentNullException.ThrowIfNull(pipeline);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxAttempts);

        return new Pipeline<TIn, TOut>(async (input, token) =>
        {
            Exception? lastException = null;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    return await pipeline.RunAsync(input, token);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    lastException = ex;
                    if (delay is not null && attempt < maxAttempts)
                        await Task.Delay(delay.Value * attempt, token);
                }
            }
            throw lastException!;
        });
    }

    /// <summary>
    /// Adds timeout to a pipeline execution.
    /// </summary>
    public static IPipeline<TIn, TOut> WithTimeout<TIn, TOut>(
        this IPipeline<TIn, TOut> pipeline,
        TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(pipeline);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(timeout, TimeSpan.Zero);

        return new Pipeline<TIn, TOut>(async (input, token) =>
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.CancelAfter(timeout);

            return await pipeline.RunAsync(input, cts.Token);
        });
    }
}
