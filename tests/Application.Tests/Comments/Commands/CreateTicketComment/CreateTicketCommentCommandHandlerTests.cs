using Lucy.Application.Comments.Commands.CreateTicketComment;
using Lucy.Application.Comments.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Comments.Commands.CreateTicketComment;

public class CreateTicketCommentCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<ICommentRepository> _commentRepositoryMock;
    private readonly CreateTicketCommentCommandHandler _handler;

    public CreateTicketCommentCommandHandlerTests()
    {
        _commentRepositoryMock = SetupRepository(u => u.Comments);
        _handler = new CreateTicketCommentCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateComment_WhenValidCommandIsGiven()
    {
        // Arrange
        _commentRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<TicketComment>(), It.IsAny<CancellationToken>()))
            .Callback<Comment, CancellationToken>((comment, _) => comment.Id = 1)
            .Returns(Task.CompletedTask);

        var command = new CreateTicketCommentCommand(1, "Test ticket comment");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);

        _commentRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<TicketComment>(c =>
                c.TicketId == 1 &&
                c.Content == "Test ticket comment"
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}
