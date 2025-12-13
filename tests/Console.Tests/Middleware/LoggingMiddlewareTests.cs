using Lucy.Console.Enums;
using Lucy.Console.Middleware;
using Lucy.Console.Tests.Helpers;
using Lucy.Infrastructure.Logging.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Spectre.Console.Cli;

namespace Lucy.Console.Tests.Middleware;

/// <summary>
/// Tests for the LoggingMiddleware.
/// </summary>
public class LoggingMiddlewareTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Mock<ILogger<LoggingMiddleware>> _logger;
    private readonly IStringLocalizer<Program> _localizer;
    private readonly TestDatabaseLoggingService _loggingService;
    private readonly LoggingMiddleware _middleware;

    public LoggingMiddlewareTests()
    {
        var services = new ServiceCollection();
        _logger = new Mock<ILogger<LoggingMiddleware>>();
        _localizer = new TestStringLocalizer<Program>(); // Use real implementation instead of mock
        _loggingService = new TestDatabaseLoggingService();

        // Register our test logging service
        services.AddSingleton<IDatabaseLoggingService>(_loggingService);
        _serviceProvider = services.BuildServiceProvider();

        _middleware = new LoggingMiddleware(_serviceProvider, _logger.Object, _localizer);
    }

    [Fact]
    public async Task InvokeAsync_StartsAndStopsLoggingService()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var nextCalled = false;

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            nextCalled = true;
            Assert.True(_loggingService.IsStarted);
            Assert.False(_loggingService.IsStopped);
            return Task.FromResult(ExitCode.Success);
        }

        // Act
        var result = await _middleware.InvokeAsync(context, command, Next);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.True(nextCalled);
        Assert.True(_loggingService.IsStarted);
        Assert.True(_loggingService.IsStopped);
        Assert.NotNull(_loggingService.StopMessageFactory);
    }

    [Fact]
    public async Task InvokeAsync_PassesCancellationTokenToLoggingService()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var cts = new CancellationTokenSource();

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            return Task.FromResult(ExitCode.Success);
        }

        // Act
        await _middleware.InvokeAsync(context, command, Next, cts.Token);

        // Assert
        Assert.Equal(cts.Token, _loggingService.StartToken);
    }

    [Fact]
    public async Task InvokeAsync_StopsLoggingServiceEvenWhenNextThrows()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var testException = new InvalidOperationException("Test exception");

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            throw testException;
        }

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _middleware.InvokeAsync(context, command, Next));
        Assert.Same(testException, exception);
        Assert.True(_loggingService.IsStarted);
        Assert.True(_loggingService.IsStopped);
    }

    [Fact]
    public async Task InvokeAsync_HandlesLoggingServiceStopException()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var stopException = new InvalidOperationException("Stop failed");
        _loggingService.StopException = stopException;

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            return Task.FromResult(ExitCode.Success);
        }

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _middleware.InvokeAsync(context, command, Next));
        Assert.Same(stopException, exception);
        Assert.True(_loggingService.IsStarted);
    }

    [Fact]
    public async Task InvokeAsync_GeneratesFinalMessage()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            // Simulate some processing time
            Thread.Sleep(10);
            return Task.FromResult(ExitCode.Success);
        }

        // Act
        await _middleware.InvokeAsync(context, command, Next);

        // Assert
        Assert.NotNull(_loggingService.StopMessageFactory);
        var message = _loggingService.StopMessageFactory!(0, 100);
        Assert.Contains("Messages.LoggingServiceStopped", message);
    }

    private static CommandContext CreateCommandContext()
    {
        var args = Array.Empty<string>();
        var remainingArgs = new Mock<IRemainingArguments>();
        remainingArgs.Setup(x => x.Raw).Returns(args);
        return new CommandContext(args, remainingArgs.Object, "test", null);
    }
}
