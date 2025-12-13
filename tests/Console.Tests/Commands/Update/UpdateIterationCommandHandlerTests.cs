using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.DTOs;
using Lucy.Application.Iterations.Queries.GetIterationByKey;
using Lucy.Console.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;
using AppUpdateIterationCommand = Lucy.Application.Iterations.Commands.UpdateIteration.UpdateIterationCommand;
using ConsoleUpdateIterationCommand = Lucy.Console.Commands.Update.UpdateIterationCommand;
using ConsoleUpdateIterationCommandHandler = Lucy.Console.Commands.Update.UpdateIterationCommandHandler;

namespace Lucy.Console.Tests.Commands.Update;

public class UpdateIterationCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ConsoleUpdateIterationCommandHandler _handler;

    public UpdateIterationCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _mediatorMock = new Mock<IMediator>();

        _localizerMock
            .Setup(x => x[It.IsAny<string>(), It.IsAny<object[]>()])
            .Returns((string key, object[] args) =>
            {
                var formatted = args.Length > 0 ? $"{key} {string.Join(" ", args)}" : key;
                return new LocalizedString(key, formatted);
            });

        _handler = new ConsoleUpdateIterationCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithId_ReturnsSuccess()
    {
        // Arrange
        var command = new ConsoleUpdateIterationCommand
        {
            Id = 1,
            Name = "Updated Iteration",
            Description = "Updated Description"
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<AppUpdateIterationCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(null!, command);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _mediatorMock.Verify(x => x.Send(It.Is<AppUpdateIterationCommand>(c =>
            c.Id == command.Id &&
            c.Name == command.Name &&
            c.Description == command.Description), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithKey_ReturnsSuccess()
    {
        // Arrange
        var command = new ConsoleUpdateIterationCommand
        {
            Key = "ITER-1",
            Name = "Updated Iteration"
        };

        var iteration = new IterationDto
        {
            Id = 1,
            Name = "Iteration 1",
            Description = "Description",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(14),
            ProjectId = 1,
            Key = "ITER-1"
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetIterationByKeyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(iteration);

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<AppUpdateIterationCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(null!, command);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _mediatorMock.Verify(x => x.Send(It.Is<GetIterationByKeyQuery>(q => q.Key == "ITER-1"), It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(x => x.Send(It.Is<AppUpdateIterationCommand>(c => c.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
    }
}
