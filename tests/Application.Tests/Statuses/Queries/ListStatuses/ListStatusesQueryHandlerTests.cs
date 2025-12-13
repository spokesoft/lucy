using Lucy.Application.Queries;
using Lucy.Application.Statuses.Queries;
using Lucy.Application.Statuses.Queries.ListStatuses;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Statuses.Queries.ListStatuses;

public class ListStatusesQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<IStatusReadOnlyRepository> _statusRepositoryMock;
    private readonly ListStatusesQueryHandler _handler;

    public ListStatusesQueryHandlerTests()
    {
        _statusRepositoryMock = SetupReadOnlyRepository(u => u.Statuses);
        _handler = new ListStatusesQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllStatuses()
    {
        // Arrange
        var statuses = new List<Status>
        {
            new Status(1, "TODO", 1, "To Do", "Tasks to do", Color.Gray),
            new Status(1, "INPROG", 2, "In Progress", "Tasks in progress", Color.Blue),
            new Status(1, "DONE", 3, "Done", "Completed tasks", Color.Green)
        };

        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(
                1,
                It.IsAny<StatusSortField>(),
                It.IsAny<SortDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);

        var query = new ListStatusesQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("TODO", result[0].Key);
        Assert.Equal("INPROG", result[1].Key);
        Assert.Equal("DONE", result[2].Key);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenNoStatusesExist()
    {
        // Arrange
        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(
                1,
                It.IsAny<StatusSortField>(),
                It.IsAny<SortDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var query = new ListStatusesQuery(1);

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
        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(
                1,
                StatusSortField.Key,
                SortDirection.Descending,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var query = new ListStatusesQuery(1, StatusSortField.Key, SortDirection.Descending);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _statusRepositoryMock.Verify(u => u.GetByProjectIdAsync(
            1,
            StatusSortField.Key,
            SortDirection.Descending,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
