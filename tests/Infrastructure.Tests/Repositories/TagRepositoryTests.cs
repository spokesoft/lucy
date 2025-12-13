using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Lucy.Infrastructure.Database;
using Lucy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Tests.Repositories;

[Collection("Database collection")]
public class TagRepositoryTests : RepositoryTestBase
{
    private async Task<(LucyWriteContext writeContext, LucyReadContext readContext)> CreateSeededContextsAsync()
    {
        var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var readContext = new LucyReadContext(_readDbContextOptions);
        await SeedDatabaseAsync(writeContext);
        return (writeContext, readContext);
    }

    private async Task<(Project project, Tag[] tags)> SeedDatabaseAsync(LucyDbContext context)
    {
        var project = new Project("TEST", "Test Project", "Test Description");
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var tag1 = new Tag(project.Id, "alpha", "Alpha", "Alpha description", Color.Red);
        var tag2 = new Tag(project.Id, "beta", "Beta", "Beta description", Color.Green);
        var tag3 = new Tag(project.Id, "gamma", "Gamma", "Gamma description", Color.Blue);

        context.Set<Tag>().AddRange(tag1, tag2, tag3);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        return (project, new[] { tag1, tag2, tag3 });
    }

    [Fact]
    public async Task AddAsync_ShouldAddTagToDatabase()
    {
        // Arrange
        await using var context = new LucyWriteContext(_writeDbContextOptions);
        var project = new Project("NEW", "New Project", "Desc");
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var repository = new TagRepository(context);
        var newTag = new Tag(project.Id, "newkey", "New Key", "New description", Color.Yellow);

        // Act
        await repository.AddAsync(newTag);
        await context.SaveChangesAsync();

        // Assert
        var tagInDb = await context.Set<Tag>().FirstOrDefaultAsync(t => t.Key == "NEWKEY");
        Assert.NotNull(tagInDb);
        Assert.Equal("New Key", tagInDb.Label);
        Assert.Equal(Color.Yellow, tagInDb.Color);
    }

    [Fact]
    public async Task Update_ShouldModifyTag()
    {
        // Arrange
        await using var context = new LucyWriteContext(_writeDbContextOptions);
        var (project, tags) = await SeedDatabaseAsync(context);

        var repository = new TagRepository(context);
        var tag = await repository.GetByIdAsync(tags[0].Id);
        Assert.NotNull(tag);

        // Act
        tag.UpdateKey("updated");
        tag.UpdateLabel("Updated Label");
        tag.UpdateDescription("Updated description");
        tag.UpdateColor(Color.Purple);
        repository.Update(tag);
        await context.SaveChangesAsync();

        // Assert
        var updated = await context.Set<Tag>().FindAsync(tags[0].Id);
        Assert.NotNull(updated);
        Assert.Equal("UPDATED", updated.Key);
        Assert.Equal("Updated Label", updated.Label);
        Assert.Equal("Updated description", updated.Description);
        Assert.Equal(Color.Purple, updated.Color);
    }

