using Lucy.Application.Projects.Commands.CreateProject;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Projects.Commands.CreateProject;

public class CreateProjectCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTests()
    {
        _projectRepositoryMock = SetupRepository(u => u.Projects);
        _handler = new CreateProjectCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateProject_WhenValidCommandIsGiven()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .Callback<Project, CancellationToken>((project, _) => project.Id = 1)
            .Returns(Task.CompletedTask);

        var command = new CreateProjectCommand("TEST_KEY", "Test Name", "Test Description");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);

        _projectRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<Project>(p =>
                p.Key == "TEST_KEY" &&
                p.Name == "Test Name" &&
                p.Description == "Test Description"
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenKeyIsEmpty()
    {
        // Arrange
        var command = new CreateProjectCommand("", "Test Name", "Test Description");

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.HandleAsync(command, CancellationToken.None));
    }
}
