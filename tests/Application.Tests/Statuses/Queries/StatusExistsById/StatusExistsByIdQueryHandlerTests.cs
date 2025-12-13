using Lucy.Application.Statuses.Queries.StatusExistsById;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Statuses.Queries.StatusExistsById;

public class StatusExistsByIdQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<IStatusReadOnlyRepository> _statusRepositoryMock;
    private readonly StatusExistsByIdQueryHandler _handler;

    public StatusExistsByIdQueryHandlerTests()
    {
        _statusRepositoryMock = SetupReadOnlyRepository(u => u.Statuses);
        _handler = new StatusExistsByIdQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTrue_WhenStatusExists()
    {
        // Arrange
        _statusRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var query = new StatusExistsByIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFalse_WhenStatusDoesNotExist()
    {
        // Arrange
        _statusRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var query = new StatusExistsByIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
