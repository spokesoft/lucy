using Lucy.Application.Tickets.Queries;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Lucy.Infrastructure.Database;
using Lucy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Tests.Repositories;

/// <summary>
/// Tests for the TicketRepository.
/// </summary>
[Collection("Database collection")]
public class TicketRepositoryTests : RepositoryTestBase
{
    private async Task<(Project project, Status[] statuses, Ticket[] tickets)> SeedDatabaseAsync(LucyDbContext context)
    {
        var project = new Project("TEST", "Test Project", "Test Description");
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        // Project constructor automatically creates 3 default statuses
        var statuses = project.Statuses.OrderBy(s => s.Order).ToArray();
        var status1 = statuses[0];
        var status2 = statuses[2];

        var ticket1 = new Ticket(project.Id, status1.Id, "TEST-1", 1, "First Ticket", "Description 1");
        var ticket2 = new Ticket(project.Id, status1.Id, "TEST-2", 2, "Second Ticket", "Description 2");
        var ticket3 = new Ticket(project.Id, status2.Id, "TEST-3", 3, "Third Ticket", "Description 3");

        context.Tickets.AddRange(ticket1, ticket2, ticket3);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        return (project, statuses, new[] { ticket1, ticket2, ticket3 });
    }

    [Fact]
    public async Task AddAsync_ShouldAddTicketToDatabase()
    {
        // Arrange
        await using var context = new LucyWriteContext(DbContextOptions);
        await SeedDatabaseAsync(context);

        var repository = new TicketRepository(context);
        var newTicket = new Ticket(1, 1, "TEST-4", 4, "Fourth Ticket", "Description 4");

        // Act
        await repository.AddAsync(newTicket);
        await context.SaveChangesAsync();

        // Assert
        var ticketInDb = await context.Tickets.FirstOrDefaultAsync(t => t.Key == "TEST-4");
        Assert.NotNull(ticketInDb);
        Assert.Equal("Fourth Ticket", ticketInDb.Title);
    }

    [Fact]
    public async Task Update_ShouldModifyTicket()
    {
        // Arrange
        await using var context = new LucyWriteContext(DbContextOptions);
        await SeedDatabaseAsync(context);

        var repository = new TicketRepository(context);
        var ticket = await repository.GetByIdAsync(1);
        Assert.NotNull(ticket);

        // Act
        ticket.UpdateTitle("Updated First Ticket");
        repository.Update(ticket);
        await context.SaveChangesAsync();

        // Assert
        var updatedTicket = await context.Tickets.FindAsync(1L);
        Assert.NotNull(updatedTicket);
        Assert.Equal("Updated First Ticket", updatedTicket.Title);
    }

    [Fact]
    public async Task Remove_ShouldDeleteTicket()
    {
        // Arrange
        await using var context = new LucyWriteContext(DbContextOptions);
        await SeedDatabaseAsync(context);

        var repository = new TicketRepository(context);
        var ticket = await repository.GetByIdAsync(1);
        Assert.NotNull(ticket);

        // Act
        repository.Remove(ticket);
        await context.SaveChangesAsync();

        // Assert
        var deletedTicket = await context.Tickets.FindAsync(1L);
        Assert.Null(deletedTicket);
    }

