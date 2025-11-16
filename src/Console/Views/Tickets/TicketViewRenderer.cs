using Lucy.Application.Tickets.DTOs;
using Lucy.Console.Interfaces;
using Microsoft.Extensions.Localization;
using Spectre.Console;

namespace Lucy.Console.Views.Tickets;

/// <summary>
/// Renders the ticket view to the console.
/// </summary>
public class TicketViewRenderer : IViewRenderer<TicketDto>
{
    /// <inheritdoc />
    public async Task RenderAsync(
        TicketDto model,
        IAnsiConsole console,
        IStringLocalizer localizer,
        CancellationToken token = default)
    {
        var title = GetTitle(localizer, console.Profile);
        var content = GetTicketDetailsMarkup(model, localizer);

        var panel = new Panel(new Markup(content))
            .Header(title, Justify.Center)
            .Border(BoxBorder.Rounded)
            .BorderStyle(Style.Parse("grey"))
            .Padding(1, 2);

        console.Write(panel);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Gets the title for the ticket view.
    /// </summary>
    private static string GetTitle(IStringLocalizer localizer, Profile options)
        => options.Capabilities.Unicode
            ? $":ticket: [u]{localizer["View.ShowTicket.Title"]}[/]"
            : $"[u]{localizer["View.ShowTicket.Title"]}[/]";

    /// <summary>
    /// Gets the ticket details formatted as markup.
    /// </summary>
    private static string GetTicketDetailsMarkup(TicketDto ticket, IStringLocalizer localizer)
    {
        var ticketTitle = string.IsNullOrWhiteSpace(ticket.Title)
            ? $"[grey70]{localizer["Empty.String"]}[/]"
            : ticket.Title;

        var description = string.IsNullOrWhiteSpace(ticket.Description)
            ? $"[grey70]{localizer["Empty.String"]}[/]"
            : ticket.Description;

        return $"[grey]{localizer["Property.Id"]}:[/] {ticket.Id}\n" +
               $"[grey]{localizer["Property.Project.Id"]}:[/] {ticket.ProjectId}\n" +
               $"[grey]{localizer["Property.StatusId"]}:[/] {ticket.StatusId}\n" +
               $"[grey]{localizer["Property.Ticket.Key"]}:[/] [blue]{ticket.Key}[/]\n" +
               $"[grey]{localizer["Property.Ticket.Title"]}:[/] {ticketTitle}\n" +
               $"[grey]{localizer["Property.Ticket.Description"]}:[/] {description}\n" +
               $"[grey]{localizer["Property.CreatedAt"]}:[/] {ticket.CreatedAt:yyyy-MM-dd HH:mm:ss}\n" +
               $"[grey]{localizer["Property.UpdatedAt"]}:[/] {ticket.UpdatedAt:yyyy-MM-dd HH:mm:ss}";
    }
}
