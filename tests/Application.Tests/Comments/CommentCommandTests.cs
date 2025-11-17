using Lucy.Application.Comments.Commands.CreateProjectComment;
using Lucy.Application.Comments.Commands.CreateTicketComment;
using Lucy.Application.Comments.Commands.DeleteComment;
using Lucy.Application.Comments.Commands.UpdateComment;
using Lucy.Application.Comments.Repositories;
using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;
using Moq;

namespace Lucy.Application.Tests.Comments;

public class CommentCommandTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<ICommentRepository> _commentRepositoryMock;

    public CommentCommandTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _commentRepositoryMock = new Mock<ICommentRepository>();
        _unitOfWorkMock.Setup(u => u.Comments).Returns(_commentRepositoryMock.Object);
    }

    #region CreateProjectComment Tests

    [Fact]
    public async Task CreateProjectCommentCommandHandler_ShouldCreateComment_WhenValidCommandIsGiven()
    {
        // Arrange
        _commentRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<ProjectComment>(), It.IsAny<CancellationToken>()))
            .Callback<Comment, CancellationToken>((comment, _) => comment.Id = 1)
            .Returns(Task.CompletedTask);

        var handler = new CreateProjectCommentCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateProjectCommentCommand(1, "Test comment content");

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);

        _commentRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<ProjectComment>(c =>
                c.ProjectId == 1 &&
                c.Content == "Test comment content"
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateProjectCommentCommandValidator_ShouldValidate()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new CreateProjectCommentCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateProjectCommentCommand(1, "Valid comment content");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreateProjectCommentCommandValidator_ShouldInvalidate_WhenProjectDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new CreateProjectCommentCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateProjectCommentCommand(1, "Valid comment content");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProjectId");
    }

    [Fact]
    public async Task CreateProjectCommentCommandValidator_ShouldInvalidate_WhenContentIsEmpty()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new CreateProjectCommentCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateProjectCommentCommand(1, string.Empty);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Content");
    }

    [Fact]
    public async Task CreateProjectCommentCommandValidator_ShouldInvalidate_WhenContentIsTooLong()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new CreateProjectCommentCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateProjectCommentCommand(1, new string('A', 5001));

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Content");
    }

    #endregion

    #region CreateTicketComment Tests

    [Fact]
    public async Task CreateTicketCommentCommandHandler_ShouldCreateComment_WhenValidCommandIsGiven()
    {
        // Arrange
        _commentRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<TicketComment>(), It.IsAny<CancellationToken>()))
            .Callback<Comment, CancellationToken>((comment, _) => comment.Id = 1)
            .Returns(Task.CompletedTask);

        var handler = new CreateTicketCommentCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateTicketCommentCommand(1, "Test ticket comment");

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);

        _commentRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<TicketComment>(c =>
                c.TicketId == 1 &&
                c.Content == "Test ticket comment"
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateTicketCommentCommandValidator_ShouldValidate()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new CreateTicketCommentCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateTicketCommentCommand(1, "Valid ticket comment");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreateTicketCommentCommandValidator_ShouldInvalidate_WhenTicketDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new CreateTicketCommentCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateTicketCommentCommand(1, "Valid ticket comment");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TicketId");
    }

    [Fact]
    public async Task CreateTicketCommentCommandValidator_ShouldInvalidate_WhenContentIsEmpty()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new CreateTicketCommentCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateTicketCommentCommand(1, string.Empty);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Content");
    }

    #endregion

    #region UpdateComment Tests

    [Fact]
    public async Task UpdateCommentCommandHandler_ShouldUpdateComment_WhenValidCommandIsGiven()
    {
        // Arrange
        var comment = new ProjectComment(1, "Original content");
        comment.Id = 1;

        _commentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var handler = new UpdateCommentCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateCommentCommand(1, "Updated content");

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal("Updated content", comment.Content);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateCommentCommandHandler_ShouldThrowException_WhenCommentDoesNotExist()
    {
        // Arrange
        _commentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment)null!);

        var handler = new UpdateCommentCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateCommentCommand(1, "Updated content");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateCommentCommandValidator_ShouldValidate()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Comments.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new UpdateCommentCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateCommentCommand(1, "Valid updated content");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UpdateCommentCommandValidator_ShouldInvalidate_WhenCommentDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Comments.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new UpdateCommentCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateCommentCommand(1, "Valid updated content");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task UpdateCommentCommandValidator_ShouldInvalidate_WhenContentIsEmpty()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Comments.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new UpdateCommentCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateCommentCommand(1, string.Empty);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Content");
    }

    [Fact]
    public async Task UpdateCommentCommandValidator_ShouldInvalidate_WhenContentIsTooLong()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Comments.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new UpdateCommentCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateCommentCommand(1, new string('A', 5001));

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Content");
    }

    #endregion

    #region DeleteComment Tests

    [Fact]
    public async Task DeleteCommentCommandHandler_ShouldDeleteComment_WhenCommentExists()
    {
        // Arrange
        var comment = new ProjectComment(1, "Comment to delete");
        comment.Id = 1;

        _commentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var handler = new DeleteCommentCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteCommentCommand(1);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _commentRepositoryMock.Verify(repo => repo.Remove(comment), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteCommentCommandHandler_ShouldThrowException_WhenCommentDoesNotExist()
    {
        // Arrange
        _commentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Comment)null!);

        var handler = new DeleteCommentCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteCommentCommand(1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCommentCommandValidator_ShouldValidate()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Comments.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new DeleteCommentCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new DeleteCommentCommand(1);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task DeleteCommentCommandValidator_ShouldInvalidate_WhenCommentDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Comments.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new DeleteCommentCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new DeleteCommentCommand(1);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    #endregion
}
