using Microsoft.Extensions.Localization;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Lucy.Console.Views.Projects;

internal class ShowProjectView(
        ShowProjectViewOptions view,
        IStringLocalizer<Program> localizer,
        IAnsiConsole console) : View<ShowProjectViewOptions>(view, localizer, console)
{
    private const string TitleEmoji = "📁";
    private const string TitleResourceKey = "View.ShowProject.Title";

    protected override Measurement Measure(RenderOptions options, int maxWidth)
        => new(maxWidth, maxWidth);

    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        if (_state.Project is null)
        {
            yield return new Segment(_localizer["View.ShowProject.NoProject"], new Style(foreground: Color.Red));
            yield break;
        }

        var panel = new Panel(new Markup(GetProjectDetailsMarkup(_state.Project)))
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("grey"))
            .Padding(1, 2)
            .Header(GetTitle(_localizer, options), Justify.Center);

        foreach (var segment in panel.GetSegments(_console))
        {
            yield return segment;
        }
    }

    private static string GetTitle(IStringLocalizer localizer, RenderOptions option)
        => option.Capabilities.Unicode
            ? $"{TitleEmoji} [u]{localizer[TitleResourceKey]}[/]"
            : $"[u]{localizer[TitleResourceKey]}[/]";

    private static string GetProjectDetailsMarkup(Application.Projects.DTOs.ProjectDto project)
        => $"[bold]{project.Name}[/]\n\n" +
           $"[grey]ID:[/] {project.Id}\n" +
           $"[grey]Key:[/] {project.Key}\n" +
           $"[grey]Description:[/] {project.Description}\n" +
           $"[grey]Created At:[/] {project.CreatedAt:yyyy-MM-dd HH:mm:ss}\n" +
           $"[grey]Updated At:[/] {project.UpdatedAt:yyyy-MM-dd HH:mm:ss}";
}
