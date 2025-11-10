using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Internal;
using Lucy.Console.Interfaces;
using Lucy.Console.Middleware;
using Lucy.Console.Tests.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Spectre.Console.Cli;
using System.Globalization;

namespace Lucy.Console.Tests.Middleware;

/// <summary>
/// Tests for the ValidationMiddleware.
/// </summary>
public class ValidationMiddlewareTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Mock<ILogger<ValidationMiddleware>> _logger;
    private readonly IStringLocalizer<Program> _localizer;
    private readonly TestCommandValidator _validator;
    private readonly ValidationMiddleware _middleware;

    public ValidationMiddlewareTests()
    {
        var services = new ServiceCollection();
        _logger = new Mock<ILogger<ValidationMiddleware>>();
        _localizer = new TestStringLocalizer<Program>();
        _validator = new TestCommandValidator();

        // Register our test validator
        services.AddSingleton<ICommandValidator<TestCommand>>(_validator);
        _serviceProvider = services.BuildServiceProvider();

        _middleware = new ValidationMiddleware(_serviceProvider, _logger.Object, _localizer);
    }

    [Fact]
    public async Task InvokeAsync_WithValidCommand_CallsNext()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var nextCalled = false;
        _validator.ResultToReturn = new ValidationResult(); // Valid result

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            nextCalled = true;
            return Task.FromResult(ExitCode.Success);
        }

        // Act
        var result = await _middleware.InvokeAsync(context, command, Next);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.True(nextCalled);
        Assert.True(_validator.WasCalled);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var validationResult = new ValidationResult();
        validationResult.AddError(new ValidationError("Name is required"));
        _validator.ResultToReturn = validationResult;

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            Assert.Fail("Next should not be called when validation fails");
            return Task.FromResult(ExitCode.Success);
        }

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _middleware.InvokeAsync(context, command, Next));
        Assert.True(_validator.WasCalled);
    }

    [Fact]
    public async Task InvokeAsync_WithNoValidators_CallsNext()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var nextCalled = false;

        // Create a service provider with no validators for this test
        var emptyServices = new ServiceCollection();
        var emptyServiceProvider = emptyServices.BuildServiceProvider();
        var emptyMiddleware = new ValidationMiddleware(emptyServiceProvider, _logger.Object, _localizer);

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            nextCalled = true;
            return Task.FromResult(ExitCode.Success);
        }

        // Act
        var result = await emptyMiddleware.InvokeAsync(context, command, Next);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.True(nextCalled);
        Assert.False(_validator.WasCalled); // Validator should not be called
    }

    [Fact]
    public async Task InvokeAsync_WithValidatorException_RethrowsException()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        _validator.ExceptionToThrow = new InvalidOperationException("Validator error");

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            Assert.Fail("Next should not be called when validation throws");
            return Task.FromResult(ExitCode.Success);
        }

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _middleware.InvokeAsync(context, command, Next));
        Assert.True(_validator.WasCalled);
    }

    [Fact]
    public async Task InvokeAsync_WithCancellation_ThrowsOperationCancelledException()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var cts = new CancellationTokenSource();
        _validator.ExceptionToThrow = new OperationCanceledException();

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            Assert.Fail("Next should not be called when validation is canceled");
            return Task.FromResult(ExitCode.Success);
        }

        // Act & Assert
        cts.Cancel();
        await Assert.ThrowsAsync<OperationCanceledException>(() => _middleware.InvokeAsync(context, command, Next, cts.Token));
    }

    private static CommandContext CreateCommandContext()
    {
        var args = Array.Empty<string>();
        var remainingArgs = new Mock<IRemainingArguments>();
        remainingArgs.Setup(x => x.Raw).Returns(args);
        return new CommandContext(args, remainingArgs.Object, "test", null);
    }
}