    [Theory]
    [InlineData(true)]  // Test with TicketRepository
    [InlineData(false)] // Test with TicketReadOnlyRepository
    public async Task GetByIdAsync_ShouldReturnTicket_WhenIdExists(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(DbContextOptions);
        await SeedDatabaseAsync(writeContext);

        Ticket? ticket;

        // Act
        if (useWriteRepo)
        {
            var repository = new TicketRepository(writeContext);
            ticket = await repository.GetByIdAsync(1);
        }
        else
        {
            await using var readContext = new LucyReadContext(DbContextOptions);
            var readOnlyRepo = new TicketReadOnlyRepository(readContext);
            ticket = await readOnlyRepo.GetByIdAsync(1);
        }

        // Assert
        Assert.NotNull(ticket);
        Assert.Equal("TEST-1", ticket.Key);
        Assert.Equal("First Ticket", ticket.Title);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist(bool useWriteRepo)
    {
        // Arrange & Act
        Ticket? ticket;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(DbContextOptions);
            var repository = new TicketRepository(context);
            ticket = await repository.GetByIdAsync(999);
        }
        else
        {
            await using var context = new LucyReadContext(DbContextOptions);
            var repository = new TicketReadOnlyRepository(context);
            ticket = await repository.GetByIdAsync(999);
        }

        // Assert
        Assert.Null(ticket);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByKeyAsync_ShouldReturnTicket_WhenKeyExists(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(DbContextOptions);
        await SeedDatabaseAsync(writeContext);

        Ticket? ticket;

        // Act
        if (useWriteRepo)
        {
            var repository = new TicketRepository(writeContext);
            ticket = await repository.GetByKeyAsync("TEST-1");
        }
        else
        {
            await using var readContext = new LucyReadContext(DbContextOptions);
            var readOnlyRepo = new TicketReadOnlyRepository(readContext);
            ticket = await readOnlyRepo.GetByKeyAsync("TEST-1");
        }

        // Assert
        Assert.NotNull(ticket);
        Assert.Equal("TEST-1", ticket.Key);
        Assert.Equal("First Ticket", ticket.Title);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByKeyAsync_ShouldReturnNull_WhenKeyDoesNotExist(bool useWriteRepo)
    {
        // Arrange & Act
        Ticket? ticket;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(DbContextOptions);
            var repository = new TicketRepository(context);
            ticket = await repository.GetByKeyAsync("UNKNOWN");
        }
        else
        {
            await using var context = new LucyReadContext(DbContextOptions);
            var repository = new TicketReadOnlyRepository(context);
            ticket = await repository.GetByKeyAsync("UNKNOWN");
        }

        // Assert
        Assert.Null(ticket);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByProjectIdAsync_ShouldReturnAllTicketsForProject(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(DbContextOptions);
        await SeedDatabaseAsync(writeContext);

        List<Ticket> tickets;

        // Act
        if (useWriteRepo)
        {
            var repository = new TicketRepository(writeContext);
            tickets = await repository.GetByProjectIdAsync(1);
        }
        else
        {
            await using var readContext = new LucyReadContext(DbContextOptions);
            var readOnlyRepo = new TicketReadOnlyRepository(readContext);
            tickets = await readOnlyRepo.GetByProjectIdAsync(1);
        }

        // Assert
        Assert.NotNull(tickets);
        Assert.Equal(3, tickets.Count);
        Assert.Contains(tickets, t => t.Key == "TEST-1");
        Assert.Contains(tickets, t => t.Key == "TEST-2");
        Assert.Contains(tickets, t => t.Key == "TEST-3");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByProjectIdAsync_WithSorting_ShouldReturnSortedTickets(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(DbContextOptions);
        await SeedDatabaseAsync(writeContext);

        List<Ticket> tickets;

        // Act
        if (useWriteRepo)
        {
            var repository = new TicketRepository(writeContext);
            tickets = await repository.GetByProjectIdAsync(1, TicketField.Key, Application.Common.Queries.SortDirection.Descending);
        }
        else
        {
            await using var readContext = new LucyReadContext(DbContextOptions);
            var readOnlyRepo = new TicketReadOnlyRepository(readContext);
            tickets = await readOnlyRepo.GetByProjectIdAsync(1, TicketField.Key, Application.Common.Queries.SortDirection.Descending);
        }

        // Assert
        Assert.NotNull(tickets);
        Assert.Equal(3, tickets.Count);
        Assert.Equal("TEST-3", tickets[0].Key);
        Assert.Equal("TEST-2", tickets[1].Key);
        Assert.Equal("TEST-1", tickets[2].Key);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByStatusIdAsync_ShouldReturnAllTicketsForStatus(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(DbContextOptions);
        var (project, statuses, tickets) = await SeedDatabaseAsync(writeContext);

        List<Ticket> resultTickets;

        // Act
        if (useWriteRepo)
        {
            var repository = new TicketRepository(writeContext);
            resultTickets = await repository.GetByStatusIdAsync(statuses[0].Id);
        }
        else
        {
            await using var readContext = new LucyReadContext(DbContextOptions);
            var readOnlyRepo = new TicketReadOnlyRepository(readContext);
            resultTickets = await readOnlyRepo.GetByStatusIdAsync(statuses[0].Id);
        }

        // Assert
        Assert.NotNull(resultTickets);
        Assert.Equal(2, resultTickets.Count);
        Assert.Contains(resultTickets, t => t.Key == "TEST-1");
        Assert.Contains(resultTickets, t => t.Key == "TEST-2");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByStatusIdAsync_WithSorting_ShouldReturnSortedTickets(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(DbContextOptions);
        var (project, statuses, tickets) = await SeedDatabaseAsync(writeContext);

        List<Ticket> resultTickets;

        // Act
        if (useWriteRepo)
        {
            var repository = new TicketRepository(writeContext);
            resultTickets = await repository.GetByStatusIdAsync(statuses[0].Id, TicketField.Title, Application.Common.Queries.SortDirection.Ascending);
        }
        else
        {
            await using var readContext = new LucyReadContext(DbContextOptions);
            var readOnlyRepo = new TicketReadOnlyRepository(readContext);
            resultTickets = await readOnlyRepo.GetByStatusIdAsync(statuses[0].Id, TicketField.Title, Application.Common.Queries.SortDirection.Ascending);
        }

        // Assert
        Assert.NotNull(resultTickets);
        Assert.Equal(2, resultTickets.Count);
        Assert.Equal("First Ticket", tickets[0].Title);
        Assert.Equal("Second Ticket", tickets[1].Title);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExistsByIdAsync_ShouldReturnTrue_WhenIdExists(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(DbContextOptions);
        await SeedDatabaseAsync(writeContext);

        bool exists;

        // Act
        if (useWriteRepo)
        {
            var repository = new TicketRepository(writeContext);
            exists = await repository.ExistsByIdAsync(1);
        }
        else
        {
            await using var readContext = new LucyReadContext(DbContextOptions);
            var repository = new TicketReadOnlyRepository(readContext);
            exists = await repository.ExistsByIdAsync(1);
        }

        // Assert
        Assert.True(exists);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExistsByIdAsync_ShouldReturnFalse_WhenIdDoesNotExist(bool useWriteRepo)
    {
        // Arrange & Act
        bool exists;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(DbContextOptions);
            var repository = new TicketRepository(context);
            exists = await repository.ExistsByIdAsync(999);
        }
        else
        {
            await using var context = new LucyReadContext(DbContextOptions);
            var repository = new TicketReadOnlyRepository(context);
            exists = await repository.ExistsByIdAsync(999);
        }

        // Assert
        Assert.False(exists);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExistsByKeyAsync_ShouldReturnTrue_WhenKeyExists(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(DbContextOptions);
        await SeedDatabaseAsync(writeContext);

        bool exists;

        // Act
        if (useWriteRepo)
        {
            var repository = new TicketRepository(writeContext);
            exists = await repository.ExistsByKeyAsync("TEST-1");
        }
        else
        {
            await using var readContext = new LucyReadContext(DbContextOptions);
            var repository = new TicketReadOnlyRepository(readContext);
            exists = await repository.ExistsByKeyAsync("TEST-1");
        }

        // Assert
        Assert.True(exists);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExistsByKeyAsync_ShouldReturnFalse_WhenKeyDoesNotExist(bool useWriteRepo)
    {
        // Arrange & Act
        bool exists;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(DbContextOptions);
            var repository = new TicketRepository(context);
            exists = await repository.ExistsByKeyAsync("UNKNOWN");
        }
        else
        {
            await using var context = new LucyReadContext(DbContextOptions);
            var repository = new TicketReadOnlyRepository(context);
            exists = await repository.ExistsByKeyAsync("UNKNOWN");
        }

        // Assert
        Assert.False(exists);
    }
}
