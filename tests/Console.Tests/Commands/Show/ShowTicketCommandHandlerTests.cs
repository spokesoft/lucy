using Lucy.Application.Interfaces;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.GetTicketById;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Console.Commands.Show;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.Show;

public class ShowTicketCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IViewRenderer<TicketDto>> _viewRendererMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ShowTicketCommandHandler _handler;

    public ShowTicketCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _viewRendererMock = new Mock<IViewRenderer<TicketDto>>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new ShowTicketCommandHandler(
            _console,
            _localizerMock.Object,
            _viewRendererMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidId_ReturnsSuccess()
    {
        // Arrange
        var ticketId = 1L;
        var command = new ShowTicketCommand
        {
            Id = ticketId,
            Key = null
        };
        var ticket = new TicketDto
        {
            Id = ticketId,
            ProjectId = 1L,
            StatusId = 1L,
            Key = "ABC-1",
            Title = "Test ticket",
            Description = "Test description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTicketByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                ticket,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithValidKey_ReturnsSuccess()
    {
        // Arrange
        var ticketKey = "ABC-1";
        var command = new ShowTicketCommand
        {
            Id = null,
            Key = ticketKey
        };
        var ticket = new TicketDto
        {
            Id = 1L,
            ProjectId = 1L,
            StatusId = 1L,
            Key = ticketKey,
            Title = "Test ticket",
            Description = "Test description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTicketByKeyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        _viewRendererMock.Verify(
            v => v.RenderAsync(
                ticket,
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_TicketNotFoundById_ReturnsError()
    {
        // Arrange
        var ticketId = 1L;
        var command = new ShowTicketCommand
        {
            Id = ticketId,
            Key = null
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTicketByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TicketDto?)null);

        _localizerMock
            .Setup(l => l["Error.Ticket.NotFound"])
            .Returns(new LocalizedString("Error.Ticket.NotFound", "Ticket not found"));

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Error, result);
        Assert.Contains("Ticket not found", _console.Output);
    }

    [Fact]
    public async Task HandleAsync_TicketNotFoundByKey_ReturnsError()
    {
        // Arrange
        var ticketKey = "ABC-99";
        var command = new ShowTicketCommand
        {
            Id = null,
            Key = ticketKey
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTicketByKeyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TicketDto?)null);

        _localizerMock
            .Setup(l => l["Error.Ticket.NotFound"])
            .Returns(new LocalizedString("Error.Ticket.NotFound", "Ticket not found"));

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Error, result);
        Assert.Contains("Ticket not found", _console.Output);
    }
}
