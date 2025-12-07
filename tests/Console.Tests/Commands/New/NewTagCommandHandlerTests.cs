using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Console.Commands.New;
using Lucy.Console.Enums;
using Lucy.Domain.Enums;
using Microsoft.Extensions.Localization;
using Moq;
using Spectre.Console.Testing;
using AppCreateTagCommand = Lucy.Application.Tags.Commands.CreateTag.CreateTagCommand;

namespace Lucy.Console.Tests.Commands.New;

public class NewTagCommandHandlerTests
{
    private readonly TestConsole _console;
    private readonly Mock<IStringLocalizer<Program>> _localizerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly NewTagCommandHandler _handler;

    public NewTagCommandHandlerTests()
    {
        _console = new TestConsole();
        _localizerMock = new Mock<IStringLocalizer<Program>>();
        _mediatorMock = new Mock<IMediator>();

        _localizerMock
            .Setup(x => x[It.IsAny<string>(), It.IsAny<object[]>()])
            .Returns((string key, object[] args) => new LocalizedString(key, $"{key} {string.Join(' ', args)}"));

        _handler = new NewTagCommandHandler(
            _console,
            _localizerMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_WithProjectKey_ResolvesProjectAndCreatesTag()
    {
        var command = new NewTagCommand
        {
            ProjectKey = "EXAMP",
            ProjectId = null,
            Key = "BUG",
            Label = "Bug",
            Description = "Bug description",
            Color = Color.Red
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetProjectIdByKeyQuery>(q => q.Key == command.ProjectKey), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1L);

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<AppCreateTagCommand>(c =>
                    c.ProjectId == 1 &&
                    c.Key == command.Key &&
                    c.Label == command.Label &&
                    c.Description == command.Description &&
                    c.Color == command.Color),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(5L);

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.CreatedTag", _console.Output);
        Assert.Contains("5", _console.Output);
    }

    [Fact]
    public async Task HandleAsync_WithProjectId_CreatesTag()
    {
        var command = new NewTagCommand
        {
            ProjectKey = null,
            ProjectId = 7,
            Key = "FEAT",
            Label = null,
            Description = null,
            Color = null
        };

        _mediatorMock
            .Setup(m => m.Send(
                It.Is<AppCreateTagCommand>(c =>
                    c.ProjectId == command.ProjectId &&
                    c.Key == command.Key &&
                    c.Label == command.Label &&
                    c.Description == command.Description &&
                    c.Color == command.Color),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(12L);

        var result = await _handler.HandleAsync(null!, command, CancellationToken.None);

        Assert.Equal(ExitCode.Success, result);
        Assert.Contains("Messages.CreatedTag", _console.Output);
        Assert.Contains("12", _console.Output);

        _mediatorMock.Verify(
            m => m.Send(It.Is<GetProjectIdByKeyQuery>(q => true), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
