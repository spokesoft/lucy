using Lucy.Application.Comments.DTOs;
using Lucy.Application.Comments.Queries.GetCommentById;
using Lucy.Application.Comments.Queries.ListProjectComments;
using Lucy.Application.Comments.Queries.ListTicketComments;
using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;
using Moq;

namespace Lucy.Application.Tests.Comments;

public class CommentQueryTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;

    public CommentQueryTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
    }

    #region GetCommentById Tests

    [Fact]
    public async Task GetCommentByIdQueryHandler_ShouldReturnProjectComment_WhenProjectCommentExists()
    {
        // Arrange
        var comment = new ProjectComment(1, "Test project comment");
        comment.Id = 1;

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Comments.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var handler = new GetCommentByIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetCommentByIdQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ProjectCommentDto>(result);
        var projectComment = result as ProjectCommentDto;
        Assert.Equal(1, projectComment!.Id);
        Assert.Equal("Test project comment", projectComment.Content);
        Assert.Equal(1, projectComment.ProjectId);
    }

    [Fact]
    public async Task GetCommentByIdQueryHandler_ShouldReturnTicketComment_WhenTicketCommentExists()
    {
        // Arrange
        var comment = new TicketComment(1, "Test ticket comment");
        comment.Id = 1;

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Comments.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var handler = new GetCommentByIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetCommentByIdQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<TicketCommentDto>(result);
        var ticketComment = result as TicketCommentDto;
        Assert.Equal(1, ticketComment!.Id);
        Assert.Equal("Test ticket comment", ticketComment.Content);
        Assert.Equal(1, ticketComment.TicketId);
    }

    [Fact]
    public async Task GetCommentByIdQueryHandler_ShouldReturnNull_WhenCommentDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Comments.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment)null!);

        var handler = new GetCommentByIdQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new GetCommentByIdQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region ListProjectComments Tests

    [Fact]
    public async Task ListProjectCommentsQueryHandler_ShouldReturnAllProjectComments()
    {
        // Arrange
        var comments = new List<ProjectCommentDto>
        {
            new ProjectCommentDto
            {
                Id = 1,
                ProjectId = 1,
                Content = "First comment",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ProjectCommentDto
            {
                Id = 2,
                ProjectId = 1,
                Content = "Second comment",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ProjectCommentDto
            {
                Id = 3,
                ProjectId = 1,
                Content = "Third comment",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Comments.GetProjectCommentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comments);

        var handler = new ListProjectCommentsQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListProjectCommentsQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("First comment", result[0].Content);
        Assert.Equal("Second comment", result[1].Content);
        Assert.Equal("Third comment", result[2].Content);
        Assert.All(result, c => Assert.Equal(1, c.ProjectId));
    }

    [Fact]
    public async Task ListProjectCommentsQueryHandler_ShouldReturnEmptyList_WhenNoCommentsExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Comments.GetProjectCommentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProjectCommentDto>());

        var handler = new ListProjectCommentsQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListProjectCommentsQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListProjectCommentsQueryValidator_ShouldValidate()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new ListProjectCommentsQueryValidator(_readOnlyUnitOfWorkMock.Object);
        var query = new ListProjectCommentsQuery(1);

        // Act
        var result = await validator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ListProjectCommentsQueryValidator_ShouldInvalidate_WhenProjectDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new ListProjectCommentsQueryValidator(_readOnlyUnitOfWorkMock.Object);
        var query = new ListProjectCommentsQuery(1);

        // Act
        var result = await validator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProjectId");
    }

    #endregion

    #region ListTicketComments Tests

    [Fact]
    public async Task ListTicketCommentsQueryHandler_ShouldReturnAllTicketComments()
    {
        // Arrange
        var comments = new List<TicketCommentDto>
        {
            new TicketCommentDto
            {
                Id = 1,
                TicketId = 1,
                Content = "First ticket comment",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new TicketCommentDto
            {
                Id = 2,
                TicketId = 1,
                Content = "Second ticket comment",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Comments.GetTicketCommentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comments);

        var handler = new ListTicketCommentsQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListTicketCommentsQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("First ticket comment", result[0].Content);
        Assert.Equal("Second ticket comment", result[1].Content);
        Assert.All(result, c => Assert.Equal(1, c.TicketId));
    }

    [Fact]
    public async Task ListTicketCommentsQueryHandler_ShouldReturnEmptyList_WhenNoCommentsExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Comments.GetTicketCommentsAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TicketCommentDto>());

        var handler = new ListTicketCommentsQueryHandler(_readOnlyUnitOfWorkMock.Object);
        var query = new ListTicketCommentsQuery(1);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListTicketCommentsQueryValidator_ShouldValidate()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new ListTicketCommentsQueryValidator(_readOnlyUnitOfWorkMock.Object);
        var query = new ListTicketCommentsQuery(1);

        // Act
        var result = await validator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ListTicketCommentsQueryValidator_ShouldInvalidate_WhenTicketDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new ListTicketCommentsQueryValidator(_readOnlyUnitOfWorkMock.Object);
        var query = new ListTicketCommentsQuery(1);

        // Act
        var result = await validator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TicketId");
    }

    #endregion
}
