using Lucy.Application.Projects.DTOs;
using Lucy.Application.Tags.DTOs;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;

using DomainColor = Lucy.Domain.Enums.Color;

namespace Lucy.Console.Views.Tags;

/// <summary>
/// Renders the tag view to the console.
/// </summary>
public class TagViewRenderer : IViewRenderer<(TagDto, ProjectDto)>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        (TagDto, ProjectDto) model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var (tag, project) = model;
        var title = GetTitle(tag, localizer, console.Profile);
        var content = GetTagDetailsMarkup(tag, project, localizer);

        var panel = new Panel(new Markup(content))
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
    /// Gets the title for the tag view.
    /// </summary>
    private static string GetTitle(TagDto tag, IStringLocalizer localizer, Profile options)
    {
        var baseTitle = options.Capabilities.Unicode
            ? $":label: {localizer["View.ShowTag.Title"]}"
            : localizer["View.ShowTag.Title"];

        var coloredKey = GetColoredKey(tag.Key, tag.Color);
        return $"{baseTitle} {coloredKey}";
    }

    /// <summary>
    /// Gets the tag details formatted as markup.
    /// </summary>
    private static string GetTagDetailsMarkup(TagDto tag, ProjectDto project, IStringLocalizer localizer)
    {
        var label = string.IsNullOrWhiteSpace(tag.Label)
            ? $"[grey70]{Markup.Escape(localizer["Empty.String"])}[/]"
            : Markup.Escape(tag.Label);

        var description = string.IsNullOrWhiteSpace(tag.Description)
            ? $"[grey70]{Markup.Escape(localizer["Empty.String"])}[/]"
            : Markup.Escape(tag.Description);

        var projectKey = Markup.Escape(project.Key);
        var colorDisplay = GetColorDisplay(tag.Color);

        return $"[bold]{Markup.Escape(localizer["Property.Id"])}:[/] {tag.Id}\n"
            + $"[bold]{Markup.Escape(localizer["Property.Status.ProjectKey"])}:[/] {projectKey}\n"
            + $"[bold]{Markup.Escape(localizer["Property.Tag.Key"])}:[/] {Markup.Escape(tag.Key)}\n"
            + $"[bold]{Markup.Escape(localizer["Property.Tag.Label"])}:[/] {label}\n"
            + $"[bold]{Markup.Escape(localizer["Property.Tag.Description"])}:[/] {description}\n"
            + $"[bold]{Markup.Escape(localizer["Property.Tag.Color"])}:[/] {colorDisplay}\n"
            + $"[dim]{Markup.Escape(localizer["Property.CreatedAt"])}:[/] [dim]{tag.CreatedAt:yyyy-MM-dd HH:mm:ss}[/]\n"
            + $"[dim]{Markup.Escape(localizer["Property.UpdatedAt"])}:[/] [dim]{tag.UpdatedAt:yyyy-MM-dd HH:mm:ss}[/]";
    }

    /// <summary>
    /// Gets the colored key with brackets for a tag.
    /// </summary>
    private static string GetColoredKey(string key, DomainColor color)
    {
        return color switch
        {
            DomainColor.Red => $"[red][[{key}]][/]",
            DomainColor.Orange => $"[orangered1][[{key}]][/]",
            DomainColor.Yellow => $"[yellow][[{key}]][/]",
            DomainColor.Green => $"[green][[{key}]][/]",
            DomainColor.Blue => $"[blue][[{key}]][/]",
            DomainColor.Purple => $"[purple][[{key}]][/]",
            DomainColor.Gray => $"[gray][[{key}]][/]",
            _ => $"[[{key}]]"
        };
    }

    /// <summary>
    /// Gets the color display markup for a tag color.
    /// </summary>
    private static string GetColorDisplay(DomainColor color)
    {
        var colorName = color.ToString().ToLower();
        return color switch
        {
            DomainColor.Red => $"[red]●[/] {colorName}",
            DomainColor.Orange => $"[orangered1]●[/] {colorName}",
            DomainColor.Yellow => $"[yellow]●[/] {colorName}",
            DomainColor.Green => $"[green]●[/] {colorName}",
            DomainColor.Blue => $"[blue]●[/] {colorName}",
            DomainColor.Purple => $"[purple]●[/] {colorName}",
            DomainColor.Gray => $"[gray]●[/] {colorName}",
            _ => colorName
        };
    }
}
