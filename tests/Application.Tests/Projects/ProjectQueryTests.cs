using Lucy.Application.Interfaces;
using Lucy.Application.Projects.DTOs;
using Lucy.Application.Projects.Queries;
using Lucy.Application.Projects.Queries.GetProjectById;
using Lucy.Application.Projects.Queries.GetProjectByKey;
using Lucy.Application.Projects.Queries.ListProjects;
using Lucy.Application.Queries;
using Lucy.Domain.Entities;
using Moq;

namespace Lucy.Application.Tests.Projects;

public class ProjectQueryTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;

    public ProjectQueryTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
    }

    [Fact]
    public async Task ListProjectsQueryHandler_ShouldReturnAllProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new Project("PROJ1", "Project 1", "Description 1"),
            new Project("PROJ2", "Project 2", "Description 2"),
            new Project("PROJ3", "Project 3", "Description 3")
        };

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.GetAllAsync(
                It.IsAny<ProjectSortField>(),
                It.IsAny<SortDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        var handler = new ListProjectsQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListProjectsQuery();

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("PROJ1", result[0].Key);
        Assert.Equal("PROJ2", result[1].Key);
        Assert.Equal("PROJ3", result[2].Key);
    }

    [Fact]
    public async Task ListProjectsQueryHandler_ShouldReturnEmptyList_WhenNoProjectsExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.GetAllAsync(
                It.IsAny<ProjectSortField>(),
                It.IsAny<SortDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project>());

        var handler = new ListProjectsQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListProjectsQuery();

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListProjectsQueryHandler_ShouldPassSortParameters()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.GetAllAsync(
                ProjectSortField.Key,
                SortDirection.Descending,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project>());

        var handler = new ListProjectsQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListProjectsQuery(ProjectSortField.Key, SortDirection.Descending);

        // Act
        await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _readOnlyUnitOfWorkMock.Verify(u => u.Projects.GetAllAsync(
            ProjectSortField.Key,
            SortDirection.Descending,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProjectByIdQueryHandler_ShouldReturnProject_WhenProjectExists()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");
        project.Id = 1;

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var handler = new GetProjectByIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetProjectByIdQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("TEST", result.Key);
        Assert.Equal("Test Project", result.Name);
        Assert.Equal("Test Description", result.Description);
    }

    [Fact]
    public async Task GetProjectByIdQueryHandler_ShouldReturnNull_WhenProjectDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project)null!);

        var handler = new GetProjectByIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetProjectByIdQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetProjectByKeyQueryHandler_ShouldReturnProject_WhenProjectExists()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");
        project.Id = 1;

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.GetByKeyAsync("TEST", It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var handler = new GetProjectByKeyQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetProjectByKeyQuery("TEST");

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("TEST", result.Key);
        Assert.Equal("Test Project", result.Name);
        Assert.Equal("Test Description", result.Description);
    }

    [Fact]
    public async Task GetProjectByKeyQueryHandler_ShouldReturnNull_WhenProjectDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.GetByKeyAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project)null!);

        var handler = new GetProjectByKeyQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetProjectByKeyQuery("NONEXISTENT");

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
