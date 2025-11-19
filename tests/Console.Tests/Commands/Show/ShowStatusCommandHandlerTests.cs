using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Projects.Queries.GetProjectById;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Statuses.Queries.GetStatusById;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Console.Commands.Show;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Lucy.Domain.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;
using Lucy.Application.Projects.DTOs;
using Lucy.Application.Tickets.DTOs;

namespace Lucy.Console.Tests.Commands.Show;

public class ShowStatusCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IViewRenderer<(StatusDto, ProjectDto, List<TicketDto>)>> _viewRendererMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ShowStatusCommandHandler _handler;

    public ShowStatusCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _viewRendererMock = new Mock<IViewRenderer<(StatusDto, ProjectDto, List<TicketDto>)>>();
        _mediatorMock = new Mock<IMediator>();

        _handler = new ShowStatusCommandHandler(
            _console,
            _localizerMock.Object,
            _viewRendererMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidId_ReturnsSuccess()
    {
        // Arrange
        var statusId = 5L;
        var command = new ShowStatusCommand
        {
            StatusId = statusId,
            StatusKey = null,
            ProjectKey = null,
            ProjectId = null
        };
        var status = new StatusDto
        {
            Id = statusId,
            ProjectId = 1L,
            Key = "TODO",
            Order = 1,
            Name = "To Do",
            Description = "Tasks to do",
            Color = StatusColor.Gray,
            CreatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetStatusByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var dummyProject = new ProjectDto {
            Id = status.ProjectId,
            Key = "EXAMP",
            Name = "Example Project",
            Description = "Project Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dummyProject);

        var dummyTickets = new List<TicketDto>();

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);

        // dummyProject and dummyTickets declared above
        _viewRendererMock.Verify(
            v => v.RenderAsync(
                It.Is<(StatusDto, ProjectDto, List<TicketDto>)>(t =>
                    t.Item1.Id == status.Id &&
                    t.Item2.Id == dummyProject.Id &&
                    t.Item3.Count == dummyTickets.Count),
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithValidKeysUsingProjectKey_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 5L;
        var projectKey = "EXAMP";
        var statusKey = "TODO";

        var command = new ShowStatusCommand
        {
            StatusId = null,
            StatusKey = statusKey,
            ProjectKey = projectKey,
            ProjectId = null
        };

        var statusDto = new StatusDto
        {
            Id = statusId,
            ProjectId = projectId,
            Key = statusKey,
            Order = 1,
            Name = "To Do",
            Description = "Tasks to do",
            Color = StatusColor.Gray,
            CreatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProjectIdByKeyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetStatusByKeyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(statusDto);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetStatusByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(statusDto);

        var dummyProject = new ProjectDto {
            Id = projectId,
            Key = projectKey,
            Name = "Example Project",
            Description = "Project Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dummyProject);

        var dummyTickets = new List<TicketDto>();

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _viewRendererMock.Verify(
            v => v.RenderAsync(
                It.Is<(StatusDto, ProjectDto, List<TicketDto>)>(t =>
                    t.Item1.Id == statusDto.Id &&
                    t.Item2.Id == dummyProject.Id &&
                    t.Item3.Count == dummyTickets.Count),
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithValidKeysUsingProjectId_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 5L;
        var statusKey = "TODO";

        var command = new ShowStatusCommand
        {
            StatusId = null,
            StatusKey = statusKey,
            ProjectKey = null,
            ProjectId = projectId
        };

        var statusDto = new StatusDto
        {
            Id = statusId,
            ProjectId = projectId,
            Key = statusKey,
            Order = 1,
            Name = "To Do",
            Description = "Tasks to do",
            Color = StatusColor.Gray,
            CreatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime(),
            UpdatedAt = DateTime.Parse("2025-11-15 16:01:15Z").ToUniversalTime()
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetStatusByKeyQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(statusDto);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetStatusByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(statusDto);

        var dummyProject = new ProjectDto {
            Id = projectId,
            Key = "EXAMP",
            Name = "Example Project",
            Description = "Project Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dummyProject);

        var dummyTickets = new List<TicketDto>();

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        _viewRendererMock.Verify(
            v => v.RenderAsync(
                It.Is<(StatusDto, ProjectDto, List<TicketDto>)>(t =>
                    t.Item1.Id == statusDto.Id &&
                    t.Item2.Id == dummyProject.Id &&
                    t.Item3.Count == dummyTickets.Count),
                _console,
                _localizerMock.Object,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_StatusNotFound_ReturnsError()
    {
        // Arrange
        var statusId = 5L;
        var command = new ShowStatusCommand
        {
            StatusId = statusId,
            StatusKey = null,
            ProjectKey = null,
            ProjectId = null
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetStatusByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StatusDto?)null);

        _localizerMock
            .Setup(l => l["Error.Status.NotFound"])
            .Returns(new LocalizedString("Error.Status.NotFound", "Status not found"));

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Error, result);
        Assert.Contains("Status not found", _console.Output);
    }
}
