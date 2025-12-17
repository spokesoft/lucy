using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Application.Tickets.Commands.CreateTicket;
using Lucy.Console.Commands.New;
using Lucy.Console.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.New;

public class NewTicketCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly NewTicketCommandHandler _handler;

    public NewTicketCommandHandlerTests()
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

        _handler = new NewTicketCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCommandWithProjectKey_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var ticketId = 10L;
        var command = new NewTicketCommand
        {
            Title = "Test ticket",
            ProjectKey = "ABC",
            ProjectId = null,
            StatusKey = "TODO",
            StatusId = null,
            Description = "Test description"
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.ProjectKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetStatusByKeyQuery>(q => q.ProjectId == projectId && q.Key == command.StatusKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StatusDto { Id = statusId });

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateTicketCommand>(c =>
                    c.ProjectId == projectId &&
                    c.StatusId == statusId &&
                    c.Title == command.Title &&
                    c.Description == command.Description),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticketId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        var output = _console.Output;
        Assert.Contains(command.Title, output);
        Assert.Contains(ticketId.ToString(), output);
    }

    [Fact]
    public async Task HandleAsync_ValidCommandWithProjectId_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var ticketId = 10L;
        var command = new NewTicketCommand
        {
            Title = "Test ticket",
            ProjectKey = null,
            ProjectId = projectId,
            StatusKey = "TODO",
            StatusId = null,
            Description = "Test description"
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetStatusByKeyQuery>(q => q.ProjectId == projectId && q.Key == command.StatusKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StatusDto { Id = statusId });

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateTicketCommand>(c =>
                    c.ProjectId == projectId &&
                    c.StatusId == statusId &&
                    c.Title == command.Title &&
                    c.Description == command.Description),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticketId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        var output = _console.Output;
        Assert.Contains(command.Title, output);
        Assert.Contains(ticketId.ToString(), output);
    }

    [Fact]
    public async Task HandleAsync_ValidCommandWithStatusId_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var ticketId = 10L;
        var command = new NewTicketCommand
        {
            Title = "Test ticket",
            ProjectKey = "ABC",
            ProjectId = null,
            StatusKey = null,
            StatusId = statusId,
            Description = null
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.ProjectKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateTicketCommand>(c =>
                    c.ProjectId == projectId &&
                    c.StatusId == statusId &&
                    c.Title == command.Title &&
                    c.Description == command.Description),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticketId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        var output = _console.Output;
        Assert.Contains(command.Title, output);
        Assert.Contains(ticketId.ToString(), output);
    }
}
