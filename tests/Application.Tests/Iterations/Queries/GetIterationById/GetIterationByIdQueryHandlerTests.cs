using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Queries.GetIterationById;
using Lucy.Application.Iterations.Repositories;
using Lucy.Domain.Entities;
using Moq;

namespace Lucy.Application.Tests.Iterations.Queries.GetIterationById;

public class GetIterationByIdQueryHandlerTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<IIterationReadOnlyRepository> _iterationRepositoryMock;
    private readonly GetIterationByIdQueryHandler _handler;

    public GetIterationByIdQueryHandlerTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _iterationRepositoryMock = new Mock<IIterationReadOnlyRepository>();

        _readOnlyUnitOfWorkMock.Setup(u => u.Iterations).Returns(_iterationRepositoryMock.Object);

        _handler = new GetIterationByIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnIteration_WhenFound()
    {
        // Arrange
        var iteration = new Iteration(1, "SPRINT-1", 1, "Sprint 1", "Goal", DateTime.Now, DateTime.Now.AddDays(14)) { Id = 10 };

        _iterationRepositoryMock
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(iteration);

        var query = new GetIterationByIdQuery(10);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Sprint 1", result.Name);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        _iterationRepositoryMock
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Iteration?)null);

        var query = new GetIterationByIdQuery(10);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
