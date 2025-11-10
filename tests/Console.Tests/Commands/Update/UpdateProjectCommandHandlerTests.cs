using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Console.Commands.Update;
using Lucy.Console.Enums;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.Update;

public class UpdateProjectCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UpdateProjectCommandHandler _handler;

    public UpdateProjectCommandHandlerTests()
    {
        _console = new TestConsole();
        _mediatorMock = new Mock<IMediator>();

        _handler = new UpdateProjectCommandHandler(
            _console,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_UpdateByKey_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new UpdateProjectCommand
        {
            Key = "TEST",
            Id = null,
            Name = "Updated Project",
            Description = "Updated Description"
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.Key),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains(command.Key, _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<Application.Projects.Commands.UpdateProject.UpdateProjectCommand>(c =>
                    c.Id == projectId &&
                    c.Key == command.Key &&
                    c.Name == command.Name &&
                    c.Description == command.Description),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UpdateById_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new UpdateProjectCommand
        {
            Key = null,
            Id = projectId,
            Name = "Updated Project",
            Description = "Updated Description"
        };

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains(projectId.ToString(), _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<Application.Projects.Commands.UpdateProject.UpdateProjectCommand>(c =>
                    c.Id == projectId &&
                    c.Key == command.Key &&
                    c.Name == command.Name &&
                    c.Description == command.Description),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_PartialUpdate_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new UpdateProjectCommand
        {
            Key = "TEST",
            Id = null,
            Name = "Updated Project",
            Description = null
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.Key),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains(command.Key, _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<Application.Projects.Commands.UpdateProject.UpdateProjectCommand>(c =>
                    c.Id == projectId &&
                    c.Key == command.Key &&
                    c.Name == command.Name &&
                    c.Description == command.Description),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
