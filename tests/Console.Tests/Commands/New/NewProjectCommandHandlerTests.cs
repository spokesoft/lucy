using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Commands.CreateProject;
using Lucy.Console.Commands.New;
using Lucy.Console.Enums;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.New;

public class NewProjectCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly NewProjectCommandHandler _handler;

    public NewProjectCommandHandlerTests()
    {
        _console = new TestConsole();
        _mediatorMock = new Mock<IMediator>();

        _handler = new NewProjectCommandHandler(
            _console,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new NewProjectCommand
        {
            Key = "TEST",
            Name = "Test Project",
            Description = "Test Description"
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateProjectCommand>(c =>
                    c.Key == command.Key &&
                    c.Name == command.Name &&
                    c.Description == command.Description),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains(command.Key, _console.Output);
        Assert.Contains(projectId.ToString(), _console.Output);
    }

    [Fact]
    public async Task HandleAsync_MinimalCommand_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new NewProjectCommand
        {
            Key = "TEST",
            Name = "Test Project",
            Description = string.Empty
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateProjectCommand>(c =>
                    c.Key == command.Key &&
                    c.Name == command.Name &&
                    c.Description == command.Description),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains(command.Key, _console.Output);
        Assert.Contains(projectId.ToString(), _console.Output);
    }
}
