using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Queries.IterationExistsById;
using Lucy.Application.Iterations.Repositories;
using Moq;

namespace Lucy.Application.Tests.Iterations.Queries.IterationExistsById;

public class IterationExistsByIdQueryHandlerTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<IIterationReadOnlyRepository> _iterationRepositoryMock;
    private readonly IterationExistsByIdQueryHandler _handler;

    public IterationExistsByIdQueryHandlerTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _iterationRepositoryMock = new Mock<IIterationReadOnlyRepository>();

        _readOnlyUnitOfWorkMock.Setup(u => u.Iterations).Returns(_iterationRepositoryMock.Object);

        _handler = new IterationExistsByIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTrue_WhenExists()
    {
        // Arrange
        _iterationRepositoryMock
            .Setup(r => r.ExistsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var query = new IterationExistsByIdQuery(10);

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
            .Setup(r => r.ExistsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var query = new IterationExistsByIdQuery(10);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
