using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Commands.CreateIteration;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Console.Commands.New;
using Lucy.Console.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.New;

public class NewIterationCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly NewIterationCommandHandler _handler;

    public NewIterationCommandHandlerTests()
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

        _handler = new NewIterationCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithProjectId_ReturnsSuccess()
    {
        // Arrange
        var command = new NewIterationCommand
        {
            ProjectId = 1,
            Name = "Iteration 1",
            Description = "Description",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(14)
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<CreateIterationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.HandleAsync(null!, command);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _mediatorMock.Verify(x => x.Send(It.Is<CreateIterationCommand>(c =>
            c.ProjectId == command.ProjectId &&
            c.Name == command.Name &&
            c.Description == command.Description), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithProjectKey_ReturnsSuccess()
    {
        // Arrange
        var command = new NewIterationCommand
        {
            ProjectKey = "PROJ",
            Name = "Iteration 1"
        };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetProjectIdByKeyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<CreateIterationCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.HandleAsync(null!, command);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _mediatorMock.Verify(x => x.Send(It.Is<GetProjectIdByKeyQuery>(q => q.Key == "PROJ"), It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(x => x.Send(It.Is<CreateIterationCommand>(c => c.ProjectId == 1), It.IsAny<CancellationToken>()), Times.Once);
    }
}
