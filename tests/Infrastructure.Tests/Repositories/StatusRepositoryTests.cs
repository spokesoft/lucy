using Lucy.Application.Statuses.Queries;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Lucy.Infrastructure.Database;
using Lucy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lucy.Tests.Infrastructure.Repositories;

[Collection("Database collection")]
public class StatusRepositoryTests
{
    private static DbContextOptions<LucyDbContext> CreateDbContextOptions()
    {
        var databaseName = Guid.NewGuid().ToString();
        var databaseRoot = new InMemoryDatabaseRoot();

        return new DbContextOptionsBuilder<LucyDbContext>()
            .UseInMemoryDatabase(databaseName, databaseRoot)
            .EnableServiceProviderCaching(false)
            .Options;
    }

    private async Task<LucyWriteContext> CreateSeededWriteContextAsync()
    {
        var options = CreateDbContextOptions();
        var context = new LucyWriteContext(options);
        await SeedDatabaseAsync(context);
        return context;
    }

    private async Task<(LucyWriteContext writeContext, LucyReadContext readContext)> CreateSeededContextsAsync()
    {
        var options = CreateDbContextOptions();
        var writeContext = new LucyWriteContext(options);
        var readContext = new LucyReadContext(options);
        await SeedDatabaseAsync(writeContext);
        return (writeContext, readContext);
    }

    private async Task<(Project project, Status[] statuses)> SeedDatabaseAsync(LucyDbContext context)
    {
        var project = new Project("TEST", "Test Project", "Test Description");
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        // Project constructor automatically creates 3 default statuses, so just return them
        var statuses = project.Statuses.ToArray();

        return (project, statuses);
    }

    // --- Tests for StatusRepository (Write) ---

    [Fact]
    public async Task AddAsync_ShouldAddStatusToDatabase()
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var context = new LucyWriteContext(options);
        var (project, statuses) = await SeedDatabaseAsync(context);

        var repository = new StatusRepository(context);
        var newStatus = new Status(project.Id, "REVIEW", 4, "Review", "Under review", Color.Yellow);

        // Act
        await repository.AddAsync(newStatus);
        await context.SaveChangesAsync();

