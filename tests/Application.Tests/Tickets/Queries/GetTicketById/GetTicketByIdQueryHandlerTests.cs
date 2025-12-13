using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.GetTicketById;
using Lucy.Application.Tickets.Repositories;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tickets.Queries.GetTicketById;

public class GetTicketByIdQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITicketReadOnlyRepository> _ticketRepositoryMock;
    private readonly GetTicketByIdQueryHandler _handler;

    public GetTicketByIdQueryHandlerTests()
    {
        _ticketRepositoryMock = SetupReadOnlyRepository(u => u.Tickets);
        _handler = new GetTicketByIdQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnTicket_WhenTicketExists()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "PROJ-1", 1, "Test Ticket", "Test Description");
        ticket.Id = 1;

        _ticketRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var query = new GetTicketByIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.ProjectId);
        Assert.Equal(2, result.StatusId);
        Assert.Equal("PROJ-1", result.Key);
        Assert.Equal("Test Ticket", result.Title);
        Assert.Equal("Test Description", result.Description);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenTicketDoesNotExist()
    {
        // Arrange
        _ticketRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticket)null!);

        var query = new GetTicketByIdQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
