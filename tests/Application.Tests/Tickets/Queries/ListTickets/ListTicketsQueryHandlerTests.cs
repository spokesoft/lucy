using Lucy.Application.Common.Queries;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries;
using Lucy.Application.Tickets.Queries.ListTickets;
using Lucy.Application.Tickets.Repositories;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tickets.Queries.ListTickets;

public class ListTicketsQueryHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITicketReadOnlyRepository> _ticketRepositoryMock;
    private readonly ListTicketsQueryHandler _handler;

    public ListTicketsQueryHandlerTests()
    {
        _ticketRepositoryMock = SetupReadOnlyRepository(u => u.Tickets);
        _handler = new ListTicketsQueryHandler(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnAllTickets()
    {
        // Arrange
        var tickets = new List<Ticket>
        {
              new Ticket(1, 2, "PROJ-1", 1, "First Ticket", "Description 1"),
              new Ticket(1, 2, "PROJ-2", 2, "Second Ticket", "Description 2"),
              new Ticket(1, 2, "PROJ-3", 3, "Third Ticket", "Description 3")
        };

        _ticketRepositoryMock
            .Setup(u => u.SearchAsync(
                1,
                null,
                null,
                null,
                It.IsAny<TicketField>(),
                It.IsAny<SortDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tickets);

        var query = new ListTicketsQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("PROJ-1", result[0].Key);
        Assert.Equal("PROJ-2", result[1].Key);
        Assert.Equal("PROJ-3", result[2].Key);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenNoTicketsExist()
    {
        // Arrange
        _ticketRepositoryMock
            .Setup(u => u.SearchAsync(
                1,
                null,
                null,
                null,
                It.IsAny<TicketField>(),
                It.IsAny<SortDirection>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Ticket>());

        var query = new ListTicketsQuery(1);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task HandleAsync_ShouldPassSortParameters()
    {
        // Arrange
        _ticketRepositoryMock
            .Setup(u => u.SearchAsync(
                1,
                null,
                null,
                null,
                TicketField.Key,
                SortDirection.Descending,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Ticket>());

        var query = new ListTicketsQuery(1, null, TicketField.Key, SortDirection.Descending);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _ticketRepositoryMock.Verify(u => u.SearchAsync(
            1,
            null,
            null,
            null,
            TicketField.Key,
            SortDirection.Descending,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByTagId()
    {
        // Arrange
        _ticketRepositoryMock
            .Setup(u => u.SearchAsync(
                1,
                null,
                5,
                null,
                TicketField.Id,
                SortDirection.Ascending,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Ticket>());

        var query = new ListTicketsQuery(1, TagId: 5);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _ticketRepositoryMock.Verify(u => u.SearchAsync(
            1,
            null,
            5,
            null,
            TicketField.Id,
            SortDirection.Ascending,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByStatusAndTagId()
    {
        // Arrange
        _ticketRepositoryMock
            .Setup(u => u.SearchAsync(
                1,
                2,
                5,
                null,
                TicketField.Id,
                SortDirection.Ascending,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Ticket>());

        var query = new ListTicketsQuery(1, 2, TagId: 5);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _ticketRepositoryMock.Verify(u => u.SearchAsync(
            1,
            2,
            5,
            null,
            TicketField.Id,
            SortDirection.Ascending,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldFilterByIterationId()
    {
        // Arrange
        _ticketRepositoryMock
            .Setup(u => u.SearchAsync(
                1,
                null,
                null,
                10,
                TicketField.Id,
                SortDirection.Ascending,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Ticket>());

        var query = new ListTicketsQuery(1, IterationId: 10);

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _ticketRepositoryMock.Verify(u => u.SearchAsync(
            1,
            null,
            null,
            10,
            TicketField.Id,
            SortDirection.Ascending,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
