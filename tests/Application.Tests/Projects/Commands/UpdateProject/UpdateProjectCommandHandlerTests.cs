using Lucy.Application.Projects.Commands.UpdateProject;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Sequences.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ISequenceRepository> _sequenceRepositoryMock;
    private readonly UpdateProjectCommandHandler _handler;

    public UpdateProjectCommandHandlerTests()
    {
        _projectRepositoryMock = SetupRepository(u => u.Projects);
        _sequenceRepositoryMock = SetupRepository(u => u.Sequences);
        _handler = new UpdateProjectCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateProject_WhenValidCommandIsGiven()
    {
        // Arrange
        var project = new Project("OLD_KEY", "Old Name", "Old Description");

        _projectRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(project);

        var command = new UpdateProjectCommand(1, "NEW_KEY", "New Name", "New Description");

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal("NEW_KEY", project.Key);
        Assert.Equal("New Name", project.Name);
        Assert.Equal("New Description", project.Description);

        UnitOfWorkMock.Verify(
            u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, CancellationToken.None))
            .ReturnsAsync((Project)null!);

        var command = new UpdateProjectCommand(1, "NEW_KEY", "New Name", "New Description");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler
            .HandleAsync(command, CancellationToken.None));
    }
}
