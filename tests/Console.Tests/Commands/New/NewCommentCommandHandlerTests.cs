using Lucy.Application.Comments.Commands.CreateProjectComment;
using Lucy.Application.Comments.Commands.CreateTicketComment;
using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Console.Commands.New;
using Lucy.Console.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.New;

public class NewCommentCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly NewCommentCommandHandler _handler;

    public NewCommentCommandHandlerTests()
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

        _handler = new NewCommentCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_CreateProjectCommentByKey_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var commentId = 10L;
        var command = new NewCommentCommand
        {
            Key = "TEST",
            Content = "Test project comment"
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetTicketByKeyQuery>(q => q.Key == command.Key),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TicketDto?)null);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.Key),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateProjectCommentCommand>(c =>
                    c.ProjectId == projectId &&
                    c.Content == command.Content),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(commentId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.CreatedProjectComment", _console.Output);
        Assert.Contains(commentId.ToString(), _console.Output);
    }

    [Fact]
    public async Task HandleAsync_CreateTicketCommentByKey_ReturnsSuccess()
    {
        // Arrange
        var ticketId = 5L;
        var commentId = 15L;
        var command = new NewCommentCommand
        {
            Key = "TEST-1",
            Content = "Test ticket comment"
        };

        var ticketDto = new TicketDto
        {
            Id = ticketId,
            Key = "TEST-1",
            Title = "Test Ticket"
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetTicketByKeyQuery>(q => q.Key == command.Key),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticketDto);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateTicketCommentCommand>(c =>
                    c.TicketId == ticketId &&
                    c.Content == command.Content),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(commentId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.CreatedTicketComment", _console.Output);
        Assert.Contains(commentId.ToString(), _console.Output);
    }

    [Fact]
    public async Task HandleAsync_CreateProjectCommentByProjectId_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var commentId = 10L;
        var command = new NewCommentCommand
        {
            ProjectId = projectId,
            Content = "Test project comment"
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateProjectCommentCommand>(c =>
                    c.ProjectId == projectId &&
                    c.Content == command.Content),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(commentId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.CreatedProjectComment", _console.Output);
        Assert.Contains(commentId.ToString(), _console.Output);
    }

    [Fact]
    public async Task HandleAsync_CreateTicketCommentByTicketId_ReturnsSuccess()
    {
        // Arrange
        var ticketId = 5L;
        var commentId = 15L;
        var command = new NewCommentCommand
        {
            TicketId = ticketId,
            Content = "Test ticket comment"
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateTicketCommentCommand>(c =>
                    c.TicketId == ticketId &&
                    c.Content == command.Content),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(commentId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.CreatedTicketComment", _console.Output);
        Assert.Contains(commentId.ToString(), _console.Output);
    }

    [Fact]
    public async Task HandleAsync_NoIdentifierProvided_ThrowsException()
    {
        // Arrange
        var command = new NewCommentCommand
        {
            Content = "Test comment"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(null!, command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_KeyNotFound_ThrowsException()
    {
        // Arrange
        var command = new NewCommentCommand
        {
            Key = "NOTFOUND",
            Content = "Test comment"
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetTicketByKeyQuery>(q => q.Key == command.Key),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((TicketDto?)null);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.Key),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((long?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(null!, command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_EmptyContent_ThrowsException()
    {
        // Arrange
        var command = new NewCommentCommand
        {
            Key = "TEST",
            Content = ""
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(null!, command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_NullContent_ThrowsException()
    {
        // Arrange
        var command = new NewCommentCommand
        {
            Key = "TEST",
            Content = null!
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(null!, command, CancellationToken.None));
    }
}
