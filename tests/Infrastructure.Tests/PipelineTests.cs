using Lucy.Infrastructure.Pipeline;

namespace Lucy.Infrastructure.Tests;

/// <summary>
/// Tests for the pipeline infrastructure.
/// </summary>
public class PipelineTests
{
    class TestMiddleware : IMiddleware<string, string>
    {
        public Task<string> ExecuteAsync(string input, CancellationToken token = default)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input cannot be null or whitespace");
            return Task.FromResult(input.Trim().ToUpperInvariant());
        }
    }

    [Fact]
    public async Task Pipeline_SimpleStringTransformation_ShouldWork()
    {
        var pipeline = PipelineBuilder<string>.Create()
            .Pipe(input => string.IsNullOrWhiteSpace(input)
                ? throw new ArgumentException("Input cannot be null or whitespace")
                : input)
            .Pipe(input => input.Trim())
            .Pipe(input => input.ToUpperInvariant())
            .Pipe(input => $"Processed: {input}")
            .Build();

        var result = await pipeline.RunAsync("  hello world  ", CancellationToken.None);
        Assert.True(result == "Processed: HELLO WORLD");
    }

    [Fact]
    public async Task Pipeline_WithTypeTransformation_ShouldWork()
    {
        var pipeline = PipelineBuilder<string, string>.Create()
            .Pipe(input => string.IsNullOrWhiteSpace(input)
                ? throw new ArgumentException("Input cannot be null or whitespace")
                : input)
            .Pipe(input => input.Trim())
            .Pipe(input => input.Length)
            .Build();

        var result = await pipeline.RunAsync("  hello world  ", CancellationToken.None);
        Assert.True(result == 11);
    }

    [Fact]
    public async Task Pipeline_ConditionalPipeline_ShouldWork()
    {
        var pipeline = PipelineBuilder<int>.Create()
            .Pipe(x => x * 2)
            .PipeIf(
                predicate: x => x > 10,
                middleware: x => Task.FromResult(x + 100))
            .Pipe(x => x + 5)
            .Build();

        var result1 = await pipeline.RunAsync(3, CancellationToken.None);
        var result2 = await pipeline.RunAsync(10, CancellationToken.None);
        Assert.True(result1 == 11);
        Assert.True(result2 == 125);
    }

    [Fact]
    public async Task Pipeline_WithExceptionHander_ShouldWork()
    {
        var pipeline = PipelineBuilder<string>.Create()
            .Pipe(input => string.IsNullOrWhiteSpace(input)
                ? throw new ArgumentException("Input cannot be null or whitespace")
                : input)
            .Build();

        pipeline = pipeline.WithErrorHandler((ex, input) =>
        {
            if (ex is ArgumentException)
                return Task.FromResult("Default Value");
            throw ex;
        });

        var result = await pipeline.RunAsync("", CancellationToken.None);
        Assert.True(result == "Default Value");
    }

    [Fact]
    public async Task Pipeline_WithMiddleware_ShouldWork()
    {
        var pipeline = PipelineBuilder<string>.Create()
            .Pipe(new TestMiddleware())
            .Build();

        var result = await pipeline.RunAsync(" abc ", CancellationToken.None);
        Assert.True(result == "ABC");
    }
}

