using Lucy.Application.Comments.DTOs;
using Lucy.Application.Comments.Queries.ListProjectComments;
using Lucy.Application.Comments.Queries.ListTicketComments;
using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Console.Commands.List;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.List;

public class ListCommentsCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IViewRenderer<IEnumerable<CommentDto>>> _viewRendererMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ListCommentsCommandHandler _handler;

    public ListCommentsCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _viewRendererMock = new Mock<IViewRenderer<IEnumerable<CommentDto>>>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new ListCommentsCommandHandler(
            _console,
            _localizerMock.Object,
            _viewRendererMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ListProjectCommentsByKey_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new ListCommentsCommand { Key = "TEST" };
        var comments = new List<ProjectCommentDto>
        {
            new()
            {
                Id = 1,
                Content = "First comment",
                ProjectId = projectId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = 2,
                Content = "Second comment",
                ProjectId = projectId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
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
                It.Is<ListProjectCommentsQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(comments);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _viewRendererMock.Verify(
            v => v.RenderAsync(
                comments,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ListTicketCommentsByKey_ReturnsSuccess()
    {
        // Arrange
        var ticketId = 5L;
        var command = new ListCommentsCommand { Key = "TEST-1" };
        var ticketDto = new TicketDto
        {
            Id = ticketId,
            Key = "TEST-1",
            Title = "Test Ticket"
        };
        var comments = new List<TicketCommentDto>
        {
            new()
            {
                Id = 1,
                Content = "First ticket comment",
                TicketId = ticketId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetTicketByKeyQuery>(q => q.Key == command.Key),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticketDto);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListTicketCommentsQuery>(q => q.TicketId == ticketId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(comments);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _viewRendererMock.Verify(
            v => v.RenderAsync(
                comments,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ListProjectCommentsByProjectId_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new ListCommentsCommand { ProjectId = projectId };
        var comments = new List<ProjectCommentDto>
        {
            new()
            {
                Id = 1,
                Content = "Project comment",
                ProjectId = projectId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListProjectCommentsQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(comments);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _viewRendererMock.Verify(
            v => v.RenderAsync(
                comments,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ListTicketCommentsByTicketId_ReturnsSuccess()
    {
        // Arrange
        var ticketId = 5L;
        var command = new ListCommentsCommand { TicketId = ticketId };
        var comments = new List<TicketCommentDto>
        {
            new()
            {
                Id = 1,
                Content = "Ticket comment",
                TicketId = ticketId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListTicketCommentsQuery>(q => q.TicketId == ticketId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(comments);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _viewRendererMock.Verify(
            v => v.RenderAsync(
                comments,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NoCommentsFound_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new ListCommentsCommand { ProjectId = projectId };
        var comments = new List<ProjectCommentDto>();

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListProjectCommentsQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(comments);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _viewRendererMock.Verify(
            v => v.RenderAsync(
                comments,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NoIdentifierProvided_ThrowsException()
    {
        // Arrange
        var command = new ListCommentsCommand();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(null!, command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_KeyNotFound_ThrowsException()
    {
        // Arrange
        var command = new ListCommentsCommand { Key = "NOTFOUND" };

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
}
