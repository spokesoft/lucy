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

        if (!statusList.Any())
        {
            console.MarkupLine($"[yellow]{localizer["View.Board.NoStatuses"]}[/]");
            return;
        }

        console.WriteLine();

        // Create a sleek table layout
        var table = new Table()
            .Border(TableBorder.None)
            .HideHeaders()
            .NoBorder();

        // Add columns for each status
        foreach (var status in statusList)
        {
            table.AddColumn(new TableColumn("").PadLeft(1).PadRight(1));
        }

        // Add status headers row
        table.AddRow(statusList
            .Select(s => CreateStatusHeader(s, ticketsByStatus.TryGetValue(s.Id, out var t) ? t.Count : 0))
            .ToArray());

        // Add separator row
        table.AddRow(statusList
            .Select(s => new Rule().RuleStyle(s.Color.ToString().ToLowerInvariant()))
            .ToArray());

        // Add tickets rows - find max ticket count
        var maxTickets = ticketsByStatus.Any() ? ticketsByStatus.Max(kvp => kvp.Value.Count) : 0;

        for (int i = 0; i < maxTickets; i++)
        {
            table.AddRow(statusList
                .Select(status =>
                {
                    var tickets = ticketsByStatus.TryGetValue(status.Id, out var t) ? t : new List<TicketDto>();
                    return i < tickets.Count
                        ? CreateTicketCard(tickets[i], localizer)
                        : (IRenderable)Text.Empty;
                })
                .ToArray());
        }

        // Add empty state if no tickets
        if (maxTickets == 0)
        {
            table.AddRow(statusList
                .Select(_ => new Markup($"[grey]{localizer["View.Board.NoTickets"]}[/]"))
                .ToArray());
        }

        console.Write(table);
        console.WriteLine();

        await Task.CompletedTask;
    }

    /// <summary>
    /// Creates a status header with count badge.
    /// </summary>
    private static Markup CreateStatusHeader(StatusDto status, int count)
    {
        var color = status.Color.ToString().ToLowerInvariant();
        var badge = count > 0 ? $" [white on {color}] {count} [/]" : "";
        return new Markup($"[bold {color}]{status.Key.ToUpper()}[/]{badge}");
    }

    /// <summary>
    /// Creates a minimal, sleek ticket card.
    /// </summary>
    private static Panel CreateTicketCard(TicketDto ticket, IStringLocalizer localizer)
    {
        var title = string.IsNullOrWhiteSpace(ticket.Title)
            ? $"[dim]{localizer["Empty.String"]}[/]"
            : Markup.Escape(ticket.Title);

        var description = string.IsNullOrWhiteSpace(ticket.Description)
            ? ""
            : $"\n[dim]{Markup.Escape(TruncateDescription(ticket.Description, 45))}[/]";

        var content = $"[blue][[{ticket.Key}]][/] [bold]{title}[/]{description}";

        return new Panel(new Markup(content))
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("grey23"))
            .Padding(1, 0)
            .Expand();
    }

    /// <summary>
    /// Truncates a description to a reasonable length for board display.
    /// </summary>
    private static string TruncateDescription(string description, int maxLength = 45)
    {
        if (description.Length <= maxLength)
            return description;

        return description[..(maxLength - 3)] + "...";
    }
}
