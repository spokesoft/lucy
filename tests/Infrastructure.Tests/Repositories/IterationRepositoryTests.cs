using Lucy.Application.Iterations.Queries;
using Lucy.Application.Queries;
using Lucy.Domain.Entities;
using Lucy.Infrastructure.Database;
using Lucy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Tests.Repositories;

[Collection("Database collection")]
public class IterationRepositoryTests : RepositoryTestBase
{
    private async Task<(Project project, Iteration[] iterations)> SeedDatabaseAsync(LucyDbContext context)
    {
        var project = new Project("TEST", "Test Project", "Test Description");
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var iteration1 = new Iteration(project.Id, "ITER-1", 1, "Iteration 1", "First iteration", DateTime.UtcNow, DateTime.UtcNow.AddDays(14));
        var iteration2 = new Iteration(project.Id, "ITER-2", 2, "Iteration 2", "Second iteration", DateTime.UtcNow.AddDays(14), DateTime.UtcNow.AddDays(28));
        var iteration3 = new Iteration(project.Id, "ITER-3", 3, "Iteration 3", "Third iteration", DateTime.UtcNow.AddDays(28), DateTime.UtcNow.AddDays(42));

        context.Set<Iteration>().AddRange(iteration1, iteration2, iteration3);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        return (project, new[] { iteration1, iteration2, iteration3 });
    }

    [Fact]
    public async Task AddAsync_ShouldAddIterationToDatabase()
    {
        // Arrange
        await using var context = new LucyWriteContext(_writeDbContextOptions);
        var project = new Project("NEW", "New Project", "Desc");
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var repository = new IterationRepository(context);
        var newIteration = new Iteration(project.Id, "NEW-1", 1, "New Iteration", "Description", DateTime.UtcNow, DateTime.UtcNow.AddDays(7));

        // Act
        await repository.AddAsync(newIteration);
        await context.SaveChangesAsync();

        // Assert
        var iterationInDb = await context.Set<Iteration>().FirstOrDefaultAsync(i => i.Key == "NEW-1");
        Assert.NotNull(iterationInDb);
        Assert.Equal("New Iteration", iterationInDb.Name);
        Assert.Equal(project.Id, iterationInDb.ProjectId);
    }

