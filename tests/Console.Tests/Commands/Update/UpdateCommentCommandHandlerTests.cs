using AppUpdateCommentCommand = Lucy.Application.Comments.Commands.UpdateComment.UpdateCommentCommand;
using Lucy.Application.Interfaces;
using Lucy.Console.Commands.Update;
using Lucy.Console.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.Update;

public class UpdateCommentCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UpdateCommentCommandHandler _handler;

    public UpdateCommentCommandHandlerTests()
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

        _handler = new UpdateCommentCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_UpdateComment_ReturnsSuccess()
    {
        // Arrange
        var commentId = 10L;
        var command = new UpdateCommentCommand
        {
            Id = commentId,
            Content = "Updated comment content"
        };

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.UpdatedComment", _console.Output);
        Assert.Contains(commentId.ToString(), _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<AppUpdateCommentCommand>(c =>
                    c.Id == commentId &&
                    c.Content == command.Content),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UpdateCommentWithLongContent_ReturnsSuccess()
    {
        // Arrange
        var commentId = 10L;
        var longContent = new string('A', 1000);
        var command = new UpdateCommentCommand
        {
            Id = commentId,
            Content = longContent
        };

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.UpdatedComment", _console.Output);
        Assert.Contains(commentId.ToString(), _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<AppUpdateCommentCommand>(c =>
                    c.Id == commentId &&
                    c.Content == command.Content),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UpdateCommentWithMinimalContent_ReturnsSuccess()
    {
        // Arrange
        var commentId = 10L;
        var command = new UpdateCommentCommand
        {
            Id = commentId,
            Content = "A"
        };

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.UpdatedComment", _console.Output);
        Assert.Contains(commentId.ToString(), _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<AppUpdateCommentCommand>(c =>
                    c.Id == commentId &&
                    c.Content == command.Content),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
