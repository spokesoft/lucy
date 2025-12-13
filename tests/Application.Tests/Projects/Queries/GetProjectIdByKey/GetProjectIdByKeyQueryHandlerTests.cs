using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Projects.Queries.GetProjectIdByKey;

public class GetProjectIdByKeyQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly GetProjectIdByKeyQueryHandler _handler;

    public GetProjectIdByKeyQueryHandlerTests()
    {
        _projectRepositoryMock = SetupReadOnlyRepository(u => u.Projects);
        _handler = new GetProjectIdByKeyQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnId_WhenProjectExists()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");
        project.Id = 123;

        _projectRepositoryMock
            .Setup(u => u.GetByKeyAsync("TEST", It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var query = new GetProjectIdByKeyQuery("TEST");

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(123, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.GetByKeyAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var query = new GetProjectIdByKeyQuery("NONEXISTENT");

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