    [Fact]
    public async Task Update_ShouldModifyIteration()
    {
        // Arrange
        await using var context = new LucyWriteContext(_writeDbContextOptions);
        var (project, iterations) = await SeedDatabaseAsync(context);

        var repository = new IterationRepository(context);
        var iteration = await repository.GetByIdAsync(iterations[0].Id);
        Assert.NotNull(iteration);

        // Act
        iteration.UpdateName("Updated Iteration");
        iteration.UpdateDescription("Updated Description");
        repository.Update(iteration);
        await context.SaveChangesAsync();

        // Assert
        var updated = await context.Set<Iteration>().FindAsync(iterations[0].Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Iteration", updated.Name);
        Assert.Equal("Updated Description", updated.Description);
    }

    [Fact]
    public async Task Remove_ShouldDeleteIteration()
    {
        // Arrange
        await using var context = new LucyWriteContext(_writeDbContextOptions);
        var (project, iterations) = await SeedDatabaseAsync(context);

        var repository = new IterationRepository(context);
        var iteration = await repository.GetByIdAsync(iterations[1].Id);
        Assert.NotNull(iteration);

        // Act
        repository.Remove(iteration);
        await context.SaveChangesAsync();

        // Assert
        var deleted = await context.Set<Iteration>().FindAsync(iterations[1].Id);
        Assert.Null(deleted);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsync_ShouldReturnIteration_WhenIdExists(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var (project, iterations) = await SeedDatabaseAsync(writeContext);
        var iterationId = iterations[0].Id;

        Iteration? iteration;

        // Act
        if (useWriteRepo)
        {
            var repository = new IterationRepository(writeContext);
            iteration = await repository.GetByIdAsync(iterationId);
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new IterationReadOnlyRepository(readContext);
            iteration = await repository.GetByIdAsync(iterationId);
        }

        // Assert
        Assert.NotNull(iteration);
        Assert.Equal(iterationId, iteration.Id);
        Assert.Equal("ITER-1", iteration.Key);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist(bool useWriteRepo)
    {
        // Arrange & Act
        Iteration? iteration;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(_writeDbContextOptions);
            var repository = new IterationRepository(context);
            iteration = await repository.GetByIdAsync(999);
        }
        else
        {
            await using var context = new LucyReadContext(_readDbContextOptions);
            var repository = new IterationReadOnlyRepository(context);
            iteration = await repository.GetByIdAsync(999);
        }

        // Assert
        Assert.Null(iteration);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByKeyAsync_ShouldReturnIteration_WhenKeyExists(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var (project, iterations) = await SeedDatabaseAsync(writeContext);

        Iteration? iteration;

        // Act
        if (useWriteRepo)
        {
            var repository = new IterationRepository(writeContext);
            iteration = await repository.GetByKeyAsync("ITER-1");
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new IterationReadOnlyRepository(readContext);
            iteration = await repository.GetByKeyAsync("ITER-1");
        }

        // Assert
        Assert.NotNull(iteration);
        Assert.Equal("ITER-1", iteration.Key);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByKeyAsync_ShouldReturnNull_WhenKeyDoesNotExist(bool useWriteRepo)
    {
        // Arrange & Act
        Iteration? iteration;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(_writeDbContextOptions);
            var repository = new IterationRepository(context);
            iteration = await repository.GetByKeyAsync("UNKNOWN");
        }
        else
        {
            await using var context = new LucyReadContext(_readDbContextOptions);
            var repository = new IterationReadOnlyRepository(context);
            iteration = await repository.GetByKeyAsync("UNKNOWN");
        }

        // Assert
        Assert.Null(iteration);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByProjectIdAsync_ShouldReturnAllIterationsForProject(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var (project, iterations) = await SeedDatabaseAsync(writeContext);

        List<Iteration> result;

        // Act
        if (useWriteRepo)
        {
            var repository = new IterationRepository(writeContext);
            result = await repository.GetByProjectIdAsync(project.Id);
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new IterationReadOnlyRepository(readContext);
            result = await repository.GetByProjectIdAsync(project.Id);
        }

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(result, i => i.Key == "ITER-1");
        Assert.Contains(result, i => i.Key == "ITER-2");
        Assert.Contains(result, i => i.Key == "ITER-3");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExistsByIdAsync_ShouldReturnTrue_WhenIdExists(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var (project, iterations) = await SeedDatabaseAsync(writeContext);
        var iterationId = iterations[0].Id;

        bool exists;

        // Act
        if (useWriteRepo)
        {
            var repository = new IterationRepository(writeContext);
            exists = await repository.ExistsByIdAsync(iterationId);
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new IterationReadOnlyRepository(readContext);
            exists = await repository.ExistsByIdAsync(iterationId);
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
            await using var context = new LucyWriteContext(_writeDbContextOptions);
            var repository = new IterationRepository(context);
            exists = await repository.ExistsByIdAsync(999);
        }
        else
        {
            await using var context = new LucyReadContext(_readDbContextOptions);
            var repository = new IterationReadOnlyRepository(context);
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
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var (project, iterations) = await SeedDatabaseAsync(writeContext);

        bool exists;

        // Act
        if (useWriteRepo)
        {
            var repository = new IterationRepository(writeContext);
            exists = await repository.ExistsByKeyAsync("ITER-1");
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new IterationReadOnlyRepository(readContext);
            exists = await repository.ExistsByKeyAsync("ITER-1");
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
            await using var context = new LucyWriteContext(_writeDbContextOptions);
            var repository = new IterationRepository(context);
            exists = await repository.ExistsByKeyAsync("UNKNOWN");
        }
        else
        {
            await using var context = new LucyReadContext(_readDbContextOptions);
            var repository = new IterationReadOnlyRepository(context);
            exists = await repository.ExistsByKeyAsync("UNKNOWN");
        }

        // Assert
        Assert.False(exists);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetAllAsync_ShouldReturnSortedIterations(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var (project, iterations) = await SeedDatabaseAsync(writeContext);

        List<Iteration> result;

        // Act
        if (useWriteRepo)
        {
            var repository = new IterationRepository(writeContext);
            result = await repository.GetAllAsync(IterationSortField.Name, SortDirection.Descending);
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new IterationReadOnlyRepository(readContext);
            result = await repository.GetAllAsync(IterationSortField.Name, SortDirection.Descending);
        }

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Iteration 3", result[0].Name);
        Assert.Equal("Iteration 2", result[1].Name);
        Assert.Equal("Iteration 1", result[2].Name);
    }
}
