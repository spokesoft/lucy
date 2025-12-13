using Lucy.Application.Comments.Commands.CreateProjectComment;
using Lucy.Application.Comments.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Comments.Commands.CreateProjectComment;

public class CreateProjectCommentCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly CreateProjectCommentCommandHandler _handler;

    public CreateProjectCommentCommandHandlerTests()
    {
        _commentRepositoryMock = SetupRepository(u => u.Comments);
        _handler = new CreateProjectCommentCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateComment_WhenValidCommandIsGiven()
    {
        // Arrange
        _commentRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<ProjectComment>(), It.IsAny<CancellationToken>()))
            .Callback<Comment, CancellationToken>((comment, _) => comment.Id = 1)
            .Returns(Task.CompletedTask);

        var command = new CreateProjectCommentCommand(1, "Test comment content");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);

        _commentRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<ProjectComment>(c =>
                c.ProjectId == 1 &&
                c.Content == "Test comment content"
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}
