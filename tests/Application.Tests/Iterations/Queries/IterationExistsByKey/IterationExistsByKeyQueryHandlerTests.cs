using Lucy.Application.Common.Interfaces;
using Lucy.Application.Iterations.Queries.IterationExistsByKey;
using Lucy.Application.Iterations.Repositories;
using Moq;

namespace Lucy.Application.Tests.Iterations.Queries.IterationExistsByKey;

public class IterationExistsByKeyQueryHandlerTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<IIterationReadOnlyRepository> _iterationRepositoryMock;
    private readonly IterationExistsByKeyQueryHandler _handler;

    public IterationExistsByKeyQueryHandlerTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _iterationRepositoryMock = new Mock<IIterationReadOnlyRepository>();

        _readOnlyUnitOfWorkMock.Setup(u => u.Iterations).Returns(_iterationRepositoryMock.Object);

        _handler = new IterationExistsByKeyQueryHandler(_readOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTrue_WhenExists()
    {
        // Arrange
        _iterationRepositoryMock
            .Setup(r => r.ExistsByKeyAsync("SPRINT-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var query = new IterationExistsByKeyQuery("SPRINT-1");

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFalse_WhenNotExists()
    {
        // Arrange
        _iterationRepositoryMock
            .Setup(r => r.ExistsByKeyAsync("SPRINT-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var query = new IterationExistsByKeyQuery("SPRINT-1");

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
