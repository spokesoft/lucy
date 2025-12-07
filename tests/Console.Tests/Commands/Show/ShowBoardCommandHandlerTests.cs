using System.Linq;
using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Statuses.Queries.ListStatuses;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.ListTickets;
using Lucy.Console.Commands.Show;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.Show;

public class ShowBoardCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IViewRenderer<(IEnumerable<StatusDto>, Dictionary<long, List<TicketDto>>)>> _viewRendererMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ShowBoardCommandHandler _handler;

    public ShowBoardCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _viewRendererMock = new Mock<IViewRenderer<(IEnumerable<StatusDto>, Dictionary<long, List<TicketDto>>)>>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new ShowBoardCommandHandler(
            _console,
            _localizerMock.Object,
            _viewRendererMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithProjectKey_RendersBoard()
    {
        var command = new ShowBoardCommand { Key = "EX", Id = null };
        var statuses = new List<StatusDto>
        {
            new StatusDto
            {
                Id = 1,
                ProjectId = 10,
                Key = "TODO",
                Order = 1,
                Name = "To Do",
                Description = "",
                Color = Lucy.Domain.Enums.Color.Gray,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var tickets = new List<TicketDto>
        {
            new TicketDto
            {
                Id = 100,
                ProjectId = 10,
                StatusId = 1,
                Key = "EX-1",
                Number = 1,
                Title = "First",
                Description = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.Key), It.IsAny<CancellationToken>()))
            .ReturnsAsync(10L);

        _mediatorMock
            .Setup(m => m.Send(It.Is<ListStatusesQuery>(q => q.ProjectId == 10), It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);

        _mediatorMock
            .Setup(m => m.Send(It.Is<ListTicketsQuery>(q => q.ProjectId == 10), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tickets);

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                It.Is<(IEnumerable<StatusDto>, Dictionary<long, List<TicketDto>>)>(tuple =>
                    tuple.Item1 == statuses &&
                    tuple.Item2.ContainsKey(1) &&
                    tuple.Item2[1].SequenceEqual(tickets)),
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ProjectNotFound_ReturnsError()
    {
        var command = new ShowBoardCommand { Key = "MISSING", Id = null };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.Key), It.IsAny<CancellationToken>()))
            .ReturnsAsync((long?)null);

        _localizerMock
            .Setup(l => l["Error.Project.NotFound"])
            .Returns(new LocalizedString("Error.Project.NotFound", "Project not found"));

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Error, result);
        Assert.Contains("Project not found", _console.Output);

        _viewRendererMock.Verify(
            v => v.RenderAsync(It.IsAny<(IEnumerable<StatusDto>, Dictionary<long, List<TicketDto>>)>(), _console, _localizerMock.Object, It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
