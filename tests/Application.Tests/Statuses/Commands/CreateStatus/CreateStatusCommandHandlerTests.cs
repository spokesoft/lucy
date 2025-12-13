using Lucy.Application.Projects.Repositories;
using Lucy.Application.Statuses.Commands.CreateStatus;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Statuses.Commands.CreateStatus;

public class CreateStatusCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<IStatusRepository> _statusRepositoryMock;
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly CreateStatusCommandHandler _handler;

    public CreateStatusCommandHandlerTests()
    {
        _statusRepositoryMock = SetupRepository(u => u.Statuses);
        _projectRepositoryMock = SetupRepository(u => u.Projects);
        _handler = new CreateStatusCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateStatus_WhenValidCommandIsGiven()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");

        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _statusRepositoryMock
            .Setup(repo => repo.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        _statusRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Status>(), It.IsAny<CancellationToken>()))
            .Callback<Status, CancellationToken>((status, _) => status.Id = 1)
            .Returns(Task.CompletedTask);

        var command = new CreateStatusCommand(1, "TODO", null, "To Do", "Tasks to be done", Color.Gray);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);

        _statusRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<Status>(s =>
                s.ProjectId == 1 &&
                s.Key == "TODO" &&
                s.Name == "To Do" &&
                s.Description == "Tasks to be done" &&
                s.Color == Color.Gray
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateStatusWithSpecifiedOrder_WhenOrderIsProvided()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");

        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _statusRepositoryMock
            .Setup(repo => repo.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        _statusRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Status>(), It.IsAny<CancellationToken>()))
            .Callback<Status, CancellationToken>((status, _) => status.Id = 1)
            .Returns(Task.CompletedTask);

        var command = new CreateStatusCommand(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);
        _statusRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Status>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project)null!);

        var command = new CreateStatusCommand(1, "TODO", null, "To Do", "Tasks to be done", Color.Gray);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenKeyIsEmpty()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");

        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _statusRepositoryMock
            .Setup(repo => repo.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var command = new CreateStatusCommand(1, "", null, "To Do", "Tasks to be done", Color.Gray);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.HandleAsync(command, CancellationToken.None));
    }
}
