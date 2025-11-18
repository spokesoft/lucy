using Lucy.Application.Comments.DTOs;
using Lucy.Application.Projects.DTOs;
using Lucy.Application.Tickets.DTOs;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Lucy.Console.Views.Projects;

/// <summary>
/// Renders the project view to the console.
/// </summary>
public class ProjectViewRenderer : IViewRenderer<(ProjectDto, IEnumerable<CommentDto>, IEnumerable<TicketCountByStatusDto>)>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        (ProjectDto, IEnumerable<CommentDto>, IEnumerable<TicketCountByStatusDto>) model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var (project, comments, ticketCounts) = model;
        var commentList = comments.ToList();
        var ticketCountList = ticketCounts.ToList();

        console.WriteLine();

        var title = GetTitle(project, localizer, console.Profile);
        var content = BuildContent(project, commentList, ticketCountList, localizer);

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
    private static Rows BuildContent(ProjectDto project, List<CommentDto> commentList, List<TicketCountByStatusDto> ticketCountList, IStringLocalizer localizer)
    {
        // Project details
        var detailsMarkup = GetProjectDetailsMarkup(project, localizer);
        var detailsText = new Markup(detailsMarkup);

        var sections = new List<IRenderable> { detailsText };

        // Ticket summary section
        if (ticketCountList.Count != 0)
        {
            sections.Add(Text.Empty);
            sections.Add(BuildTicketSummary(ticketCountList, localizer));
        }

        // Comments section
        if (commentList.Count != 0)
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

            sections.Add(Text.Empty);
            sections.Add(separator);
            sections.Add(Text.Empty);
            sections.Add(new Rows(commentsRows));
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

    /// <summary>
    /// Gets the title for the project view.
    /// </summary>
    private static string GetTitle(ProjectDto project, IStringLocalizer localizer, Profile options)
    {
        var baseTitle = options.Capabilities.Unicode
            ? $":file_folder: {localizer["View.ShowProject.Title"]}"
            : localizer["View.ShowProject.Title"];

        return $"{baseTitle} [blue][[{project.Key}]][/]";
    }

    /// <summary>
    /// Gets the project details formatted as markup.
    /// </summary>
    private static string GetProjectDetailsMarkup(ProjectDto project, IStringLocalizer localizer)
    {
        var name = string.IsNullOrWhiteSpace(project.Name)
            ? $"[dim]{localizer["Empty.String"]}[/]"
            : project.Name;

        var description = string.IsNullOrWhiteSpace(project.Description)
            ? $"[dim]{localizer["Empty.String"]}[/]"
            : project.Description;

        return $"[bold]{localizer["Property.Project.Name"]}:[/] {name}\n" +
               $"[bold]{localizer["Property.Project.Description"]}:[/] {description}\n" +
               $"[dim]{localizer["Property.UpdatedAt"]}:[/] [dim]{project.UpdatedAt:yyyy-MM-dd HH:mm:ss}[/]";
    }
}
