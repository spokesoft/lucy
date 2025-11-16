using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Settings for the 'list comments' command.
/// </summary>
public class ListCommentsCommand : ListCommand
{
    /// <summary>
    /// The key of the project or ticket to list comments for.
    /// </summary>
    [CommandArgument(0, "[key]")]
    [Description("The key of the project or ticket to list comments for (e.g., 'ABC' for project or 'ABC-123' for ticket).")]
    public string? Key { get; set; }

    /// <summary>
    /// The ID of the project to list comments for.
    /// </summary>
    [CommandOption("-p|--project-id <id>")]
    [Description("The ID of the project to list comments for.")]
    public long? ProjectId { get; set; }

    /// <summary>
    /// The ID of the ticket to list comments for.
    /// </summary>
    [CommandOption("-t|--ticket-id <id>")]
    [Description("The ID of the ticket to list comments for.")]
    public long? TicketId { get; set; }
}
