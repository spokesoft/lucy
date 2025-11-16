using Lucy.Application.Interfaces;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.GetTicketById;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Console.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;
using ConsoleDeleteTicketCommand = Lucy.Console.Commands.Delete.DeleteTicketCommand;
using ConsoleDeleteTicketCommandHandler = Lucy.Console.Commands.Delete.DeleteTicketCommandHandler;
using ApplicationDeleteTicketCommand = Lucy.Application.Tickets.Commands.DeleteTicket.DeleteTicketCommand;

namespace Lucy.Console.Tests.Commands.Delete;

public class DeleteTicketCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ConsoleDeleteTicketCommandHandler _handler;

    public DeleteTicketCommandHandlerTests()
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

        _handler = new ConsoleDeleteTicketCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_DeleteWithTicketId_ReturnsSuccess()
    {
        // Arrange
        var ticketId = 1L;
        var command = new ConsoleDeleteTicketCommand
        {
            Id = ticketId,
            Key = null
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ApplicationDeleteTicketCommand>(c => c.Id == ticketId),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        var output = _console.Output;
        Assert.Contains(ticketId.ToString(), output);
    }

    [Fact]
    public async Task HandleAsync_DeleteWithTicketKey_ReturnsSuccess()
    {
        // Arrange
        var ticketId = 1L;
        var ticketKey = "ABC-1";
        var command = new ConsoleDeleteTicketCommand
        {
            Id = null,
            Key = ticketKey
        };

        var ticket = new TicketDto
        {
            Id = ticketId,
            ProjectId = 1L,
            StatusId = 1L,
            Key = ticketKey,
            Title = "Test ticket",
            Description = "Test description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetTicketByKeyQuery>(q => q.Key == ticketKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ApplicationDeleteTicketCommand>(c => c.Id == ticketId),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        var output = _console.Output;
        Assert.Contains(ticketId.ToString(), output);
    }
}
