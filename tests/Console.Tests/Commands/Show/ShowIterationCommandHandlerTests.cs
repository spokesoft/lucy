using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.DTOs;
using Lucy.Application.Iterations.Queries.GetIterationById;
using Lucy.Application.Iterations.Queries.GetIterationIdByKey;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.GetTicketCountsByIterationId;
using Lucy.Console.Commands.Show;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.Show;

public class ShowIterationCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IViewRenderer<(IterationDto, IEnumerable<TicketCountByStatusDto>)>> _viewRendererMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ShowIterationCommandHandler _handler;

    public ShowIterationCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _viewRendererMock = new Mock<IViewRenderer<(IterationDto, IEnumerable<TicketCountByStatusDto>)>>();
        _mediatorMock = new Mock<IMediator>();

        _localizerMock
            .Setup(x => x[It.IsAny<string>()])
            .Returns((string key) => new LocalizedString(key, key));

        _handler = new ShowIterationCommandHandler(
            _console,
            _localizerMock.Object,
            _viewRendererMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithId_ReturnsSuccess()
    {
        // Arrange
        var command = new ShowIterationCommand { Id = 1, Key = null };
        var iteration = new IterationDto
        {
            Id = 1,
            Name = "Iteration 1",
            Description = "Description",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(14),
            ProjectId = 1,
            Key = "ITER-1"
        };
        var ticketCounts = new List<TicketCountByStatusDto>();

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetIterationByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(iteration);

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetTicketCountsByIterationIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticketCounts);

        // Act
        var result = await _handler.HandleAsync(null!, command);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _viewRendererMock.Verify(x => x.RenderAsync(
            It.Is<(IterationDto, IEnumerable<TicketCountByStatusDto>)>(t => t.Item1 == iteration && t.Item2 == ticketCounts),
            _console,
            _localizerMock.Object,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithKey_ReturnsSuccess()
    {
        // Arrange
        var command = new ShowIterationCommand { Key = "ITER-1", Id = null };
        var iteration = new IterationDto
        {
            Id = 1,
            Name = "Iteration 1",
            Description = "Description",
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(14),
            ProjectId = 1,
            Key = "ITER-1"
        };
        var ticketCounts = new List<TicketCountByStatusDto>();

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetIterationIdByKeyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetIterationByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(iteration);

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetTicketCountsByIterationIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticketCounts);

        // Act
        var result = await _handler.HandleAsync(null!, command);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _viewRendererMock.Verify(x => x.RenderAsync(
            It.Is<(IterationDto, IEnumerable<TicketCountByStatusDto>)>(t => t.Item1 == iteration && t.Item2 == ticketCounts),
            _console,
            _localizerMock.Object,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NotFound_ReturnsError()
    {
        // Arrange
        var command = new ShowIterationCommand { Id = 1, Key = null };

        _mediatorMock
            .Setup(x => x.Send(It.IsAny<GetIterationByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IterationDto?)null);

        // Act
        var result = await _handler.HandleAsync(null!, command);

        // Assert
        Assert.Equal(ExitCode.Error, result);
        Assert.Contains("Error.Iteration.NotFound", _console.Output);
    }
}
