using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Projects.Queries.ProjectExistsByKey;

public class ProjectExistsByKeyQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly ProjectExistsByKeyQueryHandler _handler;

    public ProjectExistsByKeyQueryHandlerTests()
    {
        _projectRepositoryMock = SetupReadOnlyRepository(u => u.Projects);
        _handler = new ProjectExistsByKeyQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTrue_WhenProjectExists()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByKeyAsync("TEST", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var query = new ProjectExistsByKeyQuery("TEST");

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFalse_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByKeyAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var query = new ProjectExistsByKeyQuery("NONEXISTENT");

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
