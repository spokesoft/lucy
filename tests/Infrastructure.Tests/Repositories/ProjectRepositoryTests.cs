using Lucy.Domain.Entities;
using Lucy.Infrastructure.Database;
using Lucy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Tests.Repositories;

/// <summary>
/// Tests for the ProjectRepository.
/// </summary>
[Collection("Database collection")]
public class ProjectRepositoryTests : RepositoryTestBase
{
    private async Task SeedDatabaseAsync(LucyDbContext context)
    {
        var project = new Project("LUCY", "Lucy Project", "The main project.");
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task AddAsync_ShouldAddProjectToDatabase()
    {
        // Arrange
        await using var context = new LucyWriteContext(DbContextOptions);
        var repository = new ProjectRepository(context);
        var newProject = new Project("NEW", "New Project", null);

        // Act
        await repository.AddAsync(newProject);
        await context.SaveChangesAsync();

        // Assert
        var projectInDb = await context.Projects.FirstOrDefaultAsync(p => p.Key == "NEW");
        Assert.NotNull(projectInDb);
        Assert.Equal("New Project", projectInDb.Name);
    }

    [Theory]
    [InlineData(true)]  // Test with ProjectRepository
    [InlineData(false)] // Test with ProjectReadOnlyRepository
    public async Task GetByKeyAsync_ShouldReturnProject_WhenKeyExists(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(DbContextOptions);
        await SeedDatabaseAsync(writeContext);

        Project? project;

        // Act
        if (useWriteRepo)
        {
            var repository = new ProjectRepository(writeContext);
            project = await repository.GetByKeyAsync("LUCY");
        }
        else
        {
            await using var readContext = new LucyReadContext(DbContextOptions);
            var readOnlyRepo = new ProjectReadOnlyRepository(readContext);
            project = await readOnlyRepo.GetByKeyAsync("LUCY");
        }

        // Assert
        Assert.NotNull(project);
        Assert.Equal("LUCY", project.Key);
        Assert.Equal("Lucy Project", project.Name);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByKeyAsync_ShouldReturnNull_WhenKeyDoesNotExist(bool useWriteRepo)
    {
        // Arrange & Act
        Project? project;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(DbContextOptions);
            var repository = new ProjectRepository(context);
            project = await repository.GetByKeyAsync("UNKNOWN");
        }
        else
        {
            await using var context = new LucyReadContext(DbContextOptions);
            var repository = new ProjectReadOnlyRepository(context);
            project = await repository.GetByKeyAsync("UNKNOWN");
        }

        // Assert
        Assert.Null(project);
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
            var repository = new ProjectRepository(writeContext);
            exists = await repository.ExistsByKeyAsync("LUCY");
        }
        else
        {
            await using var readContext = new LucyReadContext(DbContextOptions);
            var repository = new ProjectReadOnlyRepository(readContext);
            exists = await repository.ExistsByKeyAsync("LUCY");
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
            var repository = new ProjectRepository(context);
            exists = await repository.ExistsByKeyAsync("UNKNOWN");
        }
        else
        {
            await using var context = new LucyReadContext(DbContextOptions);
            var repository = new ProjectReadOnlyRepository(context);
            exists = await repository.ExistsByKeyAsync("UNKNOWN");
        }

        // Assert
        Assert.False(exists);
    }
}
