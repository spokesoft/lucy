using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.ListTickets;
using Lucy.Console.Commands.List;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.List;

public class ListTicketsCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IViewRenderer<IEnumerable<TicketDto>>> _viewRendererMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ListTicketsCommandHandler _handler;

    public ListTicketsCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _viewRendererMock = new Mock<IViewRenderer<IEnumerable<TicketDto>>>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new ListTicketsCommandHandler(
            _console,
            _localizerMock.Object,
            _viewRendererMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithProjectKey_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new ListTicketsCommand
        {
            Key = "ABC",
            Id = null
        };
        var tickets = new List<TicketDto>
        {
            new() { Id = 1, ProjectId = projectId, StatusId = 1, Key = "ABC-1", Title = "Test 1", Description = "Desc 1", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 2, ProjectId = projectId, StatusId = 2, Key = "ABC-2", Title = "Test 2", Description = "Desc 2", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.Key),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListTicketsQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tickets);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                tickets,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithProjectId_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new ListTicketsCommand
        {
            Key = null,
            Id = projectId
        };
        var tickets = new List<TicketDto>
        {
            new() { Id = 1, ProjectId = projectId, StatusId = 1, Key = "ABC-1", Title = "Test 1", Description = null, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListTicketsQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tickets);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                tickets,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_EmptyList_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new ListTicketsCommand
        {
            Key = "ABC",
            Id = null
        };
        var tickets = new List<TicketDto>();

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.Key),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListTicketsQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tickets);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                tickets,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
