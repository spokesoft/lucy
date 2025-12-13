using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Queries.GetIterationIdByKey;
using Lucy.Console.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;
using AppDeleteIterationCommand = Lucy.Application.Iterations.Commands.DeleteIteration.DeleteIterationCommand;
using ConsoleDeleteIterationCommand = Lucy.Console.Commands.Delete.DeleteIterationCommand;
using ConsoleDeleteIterationCommandHandler = Lucy.Console.Commands.Delete.DeleteIterationCommandHandler;

namespace Lucy.Console.Tests.Commands.Delete;

public class DeleteIterationCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ConsoleDeleteIterationCommandHandler _handler;

    public DeleteIterationCommandHandlerTests()
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

        _handler = new ConsoleDeleteIterationCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithId_ReturnsSuccess()
    {
        // Arrange
        var command = new ConsoleDeleteIterationCommand { Id = 1, Key = null };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<AppDeleteIterationCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(null!, command);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _mediatorMock.Verify(x => x.Send(It.Is<AppDeleteIterationCommand>(c => c.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithKey_ReturnsSuccess()
    {
        // Arrange
        var command = new ConsoleDeleteIterationCommand { Key = "ITER-1", Id = null };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetIterationIdByKeyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<AppDeleteIterationCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(null!, command);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _mediatorMock.Verify(x => x.Send(It.Is<GetIterationIdByKeyQuery>(q => q.Key == "ITER-1"), It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(x => x.Send(It.Is<AppDeleteIterationCommand>(c => c.Id == 1), It.IsAny<CancellationToken>()), Times.Once);
    }
}
