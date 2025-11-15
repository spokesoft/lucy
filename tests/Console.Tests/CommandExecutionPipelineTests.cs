using Lucy.Application.Interfaces;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Lucy.Console.Internal;
using Lucy.Console.Middleware;
using Lucy.Console.Tests.Commands;
using Lucy.Infrastructure.Logging.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests;

/// <summary>
/// Tests for the command execution pipeline.
/// </summary>
public class CommandExecutionPipelineTests
{
    private readonly ServiceProvider _serviceProvider;
    private readonly TestCommandHandler _commandHandler;
    private readonly TestCommandValidator _commandValidator;
    private readonly TestDatabaseLoggingService _loggingService;
    private readonly TestDatabaseMigrator _databaseMigrator;
    private readonly Mock<ILogger<ErrorHandlerMiddleware>> _errorLogger;
    private readonly Mock<ILogger<ValidationMiddleware>> _validationLogger;
    private readonly Mock<ILogger<LoggingMiddleware>> _loggingLogger;
    private readonly Mock<ILogger<HandlerMiddleware>> _handlerLogger;
    private readonly Mock<ILogger<MigrationsMiddleware>> _migrationsLogger;
    private readonly Mock<IStringLocalizer<Program>> _localizer;

    public CommandExecutionPipelineTests()
    {
        _commandHandler = new TestCommandHandler();
        _commandValidator = new TestCommandValidator();
        _loggingService = new TestDatabaseLoggingService();
        _databaseMigrator = new TestDatabaseMigrator();
        _errorLogger = new Mock<ILogger<ErrorHandlerMiddleware>>();
        _validationLogger = new Mock<ILogger<ValidationMiddleware>>();
        _loggingLogger = new Mock<ILogger<LoggingMiddleware>>();
        _handlerLogger = new Mock<ILogger<HandlerMiddleware>>();
        _migrationsLogger = new Mock<ILogger<MigrationsMiddleware>>();
        _localizer = new Mock<IStringLocalizer<Program>>();

        // Setup localizer to return the key as the localized string
        _localizer.Setup(x => x[It.IsAny<string>()])
            .Returns((string key) => new LocalizedString(key, key));

        var services = new ServiceCollection();

        // Register test implementations
        services.AddSingleton<ICommandHandler<TestCommand>>(_commandHandler);
        services.AddSingleton<ICommandValidator<TestCommand>>(_commandValidator);
        services.AddSingleton<IDatabaseLoggingService>(_loggingService);
        services.AddSingleton<IDatabaseMigrator>(_databaseMigrator);

        // Register Spectre.Console TestConsole
        services.AddSingleton<Spectre.Console.IAnsiConsole>(new TestConsole());

        // Register loggers
        services.AddSingleton(_errorLogger.Object);
        services.AddSingleton(_validationLogger.Object);
        services.AddSingleton(_loggingLogger.Object);
        services.AddSingleton(_handlerLogger.Object);
        services.AddSingleton(_migrationsLogger.Object);
        services.AddSingleton(_localizer.Object);

        // Register middleware
        services.AddSingleton<ErrorHandlerMiddleware>();
        services.AddSingleton<ValidationMiddleware>();
        services.AddSingleton<LoggingMiddleware>();
        services.AddSingleton<HandlerMiddleware>();
        services.AddSingleton<MigrationsMiddleware>();

        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task Pipeline_ExecutesSuccessfully_WithAllMiddleware()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var pipeline = CreatePipeline();

        // Act
        var result = await pipeline.RunAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Success, result);
        Assert.True(_commandHandler.WasCalled);
        Assert.True(_commandValidator.WasCalled);
        Assert.True(_loggingService.IsStarted);
        Assert.True(_loggingService.IsStopped);
    }

    [Fact]
    public async Task Pipeline_HandlesValidationFailure_ReturnsInvalid()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var validationResult = new ValidationResult();
        validationResult.AddError(new ValidationError("Name is required"));
        _commandValidator.ResultToReturn = validationResult;
        var pipeline = CreatePipeline();

        // Act
        var result = await pipeline.RunAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Invalid, result);
        Assert.True(_commandValidator.WasCalled);
        Assert.False(_commandHandler.WasCalled); // Handler should not be called
    }

    [Fact]
    public async Task Pipeline_HandlesCancellation_ReturnsCanceled()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var cts = new CancellationTokenSource();
        _commandHandler.Delay = TimeSpan.FromSeconds(10); // Long delay to ensure cancellation
        _commandHandler.ExceptionToThrow = new OperationCanceledException();
        var pipeline = CreatePipeline();

        // Act
        cts.Cancel();
        var result = await pipeline.RunAsync(context, command, cts.Token);

        // Assert
        Assert.Equal((int)ExitCode.Canceled, result);
    }

    [Fact]
    public async Task Pipeline_HandlesUnexpectedException_ReturnsError()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var handlerException = new InvalidOperationException("Test exception");
        _commandHandler.ExceptionToThrow = handlerException;
        var pipeline = CreatePipeline();

        // Act
        var result = await pipeline.RunAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Error, result);
        Assert.True(_commandHandler.WasCalled);
    }

    [Fact]
    public async Task Pipeline_HandlesMigrationFailure_ReturnsError()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        _databaseMigrator.Reset(); // Reset state from previous tests
        _databaseMigrator.MigrationRequired = true;
        var migrationException = new InvalidOperationException("Migration failed");
        _databaseMigrator.ExceptionToThrow = migrationException;
        var pipeline = CreatePipeline();

        // Act
        var result = await pipeline.RunAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Error, result);
        Assert.True(_databaseMigrator.WasCalled);
        // MigrateWasCalled should be false since exception is thrown during IsMigrationRequiredAsync
        Assert.False(_databaseMigrator.MigrateWasCalled);
    }

    [Fact]
    public async Task Pipeline_SkipsMigration_WhenNotRequired()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        _databaseMigrator.MigrationRequired = false; // No migration needed
        var pipeline = CreatePipeline();

        // Act
        var result = await pipeline.RunAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Success, result);
        Assert.True(_databaseMigrator.WasCalled);
        Assert.False(_databaseMigrator.MigrateWasCalled); // Migration should not run
    }

    [Fact]
    public async Task Pipeline_StartsAndStopsLoggingService()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var pipeline = CreatePipeline();

        // Act
        await pipeline.RunAsync(context, command);

        // Assert
        Assert.True(_loggingService.IsStarted);
        Assert.True(_loggingService.IsStopped);
        Assert.NotNull(_loggingService.StopMessageFactory);
    }

    [Fact]
    public async Task Pipeline_HandlesLoggingServiceStopFailure()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        _loggingService.Reset(); // Reset state from previous tests
        var stopException = new InvalidOperationException("Stop failed");
        _loggingService.StopException = stopException;
        var pipeline = CreatePipeline();

        // Act
        var result = await pipeline.RunAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Error, result); // ErrorHandlerMiddleware converts exceptions to ExitCode.Error
        Assert.True(_loggingService.IsStarted);
    }

    [Fact]
    public async Task Pipeline_PassesCorrectParametersToHandler()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test", Value = "value" };
        var pipeline = CreatePipeline();

        // Act
        await pipeline.RunAsync(context, command);

        // Assert
        Assert.True(_commandHandler.WasCalled);
        Assert.Same(command, _commandHandler.LastCommand);
        Assert.Same(context, _commandHandler.LastContext);
        Assert.Equal("test", _commandHandler.LastCommand!.Name);
        Assert.Equal("value", _commandHandler.LastCommand!.Value);
    }

    [Fact]
    public async Task Pipeline_PassesCorrectParametersToValidator()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test", Value = "value" };
        var pipeline = CreatePipeline();

        // Act
        await pipeline.RunAsync(context, command);

        // Assert
        Assert.True(_commandValidator.WasCalled);
        Assert.Same(command, _commandValidator.LastCommand);
        Assert.Same(context, _commandValidator.LastContext);
    }

    [Fact]
    public async Task Pipeline_ReturnsCorrectExitCode_FromHandler()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        _commandHandler.ReturnValue = ExitCode.Error;
        var pipeline = CreatePipeline();

        // Act
        var result = await pipeline.RunAsync(context, command);

        // Assert
        Assert.Equal((int)ExitCode.Error, result);
    }

    [Fact]
    public async Task Pipeline_ExecutesMiddlewareInCorrectOrder()
    {
        // This test verifies that the middleware executes in the expected order:
        // ErrorHandler -> Migrations -> Logging -> Validation -> Handler

        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var executionOrder = new List<string>();

        // Create a pipeline with middleware that track execution order
        var pipeline = CommandPipelineBuilder<TestCommand>.Create(_serviceProvider)
            .Use(async (ctx, cmd, next, token) =>
            {
                executionOrder.Add("ErrorHandler-Start");
                var result = await next(ctx, cmd, token);
                executionOrder.Add("ErrorHandler-End");
                return result;
            })
            .Use(async (ctx, cmd, next, token) =>
            {
                executionOrder.Add("Migrations-Start");
                var result = await next(ctx, cmd, token);
                executionOrder.Add("Migrations-End");
                return result;
            })
            .Use(async (ctx, cmd, next, token) =>
            {
                executionOrder.Add("Logging-Start");
                var result = await next(ctx, cmd, token);
                executionOrder.Add("Logging-End");
                return result;
            })
            .Use(async (ctx, cmd, next, token) =>
            {
                executionOrder.Add("Validation-Start");
                var result = await next(ctx, cmd, token);
                executionOrder.Add("Validation-End");
                return result;
            })
            .Use((ctx, cmd, next, token) =>
            {
                executionOrder.Add("Handler");
                return Task.FromResult(ExitCode.Success);
            })
            .Build();

        // Act
        await pipeline.RunAsync(context, command);

        // Assert
        var expectedOrder = new[]
        {
            "ErrorHandler-Start",
            "Migrations-Start",
            "Logging-Start",
            "Validation-Start",
            "Handler",
            "Validation-End",
            "Logging-End",
            "Migrations-End",
            "ErrorHandler-End"
        };

        Assert.Equal(expectedOrder, executionOrder);
    }

    private static CommandContext CreateCommandContext()
    {
        var args = Array.Empty<string>();
        var remainingArgs = new Mock<IRemainingArguments>();
        remainingArgs.Setup(x => x.Raw).Returns(args);
        return new CommandContext(args, remainingArgs.Object, "test", null);
    }

    private CommandPipeline<TestCommand> CreatePipeline()
    {
        return CommandPipelineBuilder<TestCommand>.Create(_serviceProvider)
            .Use<ErrorHandlerMiddleware>()
            .Use<MigrationsMiddleware>()
            .Use<LoggingMiddleware>()
            .Use<ValidationMiddleware>()
            .Use<HandlerMiddleware>()
            .Build();
    }
}
