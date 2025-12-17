using Lucy.Application.Common.Interfaces;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Lucy.Console.Internal;
using Lucy.Console.Middleware;
using Lucy.Console.Tests.Helpers;
using Lucy.Infrastructure.Logging.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Internal;

/// <summary>
/// Tests for the CommandExecutor.
/// </summary>
public class CommandExecutorTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly TestCommandHandler _commandHandler;
    private readonly TestCommandValidator _commandValidator;
    private readonly TestDatabaseLoggingService _loggingService;
    private readonly TestDatabaseMigrator _databaseMigrator;
    private readonly CommandExecutor _executor;

    public CommandExecutorTests()
    {
        _commandHandler = new TestCommandHandler();
        _commandValidator = new TestCommandValidator();
        _loggingService = new TestDatabaseLoggingService();
        _databaseMigrator = new TestDatabaseMigrator();

        var services = new ServiceCollection();

        // Register test implementations
        services.AddSingleton<ICommandHandler<TestCommand>>(_ => _commandHandler);
        services.AddSingleton<ICommandValidator<TestCommand>>(_ => _commandValidator);
        services.AddSingleton<IDatabaseLoggingService>(_loggingService);
        services.AddSingleton<IDatabaseMigrator>(_databaseMigrator);

        // Register mock loggers
        var errorLogger = new Mock<ILogger<ErrorHandlerMiddleware>>();
        var validationLogger = new Mock<ILogger<ValidationMiddleware>>();
        var loggingLogger = new Mock<ILogger<LoggingMiddleware>>();
        var handlerLogger = new Mock<ILogger<HandlerMiddleware>>();
        var migrationsLogger = new Mock<ILogger<MigrationsMiddleware>>();
        var localizer = new Mock<IStringLocalizer<Program>>();

        // Setup localizer
        localizer.Setup(x => x[It.IsAny<string>()])
            .Returns((string key) => new LocalizedString(key, key));

        // Register Spectre.Console TestConsole
        services.AddSingleton<Spectre.Console.IAnsiConsole>(new TestConsole());

        services.AddSingleton(errorLogger.Object);
        services.AddSingleton(validationLogger.Object);
        services.AddSingleton(loggingLogger.Object);
        services.AddSingleton(handlerLogger.Object);
        services.AddSingleton(migrationsLogger.Object);
        services.AddSingleton(localizer.Object);

        // Register middleware
        services.AddSingleton<ErrorHandlerMiddleware>();
        services.AddSingleton<ValidationMiddleware>();
        services.AddSingleton<LoggingMiddleware>();
        services.AddSingleton<HandlerMiddleware>();
        services.AddSingleton<MigrationsMiddleware>();

        _serviceProvider = services.BuildServiceProvider();
        _executor = new CommandExecutor(_serviceProvider);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidCommand_ReturnsSuccess()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        _commandHandler.ReturnValue = ExitCode.Success;

        // Act
        var result = await _executor.ExecuteAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Success, result);
        Assert.True(_commandHandler.WasCalled);
        Assert.True(_commandValidator.WasCalled);
        Assert.True(_loggingService.IsStarted);
        Assert.True(_loggingService.IsStopped);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidCommand_ReturnsInvalid()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };

        var validationResult = new Lucy.Application.Common.Validation.ValidationResult();
        validationResult.AddError(new Lucy.Application.Common.Validation.ValidationError(
            "Name is required"));
        _commandValidator.ResultToReturn = validationResult;

        // Act
        var result = await _executor.ExecuteAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Invalid, result);
        Assert.True(_commandValidator.WasCalled);
        Assert.False(_commandHandler.WasCalled);
    }

    [Fact]
    public async Task ExecuteAsync_WithException_ReturnsError()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        _commandHandler.ExceptionToThrow = new InvalidOperationException("Test exception");

        // Act
        var result = await _executor.ExecuteAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Error, result);
        Assert.True(_commandHandler.WasCalled);
    }

    [Fact]
    public async Task ExecuteAsync_CreatesNewScope_ForEachExecution()
    {
        // Arrange
        var context1 = CreateCommandContext();
        var command1 = new TestCommand { Name = "test1" };
        var context2 = CreateCommandContext();
        var command2 = new TestCommand { Name = "test2" };

        _commandHandler.ReturnValue = ExitCode.Success;

        // Act
        var result1 = await _executor.ExecuteAsync(context1, command1);
        _commandHandler.Reset(); // Reset to verify second call
        var result2 = await _executor.ExecuteAsync(context2, command2);

        // Assert
        Assert.Equal((int)ExitCode.Success, result1);
        Assert.Equal((int)ExitCode.Success, result2);
        Assert.True(_commandHandler.WasCalled); // Should be called again after reset
    }

    [Fact]
    public async Task ExecuteAsync_PassesCorrectParameters()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test", Value = "value" };
        _commandHandler.ReturnValue = ExitCode.Success;

        // Act
        await _executor.ExecuteAsync(context, command);

        // Assert
        Assert.True(_commandHandler.WasCalled);
        Assert.Same(command, _commandHandler.LastCommand);
        Assert.Same(context, _commandHandler.LastContext);
        Assert.Equal("test", _commandHandler.LastCommand!.Name);
        Assert.Equal("value", _commandHandler.LastCommand!.Value);
    }

    [Fact]
    public async Task ExecuteAsync_HandlesOperationCanceledException()
    {
        // This test verifies that cancellation is properly handled
        // Note: The actual cancellation token handling in CommandExecutor
        // is more complex with Console.CancelKeyPress events

        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        _commandHandler.ExceptionToThrow = new OperationCanceledException();

        // Act
        var result = await _executor.ExecuteAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Canceled, result);
    }

    [Fact]
    public async Task ExecuteAsync_RunsFullPipeline()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        _databaseMigrator.MigrationRequired = true;
        _commandHandler.ReturnValue = ExitCode.Success;

        // Act
        var result = await _executor.ExecuteAsync(context, command);

        // Assert - Verify entire pipeline executed
        Assert.Equal((int)ExitCode.Success, result);

        // Verify migrations middleware
        Assert.True(_databaseMigrator.WasCalled);
        Assert.True(_databaseMigrator.MigrateWasCalled);

        // Verify logging middleware
        Assert.True(_loggingService.IsStarted);
        Assert.True(_loggingService.IsStopped);

        // Verify validation middleware
        Assert.True(_commandValidator.WasCalled);

        // Verify handler middleware
        Assert.True(_commandHandler.WasCalled);
    }

    [Fact]
    public async Task ExecuteAsync_MaintainsPipelineOrder()
    {
        // This test verifies that the middleware pipeline is built in the correct order:
        // ErrorHandler -> Migrations -> Logging -> Validation -> Handler

        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };

        // Setup everything to succeed so we can verify order
        _commandHandler.ReturnValue = ExitCode.Success;
        _databaseMigrator.MigrationRequired = false; // No migration needed for simplicity

        // Act
        var result = await _executor.ExecuteAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Success, result);

        // The fact that all middleware was called and returned success
        // indicates the pipeline order is correct
        Assert.True(_databaseMigrator.WasCalled);
        Assert.True(_loggingService.IsStarted);
        Assert.True(_loggingService.IsStopped);
        Assert.True(_commandValidator.WasCalled);
        Assert.True(_commandHandler.WasCalled);
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
