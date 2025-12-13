using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Queries.GetIterationIdByKey;
using Lucy.Application.Iterations.Repositories;
using Lucy.Domain.Entities;
using Moq;

namespace Lucy.Application.Tests.Iterations.Queries.GetIterationIdByKey;

public class GetIterationIdByKeyQueryHandlerTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<IIterationReadOnlyRepository> _iterationRepositoryMock;
    private readonly GetIterationIdByKeyQueryHandler _handler;

    public GetIterationIdByKeyQueryHandlerTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _iterationRepositoryMock = new Mock<IIterationReadOnlyRepository>();

        _readOnlyUnitOfWorkMock.Setup(u => u.Iterations).Returns(_iterationRepositoryMock.Object);

        _handler = new GetIterationIdByKeyQueryHandler(_readOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnId_WhenFound()
    {
        // Arrange
        var iteration = new Iteration(1, "SPRINT-1", 1, "Sprint 1", "Goal", DateTime.Now, DateTime.Now.AddDays(14)) { Id = 10 };

        _iterationRepositoryMock
            .Setup(r => r.GetByKeyAsync("SPRINT-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(iteration);

        var query = new GetIterationIdByKeyQuery("SPRINT-1");

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        _iterationRepositoryMock
            .Setup(r => r.GetByKeyAsync("SPRINT-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Iteration?)null);

        var query = new GetIterationIdByKeyQuery("SPRINT-1");

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
