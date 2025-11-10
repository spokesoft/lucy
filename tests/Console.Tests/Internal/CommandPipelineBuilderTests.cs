using Lucy.Console.Enums;
using Lucy.Console.Internal;
using Lucy.Console.Interfaces;
using Lucy.Console.Tests.Commands;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Spectre.Console.Cli;

namespace Lucy.Console.Tests.Internal;

/// <summary>
/// Tests for the CommandPipelineBuilder.
/// </summary>
public class CommandPipelineBuilderTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly Mock<ICommandMiddleware> _middleware1;
    private readonly Mock<ICommandMiddleware> _middleware2;

    public CommandPipelineBuilderTests()
    {
        _middleware1 = new Mock<ICommandMiddleware>();
        _middleware2 = new Mock<ICommandMiddleware>();

        var services = new ServiceCollection();
        services.AddSingleton(_middleware1.Object);
        services.AddSingleton(_middleware2.Object);
        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public void Create_ReturnsNewBuilder()
    {
        // Act
        var builder = CommandPipelineBuilder<TestCommand>.Create(_serviceProvider);

        // Assert
        Assert.NotNull(builder);
    }

    [Fact]
    public void Use_WithMiddlewareDelegate_AddsMiddlewareToPipeline()
    {
        // Arrange
        var builder = CommandPipelineBuilder<TestCommand>.Create(_serviceProvider);

        CommandMiddlewareDelegate<TestCommand> middleware = async (context, command, next, token) =>
        {
            return await next(context, command, token);
        };

        // Act
        builder.Use(middleware);
        var pipeline = builder.Build();

        // Assert
        Assert.NotNull(pipeline);
        // The middleware will be called when we run the pipeline
    }

    [Fact]
    public void Use_WithGenericMiddleware_ResolvesFromServiceProvider()
    {
        // Arrange
        var builder = CommandPipelineBuilder<TestCommand>.Create(_serviceProvider);

        // Setup middleware to be called
        _middleware1.Setup(x => x.InvokeAsync<TestCommand>(
            It.IsAny<CommandContext>(),
            It.IsAny<TestCommand>(),
            It.IsAny<CommandDelegate<TestCommand>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExitCode.Success);

        // Act & Assert (no exception should be thrown)
        var result = builder.Use(_middleware1.Object);
        Assert.Same(builder, result); // Should return the same builder for chaining
    }

    [Fact]
    public void Use_WithInstanceMiddleware_AddsMiddlewareToPipeline()
    {
        // Arrange
        var builder = CommandPipelineBuilder<TestCommand>.Create(_serviceProvider);
        var mockMiddleware = new Mock<ICommandMiddleware>();

        mockMiddleware.Setup(x => x.InvokeAsync<TestCommand>(
            It.IsAny<CommandContext>(),
            It.IsAny<TestCommand>(),
            It.IsAny<CommandDelegate<TestCommand>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(ExitCode.Success);

        // Act
        var result = builder.Use(mockMiddleware.Object);

        // Assert
        Assert.Same(builder, result); // Should return the same builder for chaining
        var pipeline = builder.Build();
        Assert.NotNull(pipeline);
    }

    [Fact]
    public async Task Build_CreatesWorkingPipeline()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var builder = CommandPipelineBuilder<TestCommand>.Create(_serviceProvider);

        var executionOrder = new List<string>();

        // Add multiple middleware in order
        builder.Use(async (ctx, cmd, next, token) =>
        {
            executionOrder.Add("Middleware1-Start");
            var result = await next(ctx, cmd, token);
            executionOrder.Add("Middleware1-End");
            return result;
        });

        builder.Use(async (ctx, cmd, next, token) =>
        {
            executionOrder.Add("Middleware2-Start");
            var result = await next(ctx, cmd, token);
            executionOrder.Add("Middleware2-End");
            return result;
        });

        builder.Use((ctx, cmd, next, token) =>
        {
            executionOrder.Add("Terminal");
            return Task.FromResult(ExitCode.Success);
        });

        // Act
        var pipeline = builder.Build();
        var result = await pipeline.RunAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Success, result);
        var expectedOrder = new[] { "Middleware1-Start", "Middleware2-Start", "Terminal", "Middleware2-End", "Middleware1-End" };
        Assert.Equal(expectedOrder, executionOrder);
    }

    [Fact]
    public async Task Build_WithNoMiddleware_ReturnsSuccessFromDefaultTerminalMiddleware()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var builder = CommandPipelineBuilder<TestCommand>.Create(_serviceProvider);

        // Act
        var pipeline = builder.Build();
        var result = await pipeline.RunAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Success, result);
    }

    [Fact]
    public async Task Build_ExecutesMiddlewareInCorrectOrder()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var builder = CommandPipelineBuilder<TestCommand>.Create(_serviceProvider);

        var callOrder = new List<int>();

        // Add middleware that track call order
        builder.Use(async (ctx, cmd, next, token) =>
        {
            callOrder.Add(1);
            var result = await next(ctx, cmd, token);
            callOrder.Add(4);
            return result;
        });

        builder.Use(async (ctx, cmd, next, token) =>
        {
            callOrder.Add(2);
            var result = await next(ctx, cmd, token);
            callOrder.Add(3);
            return result;
        });

        // Act
        var pipeline = builder.Build();
        await pipeline.RunAsync(context, command);

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 4 }, callOrder);
    }

    [Fact]
    public async Task Build_MiddlewareCanModifyResult()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var builder = CommandPipelineBuilder<TestCommand>.Create(_serviceProvider);

        builder.Use(async (ctx, cmd, next, token) =>
        {
            var result = await next(ctx, cmd, token);
            return ExitCode.Error; // Override the result
        });

        builder.Use((ctx, cmd, next, token) =>
        {
            return Task.FromResult(ExitCode.Success); // This should be overridden
        });

        // Act
        var pipeline = builder.Build();
        var result = await pipeline.RunAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Error, result);
    }

    [Fact]
    public async Task Build_MiddlewareCanShortCircuit()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var builder = CommandPipelineBuilder<TestCommand>.Create(_serviceProvider);

        var secondMiddlewareCalled = false;

        builder.Use((ctx, cmd, next, token) =>
        {
            // Don't call next - short circuit the pipeline
            return Task.FromResult(ExitCode.Canceled);
        });

        builder.Use(async (ctx, cmd, next, token) =>
        {
            secondMiddlewareCalled = true;
            return await next(ctx, cmd, token);
        });

        // Act
        var pipeline = builder.Build();
        var result = await pipeline.RunAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Canceled, result);
        Assert.False(secondMiddlewareCalled);
    }

    [Fact]
    public void Build_SupportsMethodChaining()
    {
        // Arrange
        var builder = CommandPipelineBuilder<TestCommand>.Create(_serviceProvider);

        // Act & Assert - should not throw
        var pipeline = builder
            .Use(async (ctx, cmd, next, token) => await next(ctx, cmd, token))
            .Use(async (ctx, cmd, next, token) => await next(ctx, cmd, token))
            .Use((ctx, cmd, next, token) => Task.FromResult(ExitCode.Success))
            .Build();

        Assert.NotNull(pipeline);
    }

    private static CommandContext CreateCommandContext()
    {
        var args = Array.Empty<string>();
        var remainingArgs = new Mock<IRemainingArguments>();
        remainingArgs.Setup(x => x.Raw).Returns(args);
        return new CommandContext(args, remainingArgs.Object, "test", null);
    }

    private void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}
