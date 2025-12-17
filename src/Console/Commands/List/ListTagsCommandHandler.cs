using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectIdByKey;
using Lucy.Application.Tags.DTOs;
using Lucy.Application.Tags.Queries.ListTags;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Handler for the <see cref="ListTagsCommand"/> command.
/// </summary>
internal class ListTagsCommandHandler(
    IAnsiConsole console,
    IStringLocalizer<Program> localizer,
    IViewRenderer<IEnumerable<TagDto>> viewRenderer,
    IMediator mediator) : ICommandHandler<ListTagsCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IStringLocalizer<Program> _localizer = localizer;
    private readonly IViewRenderer<IEnumerable<TagDto>> _view = viewRenderer;
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        ListTagsCommand command,
        CancellationToken token = default)
    {
        var projectId = command.Id ?? await _mediator.Send(
            new GetProjectIdByKeyQuery(command.Key!), token);

        var query = new ListTagsQuery(projectId.Value);
        var tags = await _mediator.Send(query, token);

        await _view.RenderAsync(tags, _console, _localizer, token);
        return ExitCode.Success;
    }
}
