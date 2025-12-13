using Lucy.Application.Statuses.Queries.StatusExistsByKey;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Statuses.Queries.StatusExistsByKey;

public class StatusExistsByKeyQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<IStatusReadOnlyRepository> _statusRepositoryMock;
    private readonly StatusExistsByKeyQueryHandler _handler;

    public StatusExistsByKeyQueryHandlerTests()
    {
        _statusRepositoryMock = SetupReadOnlyRepository(u => u.Statuses);
        _handler = new StatusExistsByKeyQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTrue_WhenStatusExists()
    {
        // Arrange
        _statusRepositoryMock
            .Setup(u => u.ExistsByKeyAsync(1, "TODO", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var query = new StatusExistsByKeyQuery(1, "TODO");

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
            .Setup(u => u.ExistsByKeyAsync(1, "NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var query = new StatusExistsByKeyQuery(1, "NONEXISTENT");

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
