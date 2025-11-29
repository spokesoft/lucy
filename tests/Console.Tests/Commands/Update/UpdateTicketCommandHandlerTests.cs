using Lucy.Application.Interfaces;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.GetTicketById;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Console.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;
using ConsoleUpdateTicketCommand = Lucy.Console.Commands.Update.UpdateTicketCommand;
using ConsoleUpdateTicketCommandHandler = Lucy.Console.Commands.Update.UpdateTicketCommandHandler;
using ApplicationUpdateTicketCommand = Lucy.Application.Tickets.Commands.UpdateTicket.UpdateTicketCommand;

namespace Lucy.Console.Tests.Commands.Update;

public class UpdateTicketCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ConsoleUpdateTicketCommandHandler _handler;

    public UpdateTicketCommandHandlerTests()
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

        _handler = new ConsoleUpdateTicketCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_UpdateWithTicketId_ReturnsSuccess()
    {
        // Arrange
        var ticketId = 1L;
        var command = new ConsoleUpdateTicketCommand
        {
            Id = ticketId,
            Key = null,
            StatusKey = null,
            StatusId = 2L,
            Title = "Updated title",
            Description = "Updated description"
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ApplicationUpdateTicketCommand>(c =>
                    c.Id == ticketId &&
                    c.StatusId == command.StatusId &&
                    c.Title == command.Title &&
                    c.Description == command.Description),
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
    public async Task HandleAsync_UpdateWithTicketKey_ReturnsSuccess()
    {
        // Arrange
        var ticketId = 1L;
        var ticketKey = "ABC-1";
        var command = new ConsoleUpdateTicketCommand
        {
            Id = null,
            Key = ticketKey,
            StatusKey = null,
            StatusId = 2L,
            Title = "Updated title",
            Description = null
        };

        var ticket = new TicketDto
        {
            Id = ticketId,
            ProjectId = 1L,
            StatusId = 1L,
            Key = ticketKey,
            Title = "Old title",
            Description = "Old description",
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
                It.Is<ApplicationUpdateTicketCommand>(c =>
                    c.Id == ticketId &&
                    c.StatusId == command.StatusId &&
                    c.Title == command.Title &&
                    c.Description == command.Description),
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
    public async Task HandleAsync_UpdateWithStatusKey_ReturnsSuccess()
    {
        // Arrange
        var ticketId = 1L;
        var projectId = 1L;
        var statusId = 3L;
        var command = new ConsoleUpdateTicketCommand
        {
            Id = ticketId,
            Key = null,
            StatusKey = "DONE",
            StatusId = null,
            Title = "Updated title",
            Description = null
        };

        var ticket = new TicketDto
        {
            Id = ticketId,
            ProjectId = projectId,
            StatusId = 1L,
            Key = "ABC-1",
            Title = "Old title",
            Description = "Old description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var status = new StatusDto
        {
            Id = statusId,
            ProjectId = projectId,
            Key = "DONE",
            Name = "Done",
            Description = "Completed tasks",
            Order = 3,
            Color = Domain.Enums.Color.Green,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetTicketByIdQuery>(q => q.Id == ticketId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetStatusByKeyQuery>(q => q.ProjectId == projectId && q.Key == command.StatusKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ApplicationUpdateTicketCommand>(c =>
                    c.Id == ticketId &&
                    c.StatusId == statusId &&
                    c.Title == command.Title &&
                    c.Description == command.Description),
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
    public async Task HandleAsync_UpdateTitleOnly_ReturnsSuccess()
    {
        // Arrange
        var ticketId = 1L;
        var command = new ConsoleUpdateTicketCommand
        {
            Id = ticketId,
            Key = null,
            StatusKey = null,
            StatusId = null,
            Title = "Updated title only",
            Description = null
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<ApplicationUpdateTicketCommand>(c =>
                    c.Id == ticketId &&
                    c.StatusId == null &&
                    c.Title == command.Title &&
                    c.Description == null),
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
