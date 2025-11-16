using Lucy.Application.Comments.DTOs;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;

namespace Lucy.Console.Views.Comments;

/// <summary>
/// Renders the comment list view to the console.
/// </summary>
public class CommentListViewRenderer : IViewRenderer<IEnumerable<CommentDto>>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        IEnumerable<CommentDto> model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var commentList = model.ToList();
        var title = GetTitle(localizer, console.Profile);
        var caption = commentList.Any()
            ? GetCaption(localizer, 1, commentList.Count, commentList.Count)
            : localizer["View.CommentList.NoComments"];

        var table = new Table()
            .Border(TableBorder.SimpleHeavy)
            .Title(title)
            .Caption(caption, new Style(foreground: Color.Grey));

        table.AddColumn(localizer["Property.Id"]);
        table.AddColumn(localizer["Property.Comment.Type"]);
        table.AddColumn(localizer["Property.Comment.Content"]);
        table.AddColumn(localizer["Property.UpdatedAt"]);

        foreach (var comment in commentList)
        {
            var content = string.IsNullOrWhiteSpace(comment.Content)
                ? $"[grey70]{localizer["Empty.String"]}[/]"
                : comment.Content;

            var commentType = comment.CommentType.ToString();

            table.AddRow(
                $"[gray]{comment.Id}[/]",
                $"[yellow]{commentType}[/]",
                content,
                comment.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        console.Write(table);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Gets the title for the comment list view.
    /// </summary>
    private static string GetTitle(IStringLocalizer localizer, Profile options)
        => options.Capabilities.Unicode
            ? $":speech_balloon: [u]{localizer["View.CommentList.Title"]}[/]"
            : $"[u]{localizer["View.CommentList.Title"]}[/]";

    /// <summary>
    /// Gets the caption for the comment list view.
    /// </summary>
    private static string GetCaption(IStringLocalizer localizer, int firstIndex, int lastIndex, int count)
        => count == 0 ?
            localizer["View.CommentList.NoComments"] :
            localizer["View.CommentList.Caption", firstIndex, lastIndex, count];
}
