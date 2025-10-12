using Lucy.Application.Projects.DTOs;
using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Lucy.Console.Views.Projects;

/// <summary>
/// Renders a list of projects in a table format.
/// </summary>
internal class ListProjectsView(
    ListProjectsViewOptions view,
    IStringLocalizer<Program> localizer,
    IAnsiConsole console) : View<ListProjectsViewOptions>(view, localizer, console)
{
    private static readonly string TitleEmoji = ":file_folder:";
    private static readonly string TitleResourceKey = "View.ListProjects.Title";

    /// <inheritdoc />
    protected override Measurement Measure(RenderOptions options, int maxWidth)
        => new(0, maxWidth);

    /// <inheritdoc />
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var table = new Table()
            .Border(TableBorder.SimpleHeavy)
            .Title(GetTitle(_localizer, options))
            .Caption(GetCaption(_localizer, 1, _state.Projects.Count(), _state.TotalCount), new Style(foreground: Color.Grey));

        table.AddColumn(_localizer["Property.Id"]);
        table.AddColumn(_localizer["Property.Project.Key"]);
        table.AddColumn(_localizer["Property.Project.Name"]);
        table.AddColumn(_localizer["Property.Project.Description"]);
        table.AddColumn(_localizer["Property.UpdatedAt"]);

        foreach (var project in _state.Projects)
        {
            table.AddRow(
                $"[gray]{project.Id}[/]",
                $"[blue]{project.Key}[/]",
                string.IsNullOrWhiteSpace(project.Name)
                    ? $"[gray]{_localizer["Empty.String"]}[/]"
                    : project.Name,
                string.IsNullOrWhiteSpace(project.Description)
                    ? $"[gray]{_localizer["Empty.String"]}[/]"
                    : project.Description,
                project.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        return [
            Segment.LineBreak,
            ..table.GetSegments(_console),
            Segment.LineBreak];
    }

    private static string GetTitle(IStringLocalizer localizer, RenderOptions option)
        => option.Capabilities.Unicode
            ? $"{TitleEmoji} [u]{localizer[TitleResourceKey]}[/]"
            : $"[u]{localizer[TitleResourceKey]}[/]";

    private static string GetCaption(IStringLocalizer localizer, int firstIndex, int lastIndex, int count)
        => localizer["View.ListProjects.Caption", firstIndex, lastIndex, count];
}
