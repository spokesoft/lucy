using Lucy.Application.Interfaces;
using Lucy.Application.Queries;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries;
using Lucy.Application.Tickets.Queries.GetTicketById;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Application.Tickets.Queries.ListTickets;
using Lucy.Domain.Entities;
using Moq;

namespace Lucy.Application.Tests.Tickets;

public class TicketQueryTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;

    public TicketQueryTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
    }

    [Fact]
    public async Task ListTicketsQueryHandler_ShouldReturnAllTickets()
    {
        // Arrange
        var tickets = new List<Ticket>
        {
            new Ticket(1, 2, "PROJ-1", "First Ticket", "Description 1"),
            new Ticket(1, 2, "PROJ-2", "Second Ticket", "Description 2"),
            new Ticket(1, 2, "PROJ-3", "Third Ticket", "Description 3")
        };

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByProjectIdAsync(
                1,
                It.IsAny<TicketSortField>(),
                It.IsAny<SortDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tickets);

        var handler = new ListTicketsQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListTicketsQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("PROJ-1", result[0].Key);
        Assert.Equal("PROJ-2", result[1].Key);
        Assert.Equal("PROJ-3", result[2].Key);
    }

    [Fact]
    public async Task ListTicketsQueryHandler_ShouldReturnEmptyList_WhenNoTicketsExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByProjectIdAsync(
                1,
                It.IsAny<TicketSortField>(),
                It.IsAny<SortDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Ticket>());

        var handler = new ListTicketsQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListTicketsQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListTicketsQueryHandler_ShouldPassSortParameters()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByProjectIdAsync(
                1,
                TicketSortField.Key,
                SortDirection.Descending,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Ticket>());

        var handler = new ListTicketsQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListTicketsQuery(1, TicketSortField.Key, SortDirection.Descending);

        // Act
        await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _readOnlyUnitOfWorkMock.Verify(u => u.Tickets.GetByProjectIdAsync(
            1,
            TicketSortField.Key,
            SortDirection.Descending,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTicketByIdQueryHandler_ShouldReturnTicket_WhenTicketExists()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "PROJ-1", "Test Ticket", "Test Description");
        ticket.Id = 1;

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var handler = new GetTicketByIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetTicketByIdQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

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
    public async Task GetTicketByIdQueryHandler_ShouldReturnNull_WhenTicketDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticket)null!);

        var handler = new GetTicketByIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetTicketByIdQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTicketByKeyQueryHandler_ShouldReturnTicket_WhenTicketExists()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "PROJ-1", "Test Ticket", "Test Description");
        ticket.Id = 1;

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByKeyAsync("PROJ-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var handler = new GetTicketByKeyQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetTicketByKeyQuery("PROJ-1");

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

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
    public async Task GetTicketByKeyQueryHandler_ShouldReturnNull_WhenTicketDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByKeyAsync("NONEXISTENT", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticket)null!);

        var handler = new GetTicketByKeyQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetTicketByKeyQuery("NONEXISTENT");

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
