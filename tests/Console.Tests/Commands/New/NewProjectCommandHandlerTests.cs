using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Commands.CreateProject;
using Lucy.Console.Commands.New;
using Lucy.Console.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;

namespace Lucy.Console.Tests.Commands.New;

public class NewProjectCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly NewProjectCommandHandler _handler;

    public NewProjectCommandHandlerTests()
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

        _handler = new NewProjectCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new NewProjectCommand
        {
            Key = "TEST",
            Name = "Test Project",
            Description = "Test Description"
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateProjectCommand>(c =>
                    c.Key == command.Key &&
                    c.Name == command.Name &&
                    c.Description == command.Description),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        var output = _console.Output;
        Assert.Contains(command.Key, output);
        Assert.Contains(projectId.ToString(), output);
    }

    [Fact]
    public async Task HandleAsync_MinimalCommand_ReturnsSuccess()
    {
        // Arrange
        var projectId = 1L;
        var command = new NewProjectCommand
        {
            Key = "TEST",
            Name = "Test Project",
            Description = string.Empty
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<CreateProjectCommand>(c =>
                    c.Key == command.Key &&
                    c.Name == command.Name &&
                    c.Description == command.Description),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(projectId);

        // Act
        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        // Assert
        Assert.Equal(ExitCode.Success, result);
        var output = _console.Output;
        Assert.Contains(command.Key, output);
        Assert.Contains(projectId.ToString(), output);
    }
}
