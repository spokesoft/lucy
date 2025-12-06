using AppAddTicketTagCommand = Lucy.Application.TicketTags.Commands.AddTicketTag.AddTicketTagCommand;
using Lucy.Application.Interfaces;
using Lucy.Application.Tags.Queries.GetTagIdByKey;
using Lucy.Application.Tickets.Queries.GetTicketById;
using Lucy.Application.Tickets.Queries.GetTicketByKey;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Add;

/// <summary>
/// Handler for the <see cref="AddTagCommand"/> command.
/// </summary>
internal class AddTagCommandHandler(
    IAnsiConsole console,
    IMediator mediator,
    IStringLocalizer<Program> localizer) : ICommandHandler<AddTagCommand>
{
    private readonly IAnsiConsole _console = console;
    private readonly IMediator _mediator = mediator;
    private readonly IStringLocalizer<Program> _localizer = localizer;

    /// <inheritdoc />
    public async Task<ExitCode> HandleAsync(
        CommandContext context,
        AddTagCommand command,
        CancellationToken token = default)
    {
        // Resolve ticket
        var ticketId = command.TicketId;
        var ticket = command.TicketId is null
            ? await _mediator.Send(new GetTicketByKeyQuery(command.TicketKey!), token)
            : await _mediator.Send(new GetTicketByIdQuery(command.TicketId.Value), token);

        ticketId ??= ticket!.Id;

        // Resolve tag (using project from ticket when resolving by key)
        var tagId = command.TagId;
        if (tagId is null)
        {
            var resolvedProjectId = ticket!.ProjectId;
            tagId = await _mediator.Send(new GetTagIdByKeyQuery(resolvedProjectId, command.TagKey!), token);
        }

        var request = new AppAddTicketTagCommand(tagId!.Value, ticketId.Value);
        await _mediator.Send(request, token);

        _console.MarkupLine("[green]:check_mark:[/] " + _localizer["Messages.AddedTagToTicket", tagId, ticketId]);

        return ExitCode.Success;
    }
}
