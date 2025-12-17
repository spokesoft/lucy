using Lucy.Application.Common.Interfaces;
using Lucy.Application.Iterations.Queries;
using Lucy.Application.Iterations.Queries.ListIterations;
using Lucy.Application.Iterations.Repositories;
using Lucy.Application.Common.Queries;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;

namespace Lucy.Application.Tests.Iterations.Queries.ListIterations;

public class ListIterationsQueryHandlerTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<IIterationReadOnlyRepository> _iterationRepositoryMock;
    private readonly ListIterationsQueryHandler _handler;

    public ListIterationsQueryHandlerTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _iterationRepositoryMock = new Mock<IIterationReadOnlyRepository>();

        _readOnlyUnitOfWorkMock.Setup(u => u.Iterations).Returns(_iterationRepositoryMock.Object);

        _handler = new ListIterationsQueryHandler(_readOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnIterations()
    {
        // Arrange
        var iterations = new List<Iteration>
        {
            new Iteration(1, "SPRINT-1", 1, "Sprint 1", "Goal", DateTime.Now, DateTime.Now.AddDays(14)) { Id = 10 },
            new Iteration(1, "SPRINT-2", 2, "Sprint 2", "Goal", DateTime.Now.AddDays(15), DateTime.Now.AddDays(29)) { Id = 11 }
        };

        _iterationRepositoryMock
            .Setup(r => r.GetByProjectIdAsync(1, IterationField.Id, SortDirection.Ascending, It.IsAny<CancellationToken>()))
            .ReturnsAsync(iterations);

        var query = new ListIterationsQuery(1, IterationField.Id, SortDirection.Ascending);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(10, result[0].Id);
        Assert.Equal(11, result[1].Id);
    }
}
