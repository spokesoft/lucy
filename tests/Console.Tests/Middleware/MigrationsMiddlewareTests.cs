using Lucy.Application.Interfaces;
using Lucy.Console.Enums;
using Lucy.Console.Middleware;
using Lucy.Console.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using Spectre.Console.Cli;

namespace Lucy.Console.Tests.Middleware;

/// <summary>
/// Tests for the MigrationsMiddleware.
/// </summary>
public class MigrationsMiddlewareTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Mock<ILogger<MigrationsMiddleware>> _logger;
    private readonly IStringLocalizer<Program> _localizer;
    private readonly TestDatabaseMigrator _migrator;
    private readonly MigrationsMiddleware _middleware;

    public MigrationsMiddlewareTests()
    {
        var services = new ServiceCollection();
        _logger = new Mock<ILogger<MigrationsMiddleware>>();
        _localizer = new TestStringLocalizer<Program>();
        _migrator = new TestDatabaseMigrator();

        // Register our test migrator
        services.AddSingleton<IDatabaseMigrator>(_migrator);
        _serviceProvider = services.BuildServiceProvider();

        _middleware = new MigrationsMiddleware(_serviceProvider, _logger.Object, _localizer);
    }

    [Fact]
    public async Task InvokeAsync_WithNoMigrators_CallsNext()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var nextCalled = false;

        // Create a separate service provider with no migrators for this test
        var emptyServices = new ServiceCollection();
        var emptyServiceProvider = emptyServices.BuildServiceProvider();
        var emptyMiddleware = new MigrationsMiddleware(emptyServiceProvider, _logger.Object, _localizer);

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
        Assert.False(_migrator.WasCalled);
    }

    [Fact]
    public async Task InvokeAsync_WithMigrationNotRequired_SkipsMigration()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var nextCalled = false;
        _migrator.MigrationRequired = false;

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
        Assert.True(_migrator.WasCalled);
        Assert.False(_migrator.MigrateWasCalled);
    }

    [Fact]
    public async Task InvokeAsync_WithMigrationRequired_RunsMigration()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var nextCalled = false;
        _migrator.MigrationRequired = true;

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
        Assert.True(_migrator.WasCalled);
        Assert.True(_migrator.MigrateWasCalled);
    }

    [Fact]
    public async Task InvokeAsync_WithMigrationException_ThrowsException()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };

        // Create a specific migrator for this test
        var exceptionMigrator = new TestDatabaseMigrator
        {
            MigrationRequired = true,
            ExceptionToThrow = new InvalidOperationException("Migration failed")
        };

        // Create service provider with the exception migrator
        var testServices = new ServiceCollection();
        testServices.AddSingleton<IDatabaseMigrator>(exceptionMigrator);
        var testServiceProvider = testServices.BuildServiceProvider();
        var testMiddleware = new MigrationsMiddleware(testServiceProvider, _logger.Object, _localizer);

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            Assert.Fail("Next should not be called when migration fails");
            return Task.FromResult(ExitCode.Success);
        }

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => testMiddleware.InvokeAsync(context, command, Next));
        Assert.True(exceptionMigrator.WasCalled);
        Assert.False(exceptionMigrator.MigrateWasCalled); // Migration check failed, so migrate shouldn't be called
    }

    [Fact]
    public async Task InvokeAsync_WithMigrationCheckException_ThrowsException()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        _migrator.ExceptionToThrow = new InvalidOperationException("Migration check failed");

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            Assert.Fail("Next should not be called when migration check fails");
            return Task.FromResult(ExitCode.Success);
        }

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _middleware.InvokeAsync(context, command, Next));
        Assert.True(_migrator.WasCalled);
        Assert.False(_migrator.MigrateWasCalled);
    }

    [Fact]
    public async Task InvokeAsync_PassesCancellationToken()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var cts = new CancellationTokenSource();
        _migrator.MigrationRequired = true;

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            return Task.FromResult(ExitCode.Success);
        }

        // Act
        await _middleware.InvokeAsync(context, command, Next, cts.Token);

        // Assert
        // We can't directly verify the token was passed, but we can verify the migration was called
        Assert.True(_migrator.WasCalled);
        Assert.True(_migrator.MigrateWasCalled);
    }

    [Fact]
    public async Task InvokeAsync_WithMultipleMigrators_RunsAllRequiredMigrations()
    {
        // Arrange
        var context = CreateCommandContext();
        var command = new TestCommand { Name = "test" };
        var migrator1 = new TestDatabaseMigrator { MigrationRequired = true };
        var migrator2 = new TestDatabaseMigrator { MigrationRequired = false };
        var migrator3 = new TestDatabaseMigrator { MigrationRequired = true };
        var nextCalled = false;

        // Create service provider with multiple migrators for this test
        var multiServices = new ServiceCollection();
        multiServices.AddSingleton<IDatabaseMigrator>(migrator1);
        multiServices.AddSingleton<IDatabaseMigrator>(migrator2);
        multiServices.AddSingleton<IDatabaseMigrator>(migrator3);
        var multiServiceProvider = multiServices.BuildServiceProvider();
        var multiMiddleware = new MigrationsMiddleware(multiServiceProvider, _logger.Object, _localizer);

        Task<ExitCode> Next(CommandContext ctx, TestCommand cmd, CancellationToken token)
        {
            nextCalled = true;
            return Task.FromResult(ExitCode.Success);
        }

        // Act
        var result = await multiMiddleware.InvokeAsync(context, command, Next);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.True(nextCalled);

        // Verify all migrators were checked
        Assert.True(migrator1.WasCalled);
        Assert.True(migrator2.WasCalled);
        Assert.True(migrator3.WasCalled);

        // Verify only required migrations were run
        Assert.True(migrator1.MigrateWasCalled);
        Assert.False(migrator2.MigrateWasCalled);
        Assert.True(migrator3.MigrateWasCalled);
    }

    private static CommandContext CreateCommandContext()
    {
        var args = Array.Empty<string>();
        var remainingArgs = new Mock<IRemainingArguments>();
        remainingArgs.Setup(x => x.Raw).Returns(args);
        return new CommandContext(args, remainingArgs.Object, "test", null);
    }
}
