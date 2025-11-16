using Lucy.Application.Interfaces;
using Lucy.Application.Queries;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Statuses.Queries;
using Lucy.Application.Statuses.Queries.GetStatusById;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Application.Statuses.Queries.ListStatuses;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;

namespace Lucy.Application.Tests.Statuses;

public class StatusQueryTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;

    public StatusQueryTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
    }

    [Fact]
    public async Task ListStatusesQueryHandler_ShouldReturnAllStatuses()
    {
        // Arrange
        var statuses = new List<Status>
        {
            new Status(1, "TODO", 1, "To Do", "Tasks to do", StatusColor.Gray),
            new Status(1, "INPROG", 2, "In Progress", "Tasks in progress", StatusColor.Blue),
            new Status(1, "DONE", 3, "Done", "Completed tasks", StatusColor.Green)
        };

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(
                1,
                It.IsAny<StatusSortField>(),
                It.IsAny<SortDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);

        var handler = new ListStatusesQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListStatusesQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("TODO", result[0].Key);
        Assert.Equal("INPROG", result[1].Key);
        Assert.Equal("DONE", result[2].Key);
    }

    [Fact]
    public async Task ListStatusesQueryHandler_ShouldReturnEmptyList_WhenNoStatusesExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(
                1,
                It.IsAny<StatusSortField>(),
                It.IsAny<SortDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var handler = new ListStatusesQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListStatusesQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListStatusesQueryHandler_ShouldPassSortParameters()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(
                1,
                StatusSortField.Key,
                SortDirection.Descending,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var handler = new ListStatusesQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListStatusesQuery(1, StatusSortField.Key, SortDirection.Descending);

        // Act
        await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _readOnlyUnitOfWorkMock.Verify(u => u.Statuses.GetByProjectIdAsync(
            1,
            StatusSortField.Key,
            SortDirection.Descending,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetStatusByIdQueryHandler_ShouldReturnStatus_WhenStatusExists()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to do", StatusColor.Gray);
        status.Id = 1;

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var handler = new GetStatusByIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetStatusByIdQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("TODO", result.Key);
        Assert.Equal("To Do", result.Name);
        Assert.Equal("Tasks to do", result.Description);
        Assert.Equal(StatusColor.Gray, result.Color);
    }

    [Fact]
    public async Task GetStatusByIdQueryHandler_ShouldReturnNull_WhenStatusDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Status)null!);

        var handler = new GetStatusByIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetStatusByIdQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetStatusByKeyQueryHandler_ShouldReturnStatus_WhenStatusExists()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to do", StatusColor.Gray);
        status.Id = 1;

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByKeyAsync(1, "TODO", It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var handler = new GetStatusByKeyQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetStatusByKeyQuery(1, "TODO");

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("TODO", result.Key);
        Assert.Equal("To Do", result.Name);
        Assert.Equal("Tasks to do", result.Description);
        Assert.Equal(StatusColor.Gray, result.Color);
    }

    [Fact]
    public async Task GetStatusByKeyQueryHandler_ShouldReturnNull_WhenStatusDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByKeyAsync(1, "NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Status)null!);

        var handler = new GetStatusByKeyQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetStatusByKeyQuery(1, "NONEXISTENT");

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
