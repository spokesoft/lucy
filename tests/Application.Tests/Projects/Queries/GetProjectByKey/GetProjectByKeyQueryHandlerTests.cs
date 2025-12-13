using Lucy.Application.Projects.Queries.GetProjectByKey;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Projects.Queries.GetProjectByKey;

public class GetProjectByKeyQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly GetProjectByKeyQueryHandler _handler;

    public GetProjectByKeyQueryHandlerTests()
    {
        _projectRepositoryMock = SetupReadOnlyRepository(u => u.Projects);
        _handler = new GetProjectByKeyQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnProject_WhenProjectExists()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");
        project.Id = 1;

        _projectRepositoryMock
            .Setup(u => u.GetByKeyAsync("TEST", It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var query = new GetProjectByKeyQuery("TEST");

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("TEST", result.Key);
        Assert.Equal("Test Project", result.Name);
        Assert.Equal("Test Description", result.Description);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.GetByKeyAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var query = new GetProjectByKeyQuery("NONEXISTENT");

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
