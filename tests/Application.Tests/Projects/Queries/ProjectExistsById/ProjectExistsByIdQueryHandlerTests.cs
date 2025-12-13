using Lucy.Application.Projects.Queries.ProjectExistsById;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Projects.Queries.ProjectExistsById;

public class ProjectExistsByIdQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly ProjectExistsByIdQueryHandler _handler;

    public ProjectExistsByIdQueryHandlerTests()
    {
        _projectRepositoryMock = SetupReadOnlyRepository(u => u.Projects);
        _handler = new ProjectExistsByIdQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTrue_WhenProjectExists()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var query = new ProjectExistsByIdQuery(1);

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
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var query = new ProjectExistsByIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
