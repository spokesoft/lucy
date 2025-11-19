using Lucy.Application.Projects.DTOs;
using Lucy.Application.Statuses.DTOs;
using Lucy.Application.Tickets.DTOs;
using Lucy.Console.Interfaces;
using Lucy.Domain.Enums;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Lucy.Console.Views.Statuses;

/// <summary>
/// Renders the status view to the console.
/// </summary>
public class StatusViewRenderer : IViewRenderer<(StatusDto, ProjectDto, List<TicketDto>)>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        (StatusDto, ProjectDto, List<TicketDto>) model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var (status, project, tickets) = model;
        var title = GetTitle(status, localizer, console.Profile);
        var content = GetStatusDetailsMarkup(status, project, localizer);

        // Detect --include-tickets option from command line args
        var includeTickets = Environment.GetCommandLineArgs().Any(arg => arg == "--include-tickets");

        var sections = new List<Spectre.Console.Rendering.IRenderable> { new Markup(content) };
        if (includeTickets)
        {
            sections.Add(Text.Empty);
            var ticketCount = tickets?.Count ?? 0;
            var ticketsLabel = $"[bold]Tickets ({ticketCount})[/]";
            sections.Add(new Rule(ticketsLabel).LeftJustified().RuleStyle(Style.Parse("grey23")));
            sections.Add(Text.Empty);

            if (tickets != null)
            {
                foreach (var ticket in tickets)
                {
                    var key = ticket.Key ?? "";
                    var keyWidth = key.Length;
                    var paddedHeader = key.PadRight(keyWidth);
                    var ticketTitle = string.IsNullOrWhiteSpace(ticket.Title) ? "" : Markup.Escape(ticket.Title);
                    string descriptionText = ticket.Description ?? "";
                    // Roughly 2 lines at 80 chars per line
                    int maxDescLength = 100;
                    bool isTruncated = descriptionText.Length > maxDescLength;
                    if (isTruncated)
                        descriptionText = descriptionText.Substring(0, maxDescLength).TrimEnd() + "...";
                    var description = string.IsNullOrWhiteSpace(descriptionText) ? "" : $"\n[dim]{Markup.Escape(descriptionText)}[/]";
                    var body = ticketTitle + description;
                    var ticketPanel = new Panel(new Markup(body))
                        .Header(Markup.Escape(paddedHeader), Justify.Left)
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(Style.Parse("grey23"))
                        .Padding(1, 0);
                    sections.Add(ticketPanel);
                }
            }
        }

        var panel = new Panel(new Rows(sections))
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
    /// Gets the title for the status view.
    /// </summary>
    private static string GetTitle(StatusDto status, IStringLocalizer localizer, Profile options)
    {
        var baseTitle = options.Capabilities.Unicode
            ? $":bookmark: {localizer["View.ShowStatus.Title"]}"
            : localizer["View.ShowStatus.Title"];

        var coloredKey = GetColoredKey(status.Key, status.Color);
        return $"{baseTitle} {coloredKey}";
    }

    /// <summary>
    /// Gets the status details formatted as markup.
    /// </summary>
    private static string GetStatusDetailsMarkup(StatusDto status, ProjectDto project, IStringLocalizer localizer)
    {
        var name = string.IsNullOrWhiteSpace(status.Name)
            ? $"[grey70]{Markup.Escape(localizer["Empty.String"])}[/]"
            : Markup.Escape(status.Name);

        var description = string.IsNullOrWhiteSpace(status.Description)
            ? $"[grey70]{Markup.Escape(localizer["Empty.String"])}[/]"
            : Markup.Escape(status.Description);

        var projectKey = Markup.Escape(project.Key);

        return $"[bold]{Markup.Escape(localizer["Property.Id"])}:[/] {status.Id}\n"
            + $"[bold]{Markup.Escape(localizer["Property.Status.ProjectKey"])}:[/] {projectKey}\n"
            + $"[bold]{Markup.Escape(localizer["Property.Status.Order"])}:[/] {status.Order}\n"
            + $"[bold]{Markup.Escape(localizer["Property.Status.Name"])}:[/] {name}\n"
            + $"[bold]{Markup.Escape(localizer["Property.Status.Description"])}:[/] {description}\n"
            + $"[dim]{Markup.Escape(localizer["Property.UpdatedAt"])}:[/] [dim]{status.UpdatedAt:yyyy-MM-dd HH:mm:ss}[/]";
    }

    /// <summary>
    /// Gets the colored key with brackets for a status.
    /// </summary>
    private static string GetColoredKey(string key, StatusColor color)
    {
        return color switch
        {
            StatusColor.Red => $"[red][[{key}]][/]",
            StatusColor.Orange => $"[orangered1][[{key}]][/]",
            StatusColor.Yellow => $"[yellow][[{key}]][/]",
            StatusColor.Green => $"[green][[{key}]][/]",
            StatusColor.Blue => $"[blue][[{key}]][/]",
            StatusColor.Purple => $"[purple][[{key}]][/]",
            StatusColor.Gray => $"[gray][[{key}]][/]",
            _ => $"[[{key}]]"
        };
    }

    /// <summary>
    /// Gets the color display markup for a status color.
    /// </summary>
    private static string GetColorDisplay(StatusColor color)
    {
        var colorName = color.ToString().ToLower();
        return color switch
        {
            StatusColor.Red => $"[red]●[/] {colorName}",
            StatusColor.Orange => $"[orangered1]●[/] {colorName}",
            StatusColor.Yellow => $"[yellow]●[/] {colorName}",
            StatusColor.Green => $"[green]●[/] {colorName}",
            StatusColor.Blue => $"[blue]●[/] {colorName}",
            StatusColor.Purple => $"[purple]●[/] {colorName}",
            StatusColor.Gray => $"[gray]●[/] {colorName}",
            _ => colorName
        };
    }
}
