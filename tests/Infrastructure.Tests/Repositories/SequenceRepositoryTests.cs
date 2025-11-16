using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Lucy.Infrastructure.Database;
using Lucy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lucy.Tests.Infrastructure.Repositories;

[Collection("Database collection")]
public class SequenceRepositoryTests
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

    private async Task<(Project project, Sequence[] sequences)> SeedDatabaseAsync(LucyDbContext context)
    {
        var project = new Project("TEST", "Test Project", "Test Description");
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        // Project constructor automatically creates 2 sequences (Ticket and Iteration)
        var sequences = project.Sequences.ToArray();

        return (project, sequences);
    }

    // --- Tests for SequenceRepository (Write) ---

    [Fact]
    public async Task AddAsync_ShouldAddSequenceToDatabase()
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var context = new LucyWriteContext(options);
        var repository = new SequenceRepository(context);

        var project = new Project("TEST", "Test Project", "Test Description");
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var newSequence = new Sequence(SequenceType.Ticket, project.Id, value: 0, template: "CUSTOM-{0}");

        // Act
        await repository.AddAsync(newSequence);
        await context.SaveChangesAsync();

        // Assert
        var sequenceInDb = await context.Sequences
            .FirstOrDefaultAsync(s => s.Template == "CUSTOM-{0}");
        Assert.NotNull(sequenceInDb);
        Assert.Equal(SequenceType.Ticket, sequenceInDb.Type);
    }

    [Fact]
    public async Task Update_ShouldModifySequence()
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var context = new LucyWriteContext(options);
        var repository = new SequenceRepository(context);
        var (project, sequences) = await SeedDatabaseAsync(context);

        var sequence = sequences[0];
        var originalValue = sequence.Value;

        // Act
        sequence.UpdateValue(100);
        repository.Update(sequence);
        await context.SaveChangesAsync();

        // Assert
        var updatedSequence = await context.Sequences.FindAsync(sequence.Id);
        Assert.NotNull(updatedSequence);
        Assert.Equal(100, updatedSequence.Value);
        Assert.NotEqual(originalValue, updatedSequence.Value);
    }

    [Fact]
    public async Task Remove_ShouldDeleteSequence()
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var context = new LucyWriteContext(options);
        var repository = new SequenceRepository(context);
        var (project, sequences) = await SeedDatabaseAsync(context);

        var sequenceToRemove = sequences[0];
        var sequenceId = sequenceToRemove.Id;

        // Act
        repository.Remove(sequenceToRemove);
        await context.SaveChangesAsync();

        // Assert
        var deletedSequence = await context.Sequences.FindAsync(sequenceId);
        Assert.Null(deletedSequence);
    }

    // --- Tests for both repositories (Read functionality) ---

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsync_ShouldReturnSequence_WhenIdExists(bool useWriteRepo)
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var writeContext = new LucyWriteContext(options);
        var (project, sequences) = await SeedDatabaseAsync(writeContext);

        Sequence? sequence;

        // Act
        if (useWriteRepo)
        {
            var repository = new SequenceRepository(writeContext);
            sequence = await repository.GetByIdAsync(sequences[0].Id);
        }
        else
        {
            await using var readContext = new LucyReadContext(options);
            var readOnlyRepo = new SequenceReadOnlyRepository(readContext);
            sequence = await readOnlyRepo.GetByIdAsync(sequences[0].Id);
        }

        // Assert
        Assert.NotNull(sequence);
        Assert.Equal(sequences[0].Id, sequence.Id);
        Assert.Equal(project.Id, sequence.ProjectId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist(bool useWriteRepo)
    {
        // Arrange
        var options = CreateDbContextOptions();
        Sequence? sequence;

        // Act
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(options);
            var repository = new SequenceRepository(context);
            sequence = await repository.GetByIdAsync(99999);
        }
        else
        {
            await using var context = new LucyReadContext(options);
            var readOnlyRepo = new SequenceReadOnlyRepository(context);
            sequence = await readOnlyRepo.GetByIdAsync(99999);
        }

        // Assert
        Assert.Null(sequence);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByTypeAsync_ShouldReturnSequence_WhenTypeExists(bool useWriteRepo)
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var writeContext = new LucyWriteContext(options);
        var (project, sequences) = await SeedDatabaseAsync(writeContext);

        Sequence? sequence;

        // Act
        if (useWriteRepo)
        {
            var repository = new SequenceRepository(writeContext);
            sequence = await repository.GetByTypeAsync(project.Id, SequenceType.Ticket);
        }
        else
        {
            await using var readContext = new LucyReadContext(options);
            var readOnlyRepo = new SequenceReadOnlyRepository(readContext);
            sequence = await readOnlyRepo.GetByTypeAsync(project.Id, SequenceType.Ticket);
        }

        // Assert
        Assert.NotNull(sequence);
        Assert.Equal(SequenceType.Ticket, sequence.Type);
        Assert.Equal(project.Id, sequence.ProjectId);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByTypeAsync_ShouldReturnNull_WhenTypeDoesNotExist(bool useWriteRepo)
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var writeContext = new LucyWriteContext(options);
        var (project, sequences) = await SeedDatabaseAsync(writeContext);

        Sequence? sequence;

        // Act
        if (useWriteRepo)
        {
            var repository = new SequenceRepository(writeContext);
            sequence = await repository.GetByTypeAsync(project.Id, SequenceType.None);
        }
        else
        {
            await using var readContext = new LucyReadContext(options);
            var readOnlyRepo = new SequenceReadOnlyRepository(readContext);
            sequence = await readOnlyRepo.GetByTypeAsync(project.Id, SequenceType.None);
        }

        // Assert
        Assert.Null(sequence);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExistsByIdAsync_ShouldReturnTrue_WhenIdExists(bool useWriteRepo)
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var writeContext = new LucyWriteContext(options);
        var (project, sequences) = await SeedDatabaseAsync(writeContext);

        bool exists;

        // Act
        if (useWriteRepo)
        {
            var repository = new SequenceRepository(writeContext);
            exists = await repository.ExistsByIdAsync(sequences[0].Id);
        }
        else
        {
            await using var readContext = new LucyReadContext(options);
            var readOnlyRepo = new SequenceReadOnlyRepository(readContext);
            exists = await readOnlyRepo.ExistsByIdAsync(sequences[0].Id);
        }

        // Assert
        Assert.True(exists);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExistsByIdAsync_ShouldReturnFalse_WhenIdDoesNotExist(bool useWriteRepo)
    {
        // Arrange
        var options = CreateDbContextOptions();
        bool exists;

        // Act
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(options);
            var repository = new SequenceRepository(context);
            exists = await repository.ExistsByIdAsync(99999);
        }
        else
        {
            await using var context = new LucyReadContext(options);
            var readOnlyRepo = new SequenceReadOnlyRepository(context);
            exists = await readOnlyRepo.ExistsByIdAsync(99999);
        }

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task Next_ShouldIncrementSequenceValue()
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var context = new LucyWriteContext(options);
        var repository = new SequenceRepository(context);
        var (project, sequences) = await SeedDatabaseAsync(context);

        var sequence = sequences.First(s => s.Type == SequenceType.Ticket);
        var originalValue = sequence.Value;

        // Act
        var nextValue = sequence.Next();
        repository.Update(sequence);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(originalValue + 1, sequence.Value);
        Assert.Contains((originalValue + 1).ToString(), nextValue);
    }

    [Fact]
    public async Task PreviewNext_ShouldNotModifySequenceValue()
    {
        // Arrange
        var options = CreateDbContextOptions();
        await using var context = new LucyWriteContext(options);
        var (project, sequences) = await SeedDatabaseAsync(context);

        var sequence = sequences.First(s => s.Type == SequenceType.Ticket);
        var originalValue = sequence.Value;

        // Act
        var previewValue = sequence.PreviewNext();

        // Assert
        Assert.Equal(originalValue, sequence.Value); // Value should not change
        Assert.Contains((originalValue + 1).ToString(), previewValue);
    }
}
