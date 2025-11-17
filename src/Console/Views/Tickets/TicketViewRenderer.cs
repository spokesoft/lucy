using Lucy.Application.Comments.DTOs;
using Lucy.Application.Tickets.DTOs;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;

namespace Lucy.Console.Views.Tickets;

/// <summary>
/// Renders the ticket view to the console.
/// </summary>
public class TicketViewRenderer : IViewRenderer<(TicketDto, IEnumerable<CommentDto>)>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        (TicketDto, IEnumerable<CommentDto>) model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var (ticket, comments) = model;
        var commentList = comments.ToList();

        console.WriteLine();

        var title = GetTitle(localizer, console.Profile);
        var content = BuildContent(ticket, commentList, localizer);

        var panel = new Panel(content)
            .Header(title, Justify.Left)
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("grey23"))
            .Padding(1, 1, 1, 0);

        var table = new Table()
            .Border(TableBorder.None)
            .HideHeaders()
            .AddColumn(new TableColumn("").Width(80));

        table.AddRow(panel);

        console.Write(table);

        await Task.CompletedTask;
    }

    /// <summary>
    /// Builds the complete content including details and comments.
    /// </summary>
    private static Rows BuildContent(TicketDto ticket, List<CommentDto> commentList, IStringLocalizer localizer)
    {
        // Ticket details
        var detailsMarkup = GetTicketDetailsMarkup(ticket, localizer);
        var detailsText = new Markup(detailsMarkup);

        // Comments section
        if (commentList.Any())
        {
            var commentsRows = commentList.Select(comment =>
            {
                var content = string.IsNullOrWhiteSpace(comment.Content)
                    ? $"[dim]{localizer["Empty.String"]}[/]"
                    : Markup.Escape(comment.Content);

                var timestamp = comment.UpdatedAt.ToString("MMM d, yyyy HH:mm");
                var header = $"[dim]#{comment.Id} · {timestamp}[/]";

                return new Panel(new Markup(content))
                    .Header(header, Justify.Left)
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("grey23"))
                    .Padding(1, 0);
            }).ToArray();

            var separator = new Rule($"[bold]{localizer["View.CommentList.Title"]}[/] [dim]({commentList.Count})[/]")
                .LeftJustified()
                .RuleStyle(Style.Parse("grey23"));

            return new Rows(detailsText, Text.Empty, separator, Text.Empty, new Rows(commentsRows));
        }

        return new Rows(detailsText);
    }    /// <summary>
    /// Gets the title for the ticket view.
    /// </summary>
    private static string GetTitle(IStringLocalizer localizer, Profile options)
        => options.Capabilities.Unicode
            ? $":ticket: {localizer["View.ShowTicket.Title"]}"
            : localizer["View.ShowTicket.Title"];

    /// <summary>
    /// Gets the ticket details formatted as markup.
    /// </summary>
    private static string GetTicketDetailsMarkup(TicketDto ticket, IStringLocalizer localizer)
    {
        var ticketTitle = string.IsNullOrWhiteSpace(ticket.Title)
            ? $"[dim]{localizer["Empty.String"]}[/]"
            : ticket.Title;

        var description = string.IsNullOrWhiteSpace(ticket.Description)
            ? $"[dim]{localizer["Empty.String"]}[/]"
            : ticket.Description;

        return $"[bold]{localizer["Property.Ticket.Key"]}:[/] [blue]{ticket.Key}[/]\n" +
               $"[bold]{localizer["Property.Ticket.Title"]}:[/] {ticketTitle}\n" +
               $"[bold]{localizer["Property.Ticket.Description"]}:[/] {description}\n" +
               $"[dim]{localizer["Property.UpdatedAt"]}:[/] [dim]{ticket.UpdatedAt:yyyy-MM-dd HH:mm:ss}[/]";
    }
}