        // Assert
        var statusInDb = await context.Statuses.FirstOrDefaultAsync(s => s.Key == "REVIEW");
        Assert.NotNull(statusInDb);
        Assert.Equal("Review", statusInDb.Name);
    }

    [Fact]
    public async Task Update_ShouldModifyStatus()
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var context = new LucyWriteContext(options);
        var (project, statuses) = await SeedDatabaseAsync(context);

        var repository = new StatusRepository(context);
        var status = await repository.GetByIdAsync(statuses[0].Id);
        Assert.NotNull(status);

        // Act
        status.UpdateName("Updated To Do");
        repository.Update(status);
        await context.SaveChangesAsync();

        // Assert
        var updatedStatus = await context.Statuses.FindAsync(statuses[0].Id);
        Assert.NotNull(updatedStatus);
        Assert.Equal("Updated To Do", updatedStatus.Name);
    }

    [Fact]
    public async Task Remove_ShouldDeleteStatus()
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var context = new LucyWriteContext(options);
        var (project, statuses) = await SeedDatabaseAsync(context);

        var repository = new StatusRepository(context);
        var status = await repository.GetByIdAsync(statuses[0].Id);
        Assert.NotNull(status);

        // Act
        repository.Remove(status);
        await context.SaveChangesAsync();

        // Assert
        var deletedStatus = await context.Statuses.FindAsync(statuses[0].Id);
        Assert.Null(deletedStatus);
    }

    // --- Tests for both repositories (Read functionality) ---

    [Theory]
    [InlineData(true)]  // Test with StatusRepository
    [InlineData(false)] // Test with StatusReadOnlyRepository
    public async Task GetByIdAsync_ShouldReturnStatus_WhenIdExists(bool useWriteRepo)
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var writeContext = new LucyWriteContext(options);
        var (project, statuses) = await SeedDatabaseAsync(writeContext);

        Status? status;

        // Act
        if (useWriteRepo)
        {
            var repository = new StatusRepository(writeContext);
            status = await repository.GetByIdAsync(statuses[0].Id);
        }
        else
        {
            await using var readContext = new LucyReadContext(options);
            var readOnlyRepo = new StatusReadOnlyRepository(readContext);
            status = await readOnlyRepo.GetByIdAsync(statuses[0].Id);
        }

        // Assert
        Assert.NotNull(status);
        Assert.Equal("TODO", status.Key);
        Assert.Equal("To Do", status.Name);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist(bool useWriteRepo)
    {
        // Arrange & Act
        var options = CreateDbContextOptions();
        Status? status;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(options);
            var repository = new StatusRepository(context);
            status = await repository.GetByIdAsync(999);
        }
        else
        {
            await using var context = new LucyReadContext(options);
            var repository = new StatusReadOnlyRepository(context);
            status = await repository.GetByIdAsync(999);
        }

        // Assert
        Assert.Null(status);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByKeyAsync_ShouldReturnStatus_WhenKeyExists(bool useWriteRepo)
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var writeContext = new LucyWriteContext(options);
        var (project, statuses) = await SeedDatabaseAsync(writeContext);

        Status? status;

        // Act
        if (useWriteRepo)
        {
            var repository = new StatusRepository(writeContext);
            status = await repository.GetByKeyAsync(1, "TODO");
        }
        else
        {
            await using var readContext = new LucyReadContext(options);
            var readOnlyRepo = new StatusReadOnlyRepository(readContext);
            status = await readOnlyRepo.GetByKeyAsync(1, "TODO");
        }

        // Assert
        Assert.NotNull(status);
        Assert.Equal("TODO", status.Key);
        Assert.Equal("To Do", status.Name);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByKeyAsync_ShouldReturnNull_WhenKeyDoesNotExist(bool useWriteRepo)
    {
        // Arrange & Act
        var options = CreateDbContextOptions();
        Status? status;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(options);
            var repository = new StatusRepository(context);
            status = await repository.GetByKeyAsync(1, "UNKNOWN");
        }
        else
        {
            await using var context = new LucyReadContext(options);
            var repository = new StatusReadOnlyRepository(context);
            status = await repository.GetByKeyAsync(1, "UNKNOWN");
        }

        // Assert
        Assert.Null(status);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByProjectIdAsync_ShouldReturnAllStatusesForProject(bool useWriteRepo)
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var writeContext = new LucyWriteContext(options);
        var (project, seededStatuses) = await SeedDatabaseAsync(writeContext);

        IEnumerable<Status> statuses;

        // Act
        if (useWriteRepo)
        {
            var repository = new StatusRepository(writeContext);
            statuses = await repository.GetByProjectIdAsync(project.Id);
        }
        else
        {
            await using var readContext = new LucyReadContext(options);
            var readOnlyRepo = new StatusReadOnlyRepository(readContext);
            statuses = await readOnlyRepo.GetByProjectIdAsync(project.Id);
        }

        // Assert
        Assert.NotNull(statuses);
        Assert.Equal(3, statuses.Count());
        Assert.Contains(statuses, s => s.Key == "TODO");
        Assert.Contains(statuses, s => s.Key == "IN-PROGRESS");
        Assert.Contains(statuses, s => s.Key == "DONE");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByProjectIdAsync_WithSorting_ShouldReturnSortedStatuses(bool useWriteRepo)
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var writeContext = new LucyWriteContext(options);
        var (project, seededStatuses) = await SeedDatabaseAsync(writeContext);

        List<Status> statuses;

        // Act
        if (useWriteRepo)
        {
            var repository = new StatusRepository(writeContext);
            statuses = await repository.GetByProjectIdAsync(project.Id, StatusSortField.Order, Application.Queries.SortDirection.Descending);
        }
        else
        {
            await using var readContext = new LucyReadContext(options);
            var readOnlyRepo = new StatusReadOnlyRepository(readContext);
            statuses = await readOnlyRepo.GetByProjectIdAsync(project.Id, StatusSortField.Order, Application.Queries.SortDirection.Descending);
        }

        // Assert
        Assert.NotNull(statuses);
        Assert.Equal(3, statuses.Count);
        Assert.Equal("DONE", statuses[0].Key);
        Assert.Equal("IN-PROGRESS", statuses[1].Key);
        Assert.Equal("TODO", statuses[2].Key);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExistsByIdAsync_ShouldReturnTrue_WhenIdExists(bool useWriteRepo)
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var writeContext = new LucyWriteContext(options);
        var (project, statuses) = await SeedDatabaseAsync(writeContext);

        bool exists;

        // Act
        if (useWriteRepo)
        {
            var repository = new StatusRepository(writeContext);
            exists = await repository.ExistsByIdAsync(statuses[0].Id);
        }
        else
        {
            await using var readContext = new LucyReadContext(options);
            var repository = new StatusReadOnlyRepository(readContext);
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
        var options = CreateDbContextOptions();
        bool exists;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(options);
            var repository = new StatusRepository(context);
            exists = await repository.ExistsByIdAsync(999);
        }
        else
        {
            await using var context = new LucyReadContext(options);
            var repository = new StatusReadOnlyRepository(context);
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
        var options = CreateDbContextOptions();
        await using var writeContext = new LucyWriteContext(options);
        var (project, statuses) = await SeedDatabaseAsync(writeContext);

        bool exists;

        // Act
        if (useWriteRepo)
        {
            var repository = new StatusRepository(writeContext);
            exists = await repository.ExistsByKeyAsync(1, "TODO");
        }
        else
        {
            await using var readContext = new LucyReadContext(options);
            var repository = new StatusReadOnlyRepository(readContext);
            exists = await repository.ExistsByKeyAsync(1, "TODO");
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
        var options = CreateDbContextOptions();
        bool exists;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(options);
            var repository = new StatusRepository(context);
            exists = await repository.ExistsByKeyAsync(1, "UNKNOWN");
        }
        else
        {
            await using var context = new LucyReadContext(options);
            var repository = new StatusReadOnlyRepository(context);
            exists = await repository.ExistsByKeyAsync(1, "UNKNOWN");
        }

        // Assert
        Assert.False(exists);
    }
}
