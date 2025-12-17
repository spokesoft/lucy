using Lucy.Application.Common.Interfaces;
using Lucy.Application.Iterations.Repositories;
using Lucy.Application.Tickets.Commands.AssignTicketToIteration;
using Lucy.Application.Tickets.Repositories;
using Lucy.Console.Commands.Add;
using Lucy.Console.Enums;
using Lucy.Domain.Entities;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.Add;

public class AddTicketCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ITicketRepository> _ticketRepoMock;
    private readonly Mock<IIterationRepository> _iterationRepoMock;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly AddTicketCommandHandler _handler;

    public AddTicketCommandHandlerTests()
    {
        _console = new TestConsole();
        _mediatorMock = new Mock<IMediator>();
        _uowMock = new Mock<IUnitOfWork>();
        _ticketRepoMock = new Mock<ITicketRepository>();
        _iterationRepoMock = new Mock<IIterationRepository>();
        _localizerMock = new Mock<IStringLocalizer<Program>>();

        _uowMock.Setup(u => u.Tickets).Returns(_ticketRepoMock.Object);
        _uowMock.Setup(u => u.Iterations).Returns(_iterationRepoMock.Object);

        _localizerMock
            .Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()])
            .Returns((string key, object[] args) => new LocalizedString(key, $"{key} {string.Join(' ', args)}"));

        _handler = new AddTicketCommandHandler(
            _mediatorMock.Object,
            _uowMock.Object,
            _localizerMock.Object,
            _console);
    }

    [Fact]
    public async Task HandleAsync_WithKeys_AssignsTicketToIteration()
    {
        // Arrange
        var ticket = new Ticket(1, 1, "PROJ-10", 10, "Title") { Id = 10 };
        var iteration = new Iteration(1, "ITER-1", 1, "Name", "Desc", null, null) { Id = 5 };

        _ticketRepoMock.Setup(r => r.GetByKeyAsync("PROJ-10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);
        _iterationRepoMock.Setup(r => r.GetByKeyAsync("ITER-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(iteration);

        var command = new AddTicketCommand
        {
            TicketKey = "PROJ-10",
            IterationKey = "ITER-1"
        };

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _mediatorMock.Verify(m => m.Send(
            It.Is<AssignTicketToIterationCommand>(c => c.TicketId == 10 && c.IterationId == 5),
            It.IsAny<CancellationToken>()), Times.Once);

        Assert.Contains("Messages.AddedTicketToIteration", _console.Output);
    }

    [Fact]
    public async Task HandleAsync_WithLowerCaseKeys_AssignsTicketToIteration()
    {
        // Arrange
        var ticket = new Ticket(1, 1, "PROJ-10", 10, "Title") { Id = 10 };
        var iteration = new Iteration(1, "ITER-1", 1, "Name", "Desc", null, null) { Id = 5 };

        _ticketRepoMock.Setup(r => r.GetByKeyAsync("PROJ-10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);
        _iterationRepoMock.Setup(r => r.GetByKeyAsync("ITER-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(iteration);

        var command = new AddTicketCommand
        {
            TicketKey = "proj-10",
            IterationKey = "iter-1"
        };

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _mediatorMock.Verify(m => m.Send(
            It.Is<AssignTicketToIterationCommand>(c => c.TicketId == 10 && c.IterationId == 5),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithIds_AssignsTicketToIteration()
    {
        // Arrange
        var ticket = new Ticket(1, 1, "PROJ-10", 10, "Title") { Id = 10 };
        var iteration = new Iteration(1, "ITER-1", 1, "Name", "Desc", null, null) { Id = 5 };

        _ticketRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);
        _iterationRepoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(iteration);

        var command = new AddTicketCommand
        {
            TicketId = 10,
            IterationId = 5
        };

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _mediatorMock.Verify(m => m.Send(
            It.Is<AssignTicketToIterationCommand>(c => c.TicketId == 10 && c.IterationId == 5),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_TicketNotFound_ThrowsException()
    {
        // Arrange
        _ticketRepoMock.Setup(r => r.GetByKeyAsync("PROJ-99", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticket?)null);

        // Ensure Iteration repo returns something or null, but doesn't throw NRE when accessed
        // Since we setup _uowMock.Setup(u => u.Iterations).Returns(_iterationRepoMock.Object), it's fine.
        var command = new AddTicketCommand
        {
            TicketKey = "PROJ-99",
            IterationId = 5
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.HandleAsync(null!, command, CancellationToken.None));
    }
}
