using Lucy.Application.Projects.Queries;
using Lucy.Application.Projects.Queries.ListProjects;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Common.Queries;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Projects.Queries.ListProjects;

public class ListProjectsQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly ListProjectsQueryHandler _handler;

    public ListProjectsQueryHandlerTests()
    {
        _projectRepositoryMock = SetupReadOnlyRepository(u => u.Projects);
        _handler = new ListProjectsQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new Project("PROJ1", "Project 1", "Description 1"),
            new Project("PROJ2", "Project 2", "Description 2"),
            new Project("PROJ3", "Project 3", "Description 3")
        };

        _projectRepositoryMock
            .Setup(u => u.GetAllAsync(
                It.IsAny<ProjectField>(),
                It.IsAny<SortDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        var query = new ListProjectsQuery();

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("PROJ1", result[0].Key);
        Assert.Equal("PROJ2", result[1].Key);
        Assert.Equal("PROJ3", result[2].Key);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenNoProjectsExist()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.GetAllAsync(
                It.IsAny<ProjectField>(),
                It.IsAny<SortDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project>());

        var query = new ListProjectsQuery();

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassSortParameters()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.GetAllAsync(
                ProjectField.Key,
                SortDirection.Descending,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project>());

        var query = new ListProjectsQuery(ProjectField.Key, SortDirection.Descending);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _projectRepositoryMock.Verify(u => u.GetAllAsync(
            ProjectField.Key,
            SortDirection.Descending,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
