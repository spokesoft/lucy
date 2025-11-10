using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Console.Commands.Delete;
using Lucy.Console.Enums;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.Delete;

public class DeleteProjectCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DeleteProjectCommandHandler _handler;

    public DeleteProjectCommandHandlerTests()
    {
        _console = new TestConsole();
        _mediatorMock = new Mock<IMediator>();

        _handler = new DeleteProjectCommandHandler(
            _console,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_DeleteByKey_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var projectKey = "TEST";
        var command = new DeleteProjectCommand
        {
            Key = projectKey,
            Id = null
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == projectKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains(projectKey, _console.Output);
        Assert.Contains(projectId.ToString(), _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<Application.Projects.Commands.DeleteProject.DeleteProjectCommand>(c => c.Id == projectId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeleteById_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new DeleteProjectCommand
        {
            Key = null,
            Id = projectId
        };

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains(projectId.ToString(), _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<Application.Projects.Commands.DeleteProject.DeleteProjectCommand>(c => c.Id == projectId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
