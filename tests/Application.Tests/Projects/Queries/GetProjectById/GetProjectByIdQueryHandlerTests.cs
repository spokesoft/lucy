using Lucy.Application.Projects.Queries.GetProjectById;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly GetProjectByIdQueryHandler _handler;

    public GetProjectByIdQueryHandlerTests()
    {
        _projectRepositoryMock = SetupReadOnlyRepository(u => u.Projects);
        _handler = new GetProjectByIdQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnProject_WhenProjectExists()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");
        project.Id = 1;

        _projectRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var query = new GetProjectByIdQuery(1);

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
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var query = new GetProjectByIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
