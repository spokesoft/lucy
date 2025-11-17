using AppDeleteCommentCommand = Lucy.Application.Comments.Commands.DeleteComment.DeleteCommentCommand;
using Lucy.Application.Interfaces;
using Lucy.Console.Commands.Delete;
using Lucy.Console.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.Delete;

public class DeleteCommentCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DeleteCommentCommandHandler _handler;

    public DeleteCommentCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _mediatorMock = new Mock<IMediator>();

        // Setup localizer to return formatted string with parameters
        _localizerMock
            .Setup(x => x[It.IsAny<string>(), It.IsAny<object[]>()])
            .Returns((string key, object[] args) =>
            {
                var formatted = args.Length > 0 ? $"{key} {string.Join(" ", args)}" : key;
                return new LocalizedString(key, formatted);
            });

        _handler = new DeleteCommentCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_DeleteComment_ReturnsSuccess()
    {
        // Arrange
        var commentId = 10L;
        var command = new DeleteCommentCommand
        {
            Id = commentId
        };

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.DeletedComment", _console.Output);
        Assert.Contains(commentId.ToString(), _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<AppDeleteCommentCommand>(c => c.Id == commentId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_DeleteCommentWithDifferentId_ReturnsSuccess()
    {
        // Arrange
        var commentId = 999L;
        var command = new DeleteCommentCommand
        {
            Id = commentId
        };

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.DeletedComment", _console.Output);
        Assert.Contains(commentId.ToString(), _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<AppDeleteCommentCommand>(c => c.Id == commentId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