    [Fact]
    public async Task Remove_ShouldDeleteTag()
    {
        // Arrange
        await using var context = new LucyWriteContext(_writeDbContextOptions);
        var (project, tags) = await SeedDatabaseAsync(context);

        var repository = new TagRepository(context);
        var tag = await repository.GetByIdAsync(tags[1].Id);
        Assert.NotNull(tag);

        // Act
        repository.Remove(tag);
        await context.SaveChangesAsync();

        // Assert
        var deleted = await context.Set<Tag>().FindAsync(tags[1].Id);
        Assert.Null(deleted);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsync_ShouldReturnTag_WhenIdExists(bool useWriteRepo)
    {
        // Arrange
        var (writeContext, readContext) = await CreateSeededContextsAsync();

        var tagId = await writeContext.Set<Tag>().Where(t => t.Key == "ALPHA").Select(t => t.Id).FirstAsync();

        Tag? tag;

        // Act
        if (useWriteRepo)
        {
            var repository = new TagRepository(writeContext);
            tag = await repository.GetByIdAsync(tagId);
        }
        else
        {
            var repository = new TagReadOnlyRepository(readContext);
            tag = await repository.GetByIdAsync(tagId);
        }

        // Assert
        Assert.NotNull(tag);
        Assert.Equal("ALPHA", tag.Key);
        Assert.Equal("Alpha", tag.Label);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist(bool useWriteRepo)
    {
        // Arrange & Act
        Tag? tag;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(_writeDbContextOptions);
            var repository = new TagRepository(context);
            tag = await repository.GetByIdAsync(999);
        }
        else
        {
            await using var context = new LucyReadContext(_readDbContextOptions);
            var repository = new TagReadOnlyRepository(context);
            tag = await repository.GetByIdAsync(999);
        }

        // Assert
        Assert.Null(tag);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByKeyAsync_ShouldReturnTag_WhenKeyExists(bool useWriteRepo)
    {
        // Arrange
        var (writeContext, readContext) = await CreateSeededContextsAsync();

        Tag? tag;

        // Act
        if (useWriteRepo)
        {
            var repository = new TagRepository(writeContext);
            tag = await repository.GetByKeyAsync(1, "alpha");
        }
        else
        {
            var repository = new TagReadOnlyRepository(readContext);
            tag = await repository.GetByKeyAsync(1, "ALPHA");
        }

        // Assert
        Assert.NotNull(tag);
        Assert.Equal("ALPHA", tag.Key);
        Assert.Equal("Alpha", tag.Label);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByKeyAsync_ShouldReturnNull_WhenKeyDoesNotExist(bool useWriteRepo)
    {
        // Arrange & Act
        Tag? tag;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(_writeDbContextOptions);
            var repository = new TagRepository(context);
            tag = await repository.GetByKeyAsync(1, "unknown");
        }
        else
        {
            await using var context = new LucyReadContext(_readDbContextOptions);
            var repository = new TagReadOnlyRepository(context);
            tag = await repository.GetByKeyAsync(1, "unknown");
        }

        // Assert
        Assert.Null(tag);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByProjectIdAsync_ShouldReturnAllTagsForProject(bool useWriteRepo)
    {
        // Arrange
        var (writeContext, readContext) = await CreateSeededContextsAsync();

        IEnumerable<Tag> tags;

        // Act
        if (useWriteRepo)
        {
            var repository = new TagRepository(writeContext);
            tags = await repository.GetByProjectIdAsync(1);
        }
        else
        {
            var repository = new TagReadOnlyRepository(readContext);
            tags = await repository.GetByProjectIdAsync(1);
        }

        // Assert
        Assert.NotNull(tags);
        Assert.Equal(3, tags.Count());
        Assert.Contains(tags, t => t.Key == "ALPHA");
        Assert.Contains(tags, t => t.Key == "BETA");
        Assert.Contains(tags, t => t.Key == "GAMMA");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExistsByIdAsync_ShouldReturnTrue_WhenIdExists(bool useWriteRepo)
    {
        // Arrange
        var (writeContext, readContext) = await CreateSeededContextsAsync();
        var existingId = await writeContext.Set<Tag>().Select(t => t.Id).FirstAsync();

        bool exists;

        // Act
        if (useWriteRepo)
        {
            var repository = new TagRepository(writeContext);
            exists = await repository.ExistsByIdAsync(existingId);
        }
        else
        {
            var repository = new TagReadOnlyRepository(readContext);
            exists = await repository.ExistsByIdAsync(existingId);
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
            var repository = new TagRepository(context);
            exists = await repository.ExistsByIdAsync(999);
        }
        else
        {
            await using var context = new LucyReadContext(_readDbContextOptions);
            var repository = new TagReadOnlyRepository(context);
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
        var (writeContext, readContext) = await CreateSeededContextsAsync();

        bool exists;

        // Act
        if (useWriteRepo)
        {
            var repository = new TagRepository(writeContext);
            exists = await repository.ExistsByKeyAsync(1, "alpha");
        }
        else
        {
            var repository = new TagReadOnlyRepository(readContext);
            exists = await repository.ExistsByKeyAsync(1, "ALPHA");
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
            var repository = new TagRepository(context);
            exists = await repository.ExistsByKeyAsync(1, "unknown");
        }
        else
        {
            await using var context = new LucyReadContext(_readDbContextOptions);
            var repository = new TagReadOnlyRepository(context);
            exists = await repository.ExistsByKeyAsync(1, "unknown");
        }

        // Assert
        Assert.False(exists);
    }
}
