using Lucy.Application.Tickets.DTOs;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;

namespace Lucy.Console.Views.Tickets;

/// <summary>
/// Renders the ticket list view to the console.
/// </summary>
public class TicketListViewRenderer : IViewRenderer<(IEnumerable<TicketDto>, Dictionary<long, (string Key, string Color)>)>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        (IEnumerable<TicketDto>, Dictionary<long, (string Key, string Color)>) model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var (tickets, statusLookup) = model;
        var ticketList = tickets.ToList();
        var title = GetTitle(localizer, console.Profile);
        var caption = ticketList.Any()
            ? GetCaption(localizer, 1, ticketList.Count, ticketList.Count)
            : localizer["View.TicketList.NoTickets"];

        var table = new Table()
            .Border(TableBorder.SimpleHeavy)
            .Title(title)
            .Caption(caption, new Style(foreground: Color.Grey))
            .Width(80);

        table.AddColumn(localizer["Property.Ticket.Key"]);
        table.AddColumn(localizer["Property.Status"]);
        table.AddColumn(localizer["Property.Ticket.Title"]);
        table.AddColumn(localizer["Property.UpdatedAt"]);

        foreach (var ticket in ticketList)
        {
            var ticketTitle = string.IsNullOrWhiteSpace(ticket.Title)
                ? $"[grey70]{localizer["Empty.String"]}[/]"
                : ticket.Title;

            var status = statusLookup.TryGetValue(ticket.StatusId, out var statusInfo)
                ? $"[{statusInfo.Color}]{statusInfo.Key}[/]"
                : $"[gray]{ticket.StatusId}[/]";

            table.AddRow(
                $"[{statusInfo.Color}]{ticket.Key}[/]",
                status,
                ticketTitle,
                ticket.UpdatedAt.ToString("yyyy-MM-dd"));
        }

        console.WriteLine();
        console.Write(table);
        console.WriteLine();

        await Task.CompletedTask;
    }

    /// <summary>
    /// Gets the title for the ticket list view.
    /// </summary>
    private static string GetTitle(IStringLocalizer localizer, Profile options)
        => options.Capabilities.Unicode
            ? $":ticket: [u]{localizer["View.TicketList.Title"]}[/]"
            : $"[u]{localizer["View.TicketList.Title"]}[/]";

    /// <summary>
    /// Gets the caption for the ticket list view.
    /// </summary>
    private static string GetCaption(IStringLocalizer localizer, int firstIndex, int lastIndex, int count)
        => count == 0 ?
            localizer["View.TicketList.NoTickets"] :
            localizer["View.TicketList.Caption", firstIndex, lastIndex, count];
}
