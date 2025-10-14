namespace Lucy.Infrastructure.Pipeline;

/// <summary>
/// A builder for creating pipelines.
/// </summary>
public class PipelineBuilder<TInput, TOutput>
{
    /// <summary>
    /// The list of middleware functions in the pipeline.
    /// </summary>
    private readonly List<Func<object, CancellationToken, Task<object>>> _middleware = [];

    /// <summary>
    /// Protected constructor.
    /// </summary>
    protected PipelineBuilder() { }

    /// <summary>
    /// Protected constructor.
    /// </summary>
    protected PipelineBuilder(IEnumerable<Func<object, CancellationToken, Task<object>>> middleware)
        => _middleware.AddRange(middleware);

    /// <summary>
    /// Creates a new pipeline builder.
    /// </summary>
    public static PipelineBuilder<TInput, TInput> Create()
        => new();

    /// <summary>
    /// Adds a stage to the pipeline using a function that returns a Task.
    /// </summary>
    public PipelineBuilder<TInput, TNext> Pipe<TNext>(Func<TOutput, Task<TNext>> middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);
        var builder = Copy<TInput, TNext>();
        builder.Add(async (context, token) =>
        {
            var input = (TOutput)context;
            var result = await middleware(input);
            return result!;
        });
        return builder;
    }

    /// <summary>
    /// Adds a stage to the pipeline using a function with cancellation token support.
    /// </summary>
    public PipelineBuilder<TInput, TNext> Pipe<TNext>(Func<TOutput, CancellationToken, Task<TNext>> middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);
        var builder = Copy<TInput, TNext>();
        builder.Add(async (context, token) =>
        {
            var input = (TOutput)context;
            var result = await middleware(input, token);
            return result!;
        });
        return builder;
    }

    /// <summary>
    /// Adds a stage to the pipeline using a synchronous function.
    /// </summary>
    public PipelineBuilder<TInput, TNext> Pipe<TNext>(Func<TOutput, TNext> middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);
        var builder = Copy<TInput, TNext>();
        builder.Add((context, token) =>
        {
            var input = (TOutput)context;
            var result = middleware(input);
            return Task.FromResult<object>(result!);
        });
        return builder;
    }

    /// <summary>
    /// Adds a stage to the pipeline using an IMiddleware implementation.
    /// </summary>
    public PipelineBuilder<TInput, TNext> Pipe<TNext>(IMiddleware<TOutput, TNext> middleware)
    {
        ArgumentNullException.ThrowIfNull(middleware);
        var builder = Copy<TInput, TNext>();
        builder.Add(async (context, token) =>
        {
            var input = (TOutput)context;
            var result = await middleware.ExecuteAsync(input, token);
            return result!;
        });
        return builder;
    }

    /// <summary>
    /// Adds a conditional stage that only executes if the predicate returns true.
    /// </summary>
    public PipelineBuilder<TInput, TOutput> PipeIf(
        Func<TOutput, bool> predicate,
        Func<TOutput, Task<TOutput>> middleware)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(middleware);
        var builder = Copy<TInput, TOutput>();
        builder.Add(async (context, token) =>
        {
            var input = (TOutput)context;
            if (predicate(input))
            {
                var result = await middleware(input);
                return result!;
            }
            return input;
        });
        return builder;
    }

    /// <summary>
    /// Adds a tap stage that performs an action without modifying the output.
    /// </summary>
    public PipelineBuilder<TInput, TOutput> Tap(Action<TOutput> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        var builder = Copy<TInput, TOutput>();
        builder.Add((context, token) =>
        {
            var input = (TOutput)context;
            action(input);
            return Task.FromResult<object>(input!);
        });
        return builder;
    }

    /// <summary>
    /// Adds an async tap stage that performs an action without modifying the output.
    /// </summary>
    public PipelineBuilder<TInput, TOutput> Tap(Func<TOutput, CancellationToken, Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        var builder = Copy<TInput, TOutput>();
        builder.Add(async (context, token) =>
        {
            var input = (TOutput)context;
            await action(input, token);
            return input;
        });
        return builder;
    }

    /// <summary>
    /// Builds the pipeline.
    /// </summary>
    public IPipeline<TInput, TOutput> Build()
    {
        return new Pipeline<TInput, TOutput>(async (input, token) =>
        {
            object current = input!;
            foreach (var step in _middleware)
            {
                token.ThrowIfCancellationRequested();
                current = await step(current, token);
            }
            return (TOutput)current;
        });
    }

    /// <summary>
    /// Creates a copy of the current builder with different input and output types.
    /// </summary>
    public PipelineBuilder<TCurrent, TNext> Copy<TCurrent, TNext>()
        => new(_middleware);

    /// <summary>
    /// Adds a middleware function to the pipeline.
    /// </summary>
    private void Add(Func<object, CancellationToken, Task<object>> middleware)
        => _middleware.Add(middleware);
}

/// <summary>
/// A builder for creating pipelines that take an input of type TInput and produce no output.
/// </summary>
/// <typeparam name="TInput"></typeparam>
public class PipelineBuilder<TInput> : PipelineBuilder<TInput, TInput>
{
    /// <summary>
    /// Creates a new pipeline builder.
    /// </summary>
    public static new PipelineBuilder<TInput, TInput> Create()
    {
        return PipelineBuilder<TInput, TInput>.Create();
    }
}
