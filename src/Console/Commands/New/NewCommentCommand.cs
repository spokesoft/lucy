using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Settings for the 'new comment' command.
/// </summary>
/// <remarks>
/// A comment can be added to either a project or ticket identified by their key or ID.
/// </remarks>
public class NewCommentCommand : NewCommand
{
    /// <summary>
    /// The key of the project or ticket to comment on.
    /// </summary>
    [CommandArgument(0, "[key]")]
    [Description("The key of the project or ticket to comment on (e.g., 'ABC' for project or 'ABC-123' for ticket).")]
    public string? Key { get; set; }

    /// <summary>
    /// The ID of the project to comment on.
    /// </summary>
    [CommandOption("-p|--project-id <id>")]
    [Description("The ID of the project to comment on.")]
    public long? ProjectId { get; set; }

    /// <summary>
    /// The ID of the ticket to comment on.
    /// </summary>
    [CommandOption("-t|--ticket-id <id>")]
    [Description("The ID of the ticket to comment on.")]
    public long? TicketId { get; set; }

    /// <summary>
    /// The content of the comment.
    /// </summary>
    [CommandOption("-c|--content <CONTENT>")]
    [Description("The content of the comment.")]
    public string? Content { get; set; }
}
