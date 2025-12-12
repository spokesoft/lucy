using Lucy.Application.Iterations.DTOs;
using Lucy.Application.Tickets.DTOs;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Lucy.Console.Views.Iterations;

/// <summary>
/// Renders the iteration view to the console.
/// </summary>
public class IterationViewRenderer : IViewRenderer<(IterationDto, IEnumerable<TicketCountByStatusDto>)>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        (IterationDto, IEnumerable<TicketCountByStatusDto>) model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var (iteration, ticketCounts) = model;
        var ticketCountList = ticketCounts.ToList();

        console.WriteLine();

        var title = GetTitle(iteration, localizer, console.Profile);
        var content = BuildContent(iteration, ticketCountList, localizer);

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
    /// Gets the title for the iteration view.
    /// </summary>
    /// <param name="iteration">The iteration to get the title for.</param>
    /// <param name="localizer">The string localizer.</param>
    /// <param name="profile">The console profile.</param>
    /// <returns>The title for the iteration view.</returns>
    private static string GetTitle(IterationDto iteration, IStringLocalizer localizer, Profile profile)
    {
        var baseTitle = profile.Capabilities.Unicode
            ? $":calendar: {localizer["View.ShowIteration.Title"]}"
            : localizer["View.ShowIteration.Title"];

        return $"{baseTitle} [blue][[{iteration.Key}]][/]";
    }

    /// <summary>
    /// Builds the content for the iteration view.
    /// </summary>
    /// <param name="iteration">The iteration to build the content for.</param>
    /// <param name="ticketCounts">The ticket counts by status.</param>
    /// <param name="localizer">The string localizer.</param>
    /// <returns>The content for the iteration view.</returns>
    private static IRenderable BuildContent(IterationDto iteration, List<TicketCountByStatusDto> ticketCounts, IStringLocalizer localizer)
    {
        var grid = new Grid();
        grid.AddColumn(new GridColumn().NoWrap().PadRight(2));
        grid.AddColumn(new GridColumn().PadRight(1));

        // Key
        grid.AddRow($"[bold]{localizer["View.Iteration.Key"]}:[/]", $"[white]{iteration.Key}[/]");

        // Name
        var name = string.IsNullOrWhiteSpace(iteration.Name)
            ? $"[dim]{localizer["Empty.String"]}[/]"
            : iteration.Name;
        grid.AddRow($"[bold]{localizer["View.Iteration.Name"]}:[/]", name);

        // Description
        var description = string.IsNullOrWhiteSpace(iteration.Description)
            ? $"[dim]{localizer["Empty.String"]}[/]"
            : iteration.Description;
        grid.AddRow($"[bold]{localizer["View.Iteration.Description"]}:[/]", description);

        // Start Date
        var startDate = iteration.StartDate.HasValue
            ? iteration.StartDate.Value.ToString("d")
            : $"[dim]{localizer["Empty.String"]}[/]";
        grid.AddRow($"[bold]{localizer["View.Iteration.StartDate"]}:[/]", startDate);

        // End Date
        var endDate = iteration.EndDate.HasValue
            ? iteration.EndDate.Value.ToString("d")
            : $"[dim]{localizer["Empty.String"]}[/]";
        grid.AddRow($"[bold]{localizer["View.Iteration.EndDate"]}:[/]", endDate);

        var sections = new List<IRenderable> { grid };

        // Ticket summary section
        sections.Add(Text.Empty);

        if (ticketCounts.Count == 0)
        {
            sections.Add(new Markup($"[dim]{localizer["View.Iteration.NoTickets"]}[/]"));
        }
        else
        {
            sections.Add(BuildTicketSummary(ticketCounts, localizer));
        }

        return new Rows([.. sections]);
    }

    /// <summary>
    /// Builds the ticket summary showing count by status.
    /// </summary>
    private static IRenderable BuildTicketSummary(List<TicketCountByStatusDto> ticketCounts, IStringLocalizer localizer)
    {
        var separator = new Rule($"[bold]Tickets[/]")
            .LeftJustified()
            .RuleStyle(Style.Parse("grey23"));

        var statusLines = ticketCounts.Select(tc =>
        {
            var color = tc.StatusColor.ToLowerInvariant();
            return new Markup($"[{color}]●[/] [{color}]{tc.StatusName}[/]: [bold]{tc.Count}[/]");
        }).ToArray();

        var rowsList = new List<IRenderable> { separator, Text.Empty };
        rowsList.AddRange(statusLines);

        return new Rows([.. rowsList]);
    }
}
