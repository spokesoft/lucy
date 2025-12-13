using Lucy.Application.Comments.DTOs;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Lucy.Infrastructure.Database;
using Lucy.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Lucy.Infrastructure.Tests.Repositories;

[Collection("Database collection")]
public class CommentRepositoryTests : RepositoryTestBase
{
    private async Task<(Project project, Ticket ticket)> SeedDatabaseAsync(LucyDbContext context)
    {
        var project = new Project("TEST", "Test Project", "Test Description");
        context.Projects.Add(project);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        // Get the first status for the ticket
        var status = project.Statuses.First();
        var ticket = new Ticket(project.Id, status.Id, "TEST-1", 1, "Test Ticket", "Test ticket description");
        context.Tickets.Add(ticket);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        // Add some comments
        var projectComment1 = new ProjectComment(project.Id, "First project comment");
        var projectComment2 = new ProjectComment(project.Id, "Second project comment");
        var ticketComment1 = new TicketComment(ticket.Id, "First ticket comment");
        var ticketComment2 = new TicketComment(ticket.Id, "Second ticket comment");

        context.Comments.AddRange(projectComment1, projectComment2, ticketComment1, ticketComment2);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        return (project, ticket);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProjectCommentToDatabase()
    {
        // Arrange
        await using var context = new LucyWriteContext(_writeDbContextOptions);
        var (project, _) = await SeedDatabaseAsync(context);
        var repository = new CommentRepository(context);
        var newComment = new ProjectComment(project.Id, "New project comment");

        // Act
        await repository.AddAsync(newComment);
        await context.SaveChangesAsync();

        // Assert
        var commentInDb = await context.Comments
            .OfType<ProjectComment>()
            .FirstOrDefaultAsync(c => c.Content == "New project comment");
        Assert.NotNull(commentInDb);
        Assert.Equal(project.Id, commentInDb.ProjectId);
        Assert.Equal("New project comment", commentInDb.Content);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTicketCommentToDatabase()
    {
        // Arrange
        await using var context = new LucyWriteContext(_writeDbContextOptions);
        var (_, ticket) = await SeedDatabaseAsync(context);
        var repository = new CommentRepository(context);
        var newComment = new TicketComment(ticket.Id, "New ticket comment");

        // Act
        await repository.AddAsync(newComment);
        await context.SaveChangesAsync();

        // Assert
        var commentInDb = await context.Comments
            .OfType<TicketComment>()
            .FirstOrDefaultAsync(c => c.Content == "New ticket comment");
        Assert.NotNull(commentInDb);
        Assert.Equal(ticket.Id, commentInDb.TicketId);
        Assert.Equal("New ticket comment", commentInDb.Content);
    }

    [Fact]
    public async Task Update_ShouldModifyComment()
    {
        // Arrange
        await using var context = new LucyWriteContext(_writeDbContextOptions);
        var (project, _) = await SeedDatabaseAsync(context);
        var repository = new CommentRepository(context);

        var comment = await context.Comments
            .OfType<ProjectComment>()
            .FirstAsync(c => c.Content == "First project comment");
        var commentId = comment.Id;

        // Act
        comment.UpdateContent("Updated project comment");
        repository.Update(comment);
        await context.SaveChangesAsync();

        // Assert
        var updatedComment = await context.Comments.FindAsync(commentId);
        Assert.NotNull(updatedComment);
        Assert.Equal("Updated project comment", updatedComment.Content);
    }

    [Fact]
    public async Task Remove_ShouldDeleteComment()
    {
        // Arrange
        await using var context = new LucyWriteContext(_writeDbContextOptions);
        await SeedDatabaseAsync(context);
        var repository = new CommentRepository(context);

        var comment = await context.Comments
            .OfType<ProjectComment>()
            .FirstAsync(c => c.Content == "First project comment");
        var commentId = comment.Id;

        // Act
        repository.Remove(comment);
        await context.SaveChangesAsync();

        // Assert
        var deletedComment = await context.Comments.FindAsync(commentId);
        Assert.Null(deletedComment);
    }

    [Theory]
    [InlineData(true)]  // Test with CommentRepository
    [InlineData(false)] // Test with CommentReadOnlyRepository
    public async Task GetByIdAsync_ShouldReturnComment_WhenIdExists(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        await SeedDatabaseAsync(writeContext);

        var commentId = await writeContext.Comments
            .OfType<ProjectComment>()
            .Where(c => c.Content == "First project comment")
            .Select(c => c.Id)
            .FirstAsync();

        Comment? comment;

        // Act
        if (useWriteRepo)
        {
            var repository = new CommentRepository(writeContext);
            comment = await repository.GetByIdAsync(commentId);
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new CommentReadOnlyRepository(readContext);
            comment = await repository.GetByIdAsync(commentId);
        }

        // Assert
        Assert.NotNull(comment);
        Assert.Equal(commentId, comment.Id);
        Assert.Equal("First project comment", comment.Content);
        Assert.IsType<ProjectComment>(comment);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist(bool useWriteRepo)
    {
        // Arrange & Act
        Comment? comment;
        if (useWriteRepo)
        {
            await using var context = new LucyWriteContext(_writeDbContextOptions);
            var repository = new CommentRepository(context);
            comment = await repository.GetByIdAsync(999);
        }
        else
        {
            await using var context = new LucyReadContext(_readDbContextOptions);
            var repository = new CommentReadOnlyRepository(context);
            comment = await repository.GetByIdAsync(999);
        }

        // Assert
        Assert.Null(comment);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ExistsByIdAsync_ShouldReturnTrue_WhenIdExists(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        await SeedDatabaseAsync(writeContext);

        var commentId = await writeContext.Comments
            .OfType<ProjectComment>()
            .Where(c => c.Content == "First project comment")
            .Select(c => c.Id)
            .FirstAsync();

        bool exists;

        // Act
        if (useWriteRepo)
        {
            var repository = new CommentRepository(writeContext);
            exists = await repository.ExistsByIdAsync(commentId);
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new CommentReadOnlyRepository(readContext);
            exists = await repository.ExistsByIdAsync(commentId);
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
            var repository = new CommentRepository(context);
            exists = await repository.ExistsByIdAsync(999);
        }
        else
        {
            await using var context = new LucyReadContext(_readDbContextOptions);
            var repository = new CommentReadOnlyRepository(context);
            exists = await repository.ExistsByIdAsync(999);
        }

        // Assert
        Assert.False(exists);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetProjectCommentsAsync_ShouldReturnAllProjectComments(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var (project, _) = await SeedDatabaseAsync(writeContext);

        List<ProjectCommentDto> comments;

        // Act
        if (useWriteRepo)
        {
            var repository = new CommentRepository(writeContext);
            comments = await repository.GetProjectCommentsAsync(project.Id);
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new CommentReadOnlyRepository(readContext);
            comments = await repository.GetProjectCommentsAsync(project.Id);
        }

        // Assert
        Assert.NotNull(comments);
        Assert.Equal(2, comments.Count);
        Assert.All(comments, c => Assert.Equal(CommentType.Project, c.CommentType));
        Assert.All(comments, c => Assert.Equal(project.Id, c.ProjectId));
        Assert.Equal("First project comment", comments[0].Content);
        Assert.Equal("Second project comment", comments[1].Content);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetProjectCommentsAsync_ShouldReturnEmptyList_WhenNoCommentsExist(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var project = new Project("EMPTY", "Empty Project", null);
        writeContext.Projects.Add(project);
        await writeContext.SaveChangesAsync();
        writeContext.ChangeTracker.Clear();

        List<ProjectCommentDto> comments;

        // Act
        if (useWriteRepo)
        {
            var repository = new CommentRepository(writeContext);
            comments = await repository.GetProjectCommentsAsync(project.Id);
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new CommentReadOnlyRepository(readContext);
            comments = await repository.GetProjectCommentsAsync(project.Id);
        }

        // Assert
        Assert.NotNull(comments);
        Assert.Empty(comments);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetTicketCommentsAsync_ShouldReturnAllTicketComments(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var (_, ticket) = await SeedDatabaseAsync(writeContext);

        List<TicketCommentDto> comments;

        // Act
        if (useWriteRepo)
        {
            var repository = new CommentRepository(writeContext);
            comments = await repository.GetTicketCommentsAsync(ticket.Id);
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new CommentReadOnlyRepository(readContext);
            comments = await repository.GetTicketCommentsAsync(ticket.Id);
        }

        // Assert
        Assert.NotNull(comments);
        Assert.Equal(2, comments.Count);
        Assert.All(comments, c => Assert.Equal(CommentType.Ticket, c.CommentType));
        Assert.All(comments, c => Assert.Equal(ticket.Id, c.TicketId));
        Assert.Equal("First ticket comment", comments[0].Content);
        Assert.Equal("Second ticket comment", comments[1].Content);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetTicketCommentsAsync_ShouldReturnEmptyList_WhenNoCommentsExist(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var project = new Project("TEST2", "Test Project 2", null);
        writeContext.Projects.Add(project);
        await writeContext.SaveChangesAsync();

        var status = project.Statuses.First();
        var ticket = new Ticket(project.Id, status.Id, "TEST2-1", 2, "Empty Ticket", null);
        writeContext.Tickets.Add(ticket);
        await writeContext.SaveChangesAsync();
        writeContext.ChangeTracker.Clear();

        List<TicketCommentDto> comments;

        // Act
        if (useWriteRepo)
        {
            var repository = new CommentRepository(writeContext);
            comments = await repository.GetTicketCommentsAsync(ticket.Id);
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new CommentReadOnlyRepository(readContext);
            comments = await repository.GetTicketCommentsAsync(ticket.Id);
        }

        // Assert
        Assert.NotNull(comments);
        Assert.Empty(comments);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetProjectCommentsAsync_ShouldOrderByCreatedAt(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var project = new Project("ORDER", "Order Project", null);
        writeContext.Projects.Add(project);
        await writeContext.SaveChangesAsync();

        // Add comments with different timestamps
        var comment1 = new ProjectComment(project.Id, "Third created");
        await Task.Delay(10); // Ensure different timestamps
        var comment2 = new ProjectComment(project.Id, "First created");
        await Task.Delay(10);
        var comment3 = new ProjectComment(project.Id, "Second created");

        writeContext.Comments.AddRange(comment1, comment2, comment3);
        await writeContext.SaveChangesAsync();
        writeContext.ChangeTracker.Clear();

        List<ProjectCommentDto> comments;

        // Act
        if (useWriteRepo)
        {
            var repository = new CommentRepository(writeContext);
            comments = await repository.GetProjectCommentsAsync(project.Id);
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new CommentReadOnlyRepository(readContext);
            comments = await repository.GetProjectCommentsAsync(project.Id);
        }

        // Assert
        Assert.Equal(3, comments.Count);
        Assert.True(comments[0].CreatedAt <= comments[1].CreatedAt);
        Assert.True(comments[1].CreatedAt <= comments[2].CreatedAt);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task GetTicketCommentsAsync_ShouldOrderByCreatedAt(bool useWriteRepo)
    {
        // Arrange
        await using var writeContext = new LucyWriteContext(_writeDbContextOptions);
        var project = new Project("ORDER2", "Order Project 2", null);
        writeContext.Projects.Add(project);
        await writeContext.SaveChangesAsync();

        var status = project.Statuses.First();
        var ticket = new Ticket(project.Id, status.Id, "ORDER2-1", 3, "Order Ticket", null);
        writeContext.Tickets.Add(ticket);
        await writeContext.SaveChangesAsync();

        // Add comments with different timestamps
        var comment1 = new TicketComment(ticket.Id, "Third created");
        await Task.Delay(10); // Ensure different timestamps
        var comment2 = new TicketComment(ticket.Id, "First created");
        await Task.Delay(10);
        var comment3 = new TicketComment(ticket.Id, "Second created");

        writeContext.Comments.AddRange(comment1, comment2, comment3);
        await writeContext.SaveChangesAsync();
        writeContext.ChangeTracker.Clear();

        List<TicketCommentDto> comments;

        // Act
        if (useWriteRepo)
        {
            var repository = new CommentRepository(writeContext);
            comments = await repository.GetTicketCommentsAsync(ticket.Id);
        }
        else
        {
            await using var readContext = new LucyReadContext(_readDbContextOptions);
            var repository = new CommentReadOnlyRepository(readContext);
            comments = await repository.GetTicketCommentsAsync(ticket.Id);
        }

        // Assert
        Assert.Equal(3, comments.Count);
        Assert.True(comments[0].CreatedAt <= comments[1].CreatedAt);
        Assert.True(comments[1].CreatedAt <= comments[2].CreatedAt);
    }
}
