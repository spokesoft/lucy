using Lucy.Application.Projects.Commands.DeleteProject;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Projects.Commands.DeleteProject;

public class DeleteProjectCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly DeleteProjectCommandHandler _handler;

    public DeleteProjectCommandHandlerTests()
    {
        _projectRepositoryMock = SetupRepository(u => u.Projects);
        _handler = new DeleteProjectCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldDeleteProject_WhenProjectExists()
    {
        // Arrange
        var project = new Project("TEST_KEY", "Test Name", "Test Description");

        _projectRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(project);

        var command = new DeleteProjectCommand(1);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _projectRepositoryMock.Verify(repo => repo.Remove(project), Times.Once);
        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync((Project)null!);

        var command = new DeleteProjectCommand(1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command, CancellationToken.None));
    }
}
