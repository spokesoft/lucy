using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Statuses.Queries.GetStatusByKey;
using Lucy.Console.Commands.Update;
using Lucy.Console.Enums;
using Lucy.Domain.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.Update;

public class UpdateStatusCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly UpdateStatusCommandHandler _handler;

    public UpdateStatusCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _mediatorMock = new Mock<IMediator>();

        // Setup localizer to return formatted string with parameters
        _localizerMock
            .Setup(x => x[It.IsAny<string>(), It.IsAny<object[]>()])
            .Returns((string key, object[] args) =>
            {
                // For test purposes, just concatenate key and args
                var formatted = args.Length > 0 ? $"{key} {string.Join(" ", args)}" : key;
                return new LocalizedString(key, formatted);
            });

        _handler = new UpdateStatusCommandHandler(
            _console,
            _mediatorMock.Object,
            _localizerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_UpdateByKeysWithProjectKey_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 5L;
        var projectKey = "EXAMP";
        var statusKey = "TODO";
        var command = new UpdateStatusCommand
        {
            ProjectKey = projectKey,
            StatusKey = statusKey,
            ProjectId = null,
            StatusId = null,
            NewKey = "BACKLOG",
            Order = 2,
            Name = "Updated Name",
            Description = "Updated Description",
            Color = Color.Blue
        };
        var statusDto = new StatusDto
        {
            Id = statusId,
            ProjectId = projectId,
            Key = statusKey,
            Order = 1,
            Name = "To Do",
            Description = "Tasks to do",
            Color = Color.Gray,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == projectKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetStatusByKeyQuery>(q => q.ProjectId == projectId && q.Key == statusKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statusDto);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains(statusKey, _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<Application.Statuses.Commands.UpdateStatus.UpdateStatusCommand>(c =>
                    c.Id == statusId &&
                    c.Key == command.NewKey &&
                    c.Order == command.Order &&
                    c.Name == command.Name &&
                    c.Description == command.Description &&
                    c.Color == command.Color),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UpdateByKeysWithProjectId_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 5L;
        var statusKey = "TODO";
        var command = new UpdateStatusCommand
        {
            ProjectKey = null,
            StatusKey = statusKey,
            ProjectId = projectId,
            StatusId = null,
            NewKey = null,
            Order = 2,
            Name = "Updated Name",
            Description = null,
            Color = null
        };
        var statusDto = new StatusDto
        {
            Id = statusId,
            ProjectId = projectId,
            Key = statusKey,
            Order = 1,
            Name = "To Do",
            Description = "Tasks to do",
            Color = Color.Gray,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetStatusByKeyQuery>(q => q.ProjectId == projectId && q.Key == statusKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statusDto);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains(statusKey, _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<Application.Statuses.Commands.UpdateStatus.UpdateStatusCommand>(c =>
                    c.Id == statusId &&
                    c.Key == command.NewKey &&
                    c.Order == command.Order &&
                    c.Name == command.Name &&
                    c.Description == command.Description &&
                    c.Color == command.Color),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_UpdateById_ReturnsSuccess()
    {
        // Arrange
        var statusId = 5L;
        var command = new UpdateStatusCommand
        {
            ProjectKey = null,
            StatusKey = null,
            ProjectId = null,
            StatusId = statusId,
            NewKey = null,
            Order = null,
            Name = "Updated Name",
            Description = "Updated Description",
            Color = Color.Green
        };

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains(statusId.ToString(), _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<Application.Statuses.Commands.UpdateStatus.UpdateStatusCommand>(c =>
                    c.Id == statusId &&
                    c.Key == command.NewKey &&
                    c.Order == command.Order &&
                    c.Name == command.Name &&
                    c.Description == command.Description &&
                    c.Color == command.Color),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_PartialUpdate_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 5L;
        var statusKey = "TODO";
        var command = new UpdateStatusCommand
        {
            ProjectKey = null,
            StatusKey = statusKey,
            ProjectId = projectId,
            StatusId = null,
            NewKey = null,
            Order = null,
            Name = "Updated Name",
            Description = null,
            Color = null
        };
        var statusDto = new StatusDto
        {
            Id = statusId,
            ProjectId = projectId,
            Key = statusKey,
            Order = 1,
            Name = "To Do",
            Description = "Tasks to do",
            Color = Color.Gray,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetStatusByKeyQuery>(q => q.ProjectId == projectId && q.Key == statusKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statusDto);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        Assert.Contains(statusKey, _console.Output);

        _mediatorMock.Verify(
            m => m.Send(
                It.Is<Application.Statuses.Commands.UpdateStatus.UpdateStatusCommand>(c =>
                    c.Id == statusId &&
                    c.Key == command.NewKey &&
                    c.Order == command.Order &&
                    c.Name == command.Name &&
                    c.Description == command.Description &&
                    c.Color == command.Color),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
