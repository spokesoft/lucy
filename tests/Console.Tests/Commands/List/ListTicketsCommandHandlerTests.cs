using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Statuses.Queries.ListStatuses;
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
    private readonly Mock<IViewRenderer<(IEnumerable<TicketDto>, Dictionary<long, (string Key, string Color)>)>> _viewRendererMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ListTicketsCommandHandler _handler;

    public ListTicketsCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _viewRendererMock = new Mock<IViewRenderer<(IEnumerable<TicketDto>, Dictionary<long, (string Key, string Color)>)>>();
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

        var statuses = new List<StatusDto>
        {
            new() { Id = 1, ProjectId = projectId, Key = "TODO", Name = "To Do", Color = Domain.Enums.StatusColor.Gray, Order = 1, Description = "", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new() { Id = 2, ProjectId = projectId, Key = "DONE", Name = "Done", Color = Domain.Enums.StatusColor.Green, Order = 2, Description = "", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
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

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListStatusesQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                It.Is<(IEnumerable<TicketDto>, Dictionary<long, (string Key, string Color)>)>(t =>
                    t.Item1.Count() == 2 && t.Item2.Count == 2),
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

        var statuses = new List<StatusDto>
        {
            new() { Id = 1, ProjectId = projectId, Key = "TODO", Name = "To Do", Color = Domain.Enums.StatusColor.Gray, Order = 1, Description = "", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListTicketsQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tickets);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListStatusesQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                It.Is<(IEnumerable<TicketDto>, Dictionary<long, (string Key, string Color)>)>(t =>
                    t.Item1.Count() == 1 && t.Item2.Count == 1),
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
        var statuses = new List<StatusDto>();

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

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ListStatusesQuery>(q => q.ProjectId == projectId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                It.Is<(IEnumerable<TicketDto>, Dictionary<long, (string Key, string Color)>)>(t =>
                    t.Item1.Count() == 0 && t.Item2.Count == 0),
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
