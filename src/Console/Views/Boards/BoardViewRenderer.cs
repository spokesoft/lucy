using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Tickets.DTOs;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Lucy.Console.Views.Boards;

/// <summary>
/// Renders the board view to the console.
/// </summary>
public class BoardViewRenderer : IViewRenderer<(IEnumerable<StatusDto>, Dictionary<long, List<TicketDto>>)>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        (IEnumerable<StatusDto>, Dictionary<long, List<TicketDto>>) model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var (statuses, ticketsByStatus) = model;
        var statusList = statuses.OrderBy(s => s.Order).ToList();

        if (statusList.Count == 0)
        {
            console.MarkupLine($"[yellow]{localizer["View.Board.NoStatuses"]}[/]");
            return;
        }

        console.WriteLine();

        // Create a sleek table layout
        var table = new Table()
            .Border(TableBorder.None)
            .HideHeaders()
            .NoBorder()
            .Width(80);

        // Add columns for each status
        foreach (var status in statusList)
        {
            table.AddColumn(new TableColumn("").PadLeft(1).PadRight(1));
        }

        // Print each status (column) vertically, with tickets stacked below
        foreach (var status in statusList)
        {
            var statusName = status.Name ?? status.Key;
                // Render a fixed-width rule (max 80) with the status name centered
                var safeStatusName = Markup.Escape(statusName);
                var ruleText = $"[bold]{safeStatusName}[/]";
                int ruleTextLen = new Markup(ruleText).Length;
                int totalWidth = 80;
                int dashCount = totalWidth - ruleTextLen - 2; // 2 spaces padding
                int leftDash = dashCount / 2;
                int rightDash = dashCount - leftDash;
                var dashesLeft = new string('─', leftDash);
                var dashesRight = new string('─', rightDash);
                var coloredRule = $"[{status.Color.ToString().ToLowerInvariant()}]{dashesLeft} {ruleText} {dashesRight}[/]";
                console.MarkupLine(coloredRule);

            var cardTable = new Table()
                .Border(TableBorder.None)
                .HideHeaders()
                .AddColumn(new TableColumn("").Width(80));

            if (!ticketsByStatus.TryGetValue(status.Id, out var tickets) || tickets.Count == 0)
            {
                var noTicketsMsg = Markup.Escape(localizer["View.Board.NoTickets"]);
                var emptyPanel = new Panel(new Markup($"[dim]{noTicketsMsg}[/]"))
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(Style.Parse("grey23"))
                    .Padding(1, 0, 1, 0);
                cardTable.AddRow(emptyPanel);
            }
            else
            {
                foreach (var ticket in tickets)
                {
                    var safeTitle = Markup.Escape(ticket.Title);
                    var updated = ticket.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    var updatedLabel = localizer["Property.UpdatedAt"];
                    var ticketContent = $"[bold]#{ticket.Id}[/] {safeTitle}\n[dim]{updatedLabel}: {updated}[/]";
                    var panel = new Panel(new Markup(ticketContent))
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(Style.Parse("grey23"))
                        .Padding(1, 0, 1, 0);
                    cardTable.AddRow(panel);
                }
            }
            console.Write(cardTable);
            console.WriteLine();
        }
        await Task.CompletedTask;
    }
    /// Truncates a description to a reasonable length for board display.
    /// </summary>
    private static string TruncateDescription(string description, int maxLength = 45)
    {
        if (string.IsNullOrEmpty(description) || description.Length <= maxLength)
            return description;
        return description.Substring(0, maxLength - 3) + "...";
    }
}
