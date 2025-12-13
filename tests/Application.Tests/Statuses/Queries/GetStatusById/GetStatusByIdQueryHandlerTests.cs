using Lucy.Application.Statuses.Queries.GetStatusById;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Statuses.Queries.GetStatusById;

public class GetStatusByIdQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<IStatusReadOnlyRepository> _statusRepositoryMock;
    private readonly GetStatusByIdQueryHandler _handler;

    public GetStatusByIdQueryHandlerTests()
    {
        _statusRepositoryMock = SetupReadOnlyRepository(u => u.Statuses);
        _handler = new GetStatusByIdQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnStatus_WhenStatusExists()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to do", Color.Gray);
        status.Id = 1;

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var query = new GetStatusByIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("TODO", result.Key);
        Assert.Equal("To Do", result.Name);
        Assert.Equal("Tasks to do", result.Description);
        Assert.Equal(Color.Gray, result.Color);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenStatusDoesNotExist()
    {
        // Arrange
        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Status)null!);

        var query = new GetStatusByIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
