using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Statuses.Commands.CreateStatus;
using Lucy.Console.Commands.New;
using Lucy.Console.Enums;
using Lucy.Domain.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.New;

public class NewStatusCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly NewStatusCommandHandler _handler;

    public NewStatusCommandHandlerTests()
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

        _handler = new NewStatusCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCommandWithProjectKey_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 5L;
        var command = new NewStatusCommand
        {
            ProjectKey = "EXAMP",
            Key = "TODO",
            ProjectId = null,
            Name = "To Do",
            Description = "Tasks to do",
            Order = 1,
            Color = Color.Gray
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.ProjectKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateStatusCommand>(c =>
                    c.ProjectId == projectId &&
                    c.Key == command.Key &&
                    c.Name == command.Name &&
                    c.Description == command.Description &&
                    c.Order == command.Order &&
                    c.Color == command.Color),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statusId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        var output = _console.Output;
        Assert.Contains(command.Key, output);
        Assert.Contains(command.ProjectKey, output);
        Assert.Contains(statusId.ToString(), output);
    }

    [Fact]
    public async Task HandleAsync_ValidCommandWithProjectId_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 5L;
        var command = new NewStatusCommand
        {
            ProjectKey = "EXAMP",
            Key = "TODO",
            ProjectId = projectId,
            Name = "To Do",
            Description = "Tasks to do",
            Order = 1,
            Color = Color.Gray
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateStatusCommand>(c =>
                    c.ProjectId == projectId &&
                    c.Key == command.Key &&
                    c.Name == command.Name &&
                    c.Description == command.Description &&
                    c.Order == command.Order &&
                    c.Color == command.Color),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statusId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        var output = _console.Output;
        Assert.Contains(command.Key, output);
        Assert.Contains(statusId.ToString(), output);
    }

    [Fact]
    public async Task HandleAsync_MinimalCommand_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 5L;
        var command = new NewStatusCommand
        {
            ProjectKey = "EXAMP",
            Key = "TODO",
            ProjectId = null,
            Name = string.Empty,
            Description = string.Empty,
            Order = null,
            Color = null
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.ProjectKey),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateStatusCommand>(c =>
                    c.ProjectId == projectId &&
                    c.Key == command.Key &&
                    c.Name == command.Name &&
                    c.Description == command.Description &&
                    c.Order == command.Order &&
                    c.Color == command.Color),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(statusId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        var output = _console.Output;
        Assert.Contains(command.Key, output);
        Assert.Contains(command.ProjectKey, output);
        Assert.Contains(statusId.ToString(), output);
    }
}
