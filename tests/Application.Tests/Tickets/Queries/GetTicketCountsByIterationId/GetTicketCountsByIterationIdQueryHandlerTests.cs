using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.GetTicketCountsByIterationId;
using Lucy.Application.Tickets.Repositories;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tickets.Queries.GetTicketCountsByIterationId;

public class GetTicketCountsByIterationIdQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITicketReadOnlyRepository> _ticketRepositoryMock;
    private readonly GetTicketCountsByIterationIdQueryHandler _handler;

    public GetTicketCountsByIterationIdQueryHandlerTests()
    {
        _ticketRepositoryMock = SetupReadOnlyRepository(u => u.Tickets);
        _handler = new GetTicketCountsByIterationIdQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCounts()
    {
        // Arrange
        var counts = new List<TicketCountByStatusDto>
        {
            new TicketCountByStatusDto { StatusId = 1, Count = 5, StatusKey = "TODO", StatusName = "To Do", StatusColor = "Gray" },
            new TicketCountByStatusDto { StatusId = 2, Count = 3, StatusKey = "DONE", StatusName = "Done", StatusColor = "Green" }
        };

        _ticketRepositoryMock
            .Setup(u => u.GetTicketCountsByIterationIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(counts);

        var query = new GetTicketCountsByIterationIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.StatusId == 1 && c.Count == 5);
        Assert.Contains(result, c => c.StatusId == 2 && c.Count == 3);
    }
}
