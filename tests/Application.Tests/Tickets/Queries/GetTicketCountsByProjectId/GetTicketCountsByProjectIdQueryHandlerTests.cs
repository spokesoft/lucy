using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.GetTicketCountsByProjectId;
using Lucy.Application.Tickets.Repositories;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tickets.Queries.GetTicketCountsByProjectId;

public class GetTicketCountsByProjectIdQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITicketReadOnlyRepository> _ticketRepositoryMock;
    private readonly GetTicketCountsByProjectIdQueryHandler _handler;

    public GetTicketCountsByProjectIdQueryHandlerTests()
    {
        _ticketRepositoryMock = SetupReadOnlyRepository(u => u.Tickets);
        _handler = new GetTicketCountsByProjectIdQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCounts()
    {
        // Arrange
        var counts = new List<TicketCountByStatusDto>
        {
            new TicketCountByStatusDto { StatusId = 1, Count = 10, StatusKey = "TODO", StatusName = "To Do", StatusColor = "Gray" },
            new TicketCountByStatusDto { StatusId = 2, Count = 5, StatusKey = "DONE", StatusName = "Done", StatusColor = "Green" }
        };

        _ticketRepositoryMock
            .Setup(u => u.GetTicketCountsByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(counts);

        var query = new GetTicketCountsByProjectIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.StatusId == 1 && c.Count == 10);
        Assert.Contains(result, c => c.StatusId == 2 && c.Count == 5);
    }
}
