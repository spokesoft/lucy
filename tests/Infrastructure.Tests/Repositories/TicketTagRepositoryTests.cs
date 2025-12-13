using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Lucy.Infrastructure.Database;
using Lucy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Tests.Repositories;

[Collection("Database collection")]
public class TicketTagRepositoryTests : RepositoryTestBase
{
    private async Task<(LucyWriteContext writeContext, LucyReadContext readContext)> CreateSeededContextsAsync()
    {
        var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var readContext = new LucyReadContext(_readDbContextOptions);
        await SeedDatabaseAsync(writeContext);
        return (writeContext, readContext);
    }

    private async Task<(Project project, Ticket ticket, Tag[] tags)> SeedDatabaseAsync(LucyDbContext context)
    {
        var project = new Project("TEST", "Test Project", "Test Description");
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var status = project.Statuses.First();
        var ticket = new Ticket(project.Id, status.Id, "TEST-1", 1, "Test Ticket", "Test ticket description");
        context.Tickets.Add(ticket);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var tag1 = new Tag(project.Id, "alpha", "Alpha", null, Color.Red);
        var tag2 = new Tag(project.Id, "beta", "Beta", null, Color.Green);
        context.Set<Tag>().AddRange(tag1, tag2);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        // Reload ticket and tags with ids
        ticket = await context.Set<Ticket>().FirstAsync(t => t.Key == "TEST-1");
        var tags = await context.Set<Tag>().Where(t => t.ProjectId == project.Id).ToArrayAsync();

        return (project, ticket, tags);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTicketTagToDatabase()
    {
        // Arrange
        var (writeContext, _) = await CreateSeededContextsAsync();
        var ticket = await writeContext.Set<Ticket>().FirstAsync(t => t.Key == "TEST-1");
        var tags = await writeContext.Set<Tag>().Where(t => t.ProjectId == ticket.ProjectId).ToArrayAsync();

        var repository = new TicketTagRepository(writeContext);
        var ticketTag = new TicketTag(ticket, tags[0]);

        // Act
        await repository.AddAsync(ticketTag);
        await writeContext.SaveChangesAsync();

        // Assert
        var inDb = await writeContext.Set<TicketTag>().FirstOrDefaultAsync(tt => tt.TicketId == ticket.Id && tt.TagId == tags[0].Id);
        Assert.NotNull(inDb);
        Assert.Equal(ticket.Id, inDb.TicketId);
        Assert.Equal(tags[0].Id, inDb.TagId);
    }

    [Fact]
    public async Task GetByTicketAndTagAsync_ShouldReturnTicketTag_WhenExists()
    {
        // Arrange
        var (writeContext, readContext) = await CreateSeededContextsAsync();
        var ticket = await writeContext.Set<Ticket>().FirstAsync(t => t.Key == "TEST-1");
        var tags = await writeContext.Set<Tag>().Where(t => t.ProjectId == ticket.ProjectId).ToArrayAsync();

        var repo = new TicketTagRepository(writeContext);
        var tt = new TicketTag(ticket, tags[1]);
        await repo.AddAsync(tt);
        await writeContext.SaveChangesAsync();

        // Act
        var writeRepo = new TicketTagRepository(writeContext);
        var fromWrite = await writeRepo.GetByTicketAndTagAsync(ticket.Id, tags[1].Id);

        var readRepo = new TicketTagReadOnlyRepository(readContext);
        var fromRead = await readRepo.GetByTicketAndTagAsync(ticket.Id, tags[1].Id);

        // Assert
        Assert.NotNull(fromWrite);
        Assert.Equal(ticket.Id, fromWrite.TicketId);
        Assert.NotNull(fromRead);
        Assert.Equal(tags[1].Id, fromRead.TagId);
    }

    [Fact]
    public async Task Remove_ShouldDeleteTicketTag()
    {
        // Arrange
        var (writeContext, _) = await CreateSeededContextsAsync();
        var ticket = await writeContext.Set<Ticket>().FirstAsync(t => t.Key == "TEST-1");
        var tags = await writeContext.Set<Tag>().Where(t => t.ProjectId == ticket.ProjectId).ToArrayAsync();

        var repo = new TicketTagRepository(writeContext);
        var tt = new TicketTag(ticket, tags[0]);
        await repo.AddAsync(tt);
        await writeContext.SaveChangesAsync();

        var added = await writeContext.Set<TicketTag>().FirstAsync(x => x.TicketId == ticket.Id && x.TagId == tags[0].Id);

        // Act
        repo.Remove(added);
        await writeContext.SaveChangesAsync();

        // Assert
        var deleted = await writeContext.Set<TicketTag>().FindAsync(added.Id);
        Assert.Null(deleted);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist(bool useWriteRepo)
    {
        // Arrange & Act
        TicketTag? tt;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(_writeDbContextOptions);
            var repository = new TicketTagRepository(context);
            tt = await repository.GetByIdAsync(999);
        }
        else
        {
            await using var context = new LucyReadContext(_readDbContextOptions);
            var repository = new TicketTagReadOnlyRepository(context);
            tt = await repository.GetByIdAsync(999);
        }

        // Assert
        Assert.Null(tt);
    }
}
