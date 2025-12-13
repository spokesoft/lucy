using Lucy.Application.Interfaces;
using Lucy.Application.Tags.Queries.GetTagIdByKey;
using Lucy.Application.Tickets.DTOs;
using Lucy.Application.Tickets.Queries.GetTicketById;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Console.Commands.Remove;
using Lucy.Console.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;
using AppRemoveTicketTagCommand = Lucy.Application.TicketTags.Commands.RemoveTicketTag.RemoveTicketTagCommand;

namespace Lucy.Console.Tests.Commands.Remove;

public class RemoveTagCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly RemoveTagCommandHandler _handler;

    public RemoveTagCommandHandlerTests()
    {
        _console = new TestConsole();
        _mediatorMock = new Mock<IMediator>();
        _localizerMock = new Mock<IStringLocalizer<Program>>();

        _localizerMock
            .Setup(l => l[It.IsAny<string>(), It.IsAny<object[]>()])
            .Returns((string key, object[] args) => new LocalizedString(key, $"{key} {string.Join(' ', args)}"));

        _handler = new RemoveTagCommandHandler(_console, _mediatorMock.Object, _localizerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithKeys_ResolvesAndRemovesTag()
    {
        var ticket = new TicketDto
        {
            Id = 10,
            ProjectId = 1,
            StatusId = 2,
            Key = "PROJ-10",
            Number = 10,
            Title = "Issue",
            Description = "Desc",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetTicketByKeyQuery>(q => q.Key == ticket.Key), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetTagIdByKeyQuery>(q => q.ProjectId == ticket.ProjectId && q.Key == "BUG"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(5L);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AppRemoveTicketTagCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new RemoveTagCommand
        {
            TicketKey = ticket.Key,
            TicketId = null,
            TagKey = "BUG",
            TagId = null
        };

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.RemovedTagFromTicket", _console.Output);
        Assert.Contains("5", _console.Output);
        Assert.Contains(ticket.Id.ToString(), _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<AppRemoveTicketTagCommand>(c => c.TagId == 5 && c.TicketId == ticket.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithIds_UsesDirectIds()
    {
        var ticket = new TicketDto
        {
            Id = 42,
            ProjectId = 7,
            StatusId = 2,
            Key = "PROJ-42",
            Number = 42,
            Title = "Issue",
            Description = null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetTicketByIdQuery>(q => q.Id == ticket.Id), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AppRemoveTicketTagCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new RemoveTagCommand
        {
            TicketKey = null,
            TicketId = ticket.Id,
            TagKey = null,
            TagId = 9
        };

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.RemovedTagFromTicket", _console.Output);
        Assert.Contains("9", _console.Output);
        Assert.Contains(ticket.Id.ToString(), _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<AppRemoveTicketTagCommand>(c => c.TagId == 9 && c.TicketId == ticket.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _mediatorMock.Verify(
            m => m.Send(It.IsAny<GetTagIdByKeyQuery>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
