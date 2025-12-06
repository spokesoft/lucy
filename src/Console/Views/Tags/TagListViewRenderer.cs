using Lucy.Application.Tags.DTOs;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;

using DomainColor = Lucy.Domain.Enums.Color;

namespace Lucy.Console.Views.Tags;

/// <summary>
/// Renders the tag list view to the console.
/// </summary>
public class TagListViewRenderer : IViewRenderer<IEnumerable<TagDto>>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        IEnumerable<TagDto> model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var tagList = model.ToList();
        var title = GetTitle(localizer, console.Profile);
        var caption = tagList.Any()
            ? GetCaption(localizer, 1, tagList.Count, tagList.Count)
            : localizer["View.TagList.NoTags"];

        var table = new Table()
            .Border(TableBorder.SimpleHeavy)
            .Title(title)
            .Caption(caption, new Style(foreground: Color.Grey))
            .Width(100);

        table.AddColumn(localizer["Property.Id"]);
        table.AddColumn(localizer["Property.Tag.Key"]);
        table.AddColumn(localizer["Property.Tag.Label"]);
        table.AddColumn(localizer["Property.Tag.Description"]);

        foreach (var tag in tagList)
        {
            var label = string.IsNullOrWhiteSpace(tag.Label)
                ? $"[grey70]{localizer["Empty.String"]}[/]"
                : tag.Label;

            var description = string.IsNullOrWhiteSpace(tag.Description)
                ? $"[grey70]{localizer["Empty.String"]}[/]"
                : tag.Description;

            table.AddRow(
                $"[gray]{tag.Id}[/]",
                GetColoredKey(tag.Key, tag.Color),
                label,
                description);
        }

        console.WriteLine();
        console.Write(table);
        console.WriteLine();

        await Task.CompletedTask;
    }

    private static string GetTitle(IStringLocalizer localizer, Profile options)
        => options.Capabilities.Unicode
            ? $":label: [u]{localizer["View.TagList.Title"]}[/]"
            : $"[u]{localizer["View.TagList.Title"]}[/]";

    private static string GetCaption(IStringLocalizer localizer, int firstIndex, int lastIndex, int count)
        => count == 0 ?
            localizer["View.TagList.NoTags"] :
            localizer["View.TagList.Caption", firstIndex, lastIndex, count];

    private static string GetColoredKey(string key, DomainColor color)
    {
        var colorName = color.ToString().ToLower();
        return color switch
        {
            DomainColor.Red => $"[red]{key}[/]",
            DomainColor.Orange => $"[orangered1]{key}[/]",
            DomainColor.Yellow => $"[yellow]{key}[/]",
            DomainColor.Green => $"[green]{key}[/]",
            DomainColor.Blue => $"[blue]{key}[/]",
            DomainColor.Purple => $"[purple]{key}[/]",
            DomainColor.Gray => $"[gray]{key}[/]",
            _ => key
        };
    }
}
